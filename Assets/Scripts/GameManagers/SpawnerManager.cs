using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform spawnTarget;

    public static SpawnerManager Instance { get; private set; }

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        
    }
}
