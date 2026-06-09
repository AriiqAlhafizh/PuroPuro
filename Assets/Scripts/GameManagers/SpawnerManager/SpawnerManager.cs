using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTier { 
    Normal = 0, 
    EliteI = 1,
    EliteII = 2, 
    EliteIII = 3
}

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> SpawnPointsParent;

    [SerializeField, Range(0.01f, 0.99f)] private float weightDecayFactor = 0.3f;

    [System.Serializable]
    public class TierConfig
    {
        public EnemyTier tier = EnemyTier.Normal;
        public string displayName = "";
        public float baseCooldown = 3f;
        public int enableAtDifficulty = 0;
        [Tooltip("Minimum multiplier applied at max difficulty (lerp target). 1 = no change, lower = faster spawns")]
        public float minMultiplier = 0.6f;
    }

    [SerializeField] private List<TierConfig> tierConfigs = new List<TierConfig>()
    {
        new TierConfig(){ tier = EnemyTier.Normal, displayName = "Normal", baseCooldown = 3f, enableAtDifficulty = 0, minMultiplier = 0.6f },
        new TierConfig(){ tier = EnemyTier.EliteI, displayName = "Elite I", baseCooldown = 10f, enableAtDifficulty = 1, minMultiplier = 0.8f },
        new TierConfig(){ tier = EnemyTier.EliteII, displayName = "Elite II", baseCooldown = 20f, enableAtDifficulty = 2, minMultiplier = 0.85f },
        new TierConfig(){ tier = EnemyTier.EliteIII, displayName = "Elite III", baseCooldown = 30f, enableAtDifficulty = 3, minMultiplier = 0.9f }
    };

    private float[] cooldownMultipliers;
    private float[] timeSinceLastSpawn;
    private bool[] tierEnabled;

    private List<float> spawnWeights;

    public static SpawnerManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        // ensure we have at least one config
        if (tierConfigs == null || tierConfigs.Count == 0)
        {
            tierConfigs = new List<TierConfig>() { new TierConfig() };
        }

        // sort configs by enum value to make indexing stable
        tierConfigs.Sort((a, b) => ((int)a.tier).CompareTo((int)b.tier));

        int tiers = tierConfigs.Count;
        cooldownMultipliers = new float[tiers];
        timeSinceLastSpawn = new float[tiers];
        tierEnabled = new bool[tiers];
        for (int i = 0; i < tiers; i++)
        {
            cooldownMultipliers[i] = 1f;
            timeSinceLastSpawn[i] = 0f;
            tierEnabled[i] = (i == 0); // only first (normal) enabled by default
        }

        InitializeSpawnWeights();
    }

    void Start()
    {
        if (TimerManager.instance != null)
            TimerManager.instance.OnDifficultyChanged += OnDifficultyChanged;

        StartCoroutine(SpawnLoop());
    }

    void OnDestroy()
    {
        if (TimerManager.instance != null)
            TimerManager.instance.OnDifficultyChanged -= OnDifficultyChanged;
    }

    private void InitializeSpawnWeights()
    {
        int n = (SpawnPointsParent != null) ? SpawnPointsParent.Count : 0;
        spawnWeights = new List<float>(n);
        if (n == 0) return;
        float w = 1f / n;
        for (int i = 0; i < n; i++) spawnWeights.Add(w);
    }

    void OnDifficultyChanged(Difficulty difficulty)
    {
        Debug.Log("Difficulty changed to: " + difficulty);
        int level = (int)difficulty;

        float t = Mathf.Clamp01(level / 2f);

        for (int i = 0; i < tierConfigs.Count; i++)
        {
            var cfg = tierConfigs[i];
            tierEnabled[i] = level >= cfg.enableAtDifficulty;
            cooldownMultipliers[i] = Mathf.Lerp(1f, cfg.minMultiplier, t);
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < timeSinceLastSpawn.Length; i++) timeSinceLastSpawn[i] += dt;

            for (int tier = timeSinceLastSpawn.Length - 1; tier >= 0; tier--)
            {
                if (!tierEnabled[tier]) continue;
                float cooldown = tierConfigs[tier].baseCooldown * cooldownMultipliers[tier];
                if (cooldown <= 0f) cooldown = 0.01f;
                if (timeSinceLastSpawn[tier] >= cooldown)
                {
                    SpawnAtRandomPoint(tier);
                    timeSinceLastSpawn[tier] = 0f;
                    break;
                }
            }

            yield return null;
        }
    }

    private void SpawnAtRandomPoint(int tier)
    {
        if (SpawnPointsParent == null || SpawnPointsParent.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned to SpawnerManager.");
            return;
        }

        if (spawnWeights == null || spawnWeights.Count != SpawnPointsParent.Count) InitializeSpawnWeights();

        int index = PickSpawnPointIndex();
        
        SpawnPointsParent[index].GetComponent<EnemySpawnerManager>().SpawnEnemy(tierConfigs[tier].tier);

        ReduceChosenWeight(index);
    }

    private string GetTierDisplayName(int tier)
    {
        if (tier < 0 || tier >= tierConfigs.Count) return "Unknown";
        var d = tierConfigs[tier].displayName;
        if (!string.IsNullOrEmpty(d)) return d;
        return tierConfigs[tier].tier.ToString();
    }

    private int PickSpawnPointIndex()
    {
        float total = 0f;
        for (int i = 0; i < spawnWeights.Count; i++) total += spawnWeights[i];
        if (total <= 0f)
        {
            InitializeSpawnWeights();
            total = 0f;
            for (int i = 0; i < spawnWeights.Count; i++) total += spawnWeights[i];
        }

        float r = Random.value * total;
        float cum = 0f;
        for (int i = 0; i < spawnWeights.Count; i++)
        {
            cum += spawnWeights[i];
            if (r <= cum) return i;
        }

        return spawnWeights.Count - 1;
    }

    private void ReduceChosenWeight(int index)
    {
        if (spawnWeights == null || index < 0 || index >= spawnWeights.Count) return;

        spawnWeights[index] *= weightDecayFactor;

        float sum = 0f;
        for (int i = 0; i < spawnWeights.Count; i++) sum += spawnWeights[i];
        if (sum <= 0f)
        {
            InitializeSpawnWeights();
            return;
        }
        for (int i = 0; i < spawnWeights.Count; i++) spawnWeights[i] /= sum;
    }
}
