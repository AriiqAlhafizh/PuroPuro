using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private EnemyTierSO enemyTierSO; 

    private List<GameObject> normalEnemyPrefab;
    private List<GameObject> tier2EnemyPrefab;
    private List<GameObject> tier3EnemyPrefab;
    private List<GameObject> tier4EnemyPrefab;

    private List<Transform> spawnPoints;

    void Awake()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        if (enemyTierSO != null)
        {
            normalEnemyPrefab = enemyTierSO.normalEnemyPrefab ?? new List<GameObject>();
            tier2EnemyPrefab = enemyTierSO.tier2EnemyPrefab ?? new List<GameObject>();
            tier3EnemyPrefab = enemyTierSO.tier3EnemyPrefab ?? new List<GameObject>();
            tier4EnemyPrefab = enemyTierSO.tier4EnemyPrefab ?? new List<GameObject>();
        }
        else
        {
            Debug.LogWarning("EnemyTierSO is not assigned on EnemySpawnerManager. Enemy prefab lists will be empty.");
            normalEnemyPrefab = new List<GameObject>();
            tier2EnemyPrefab = new List<GameObject>();
            tier3EnemyPrefab = new List<GameObject>();
            tier4EnemyPrefab = new List<GameObject>();
        }
    }

    void Start()
    {

    }

    public void SpawnEnemy(EnemyTier tier)
    {
        List<GameObject> prefabList = GetPrefabListForTier(tier);
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogWarning($"No prefabs assigned for tier {tier}");
            return;
        }
        GameObject prefabToSpawn = prefabList[Random.Range(0, prefabList.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
    }

    private List<GameObject> GetPrefabListForTier(EnemyTier tier)
    {
        switch (tier)
        {
            case EnemyTier.Normal:
                return normalEnemyPrefab;
            case EnemyTier.EliteI:
                return tier2EnemyPrefab;
            case EnemyTier.EliteII:
                return tier3EnemyPrefab;
            case EnemyTier.EliteIII:
                return tier4EnemyPrefab;
            default:
                Debug.LogError($"Unhandled enemy tier: {tier}");
                return null;
        }
    }
}
