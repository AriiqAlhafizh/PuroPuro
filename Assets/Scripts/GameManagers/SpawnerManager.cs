using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerManager : MonoBehaviour
{

    [SerializeField] private List<Transform>  SpawnPoint;
    [SerializeField] private List<GameObject> SpawnObject;

    public static SpawnerManager Instance { get; private set; }

    void Start()
    {

        Spawn();
    }

    void Spawn()
    {
        int randomIndex = UnityEngine.Random.Range(0, SpawnPoint.Count);
        Instantiate(SpawnObject[0], SpawnPoint[randomIndex].position, Quaternion.identity);
    }
}