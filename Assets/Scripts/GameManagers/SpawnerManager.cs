using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerManager : MonoBehaviour
{

    [SerializeField] private List<Transform>  SpawnPoint;
    [SerializeField] private List<GameObject> SpawnObject;

    public static SpawnerManager Instance { get; private set; }
    public float Cooldown { get; private set; }

    void Start()
    {
        
        //Spawn();
    }

    void Spawn()
    {
        int randomIndex = Random.Range(0, SpawnPoint.Count);
        int randomObjectIndex = Random.Range(0, SpawnObject.Count);
        Instantiate(SpawnObject[randomObjectIndex], SpawnPoint[randomIndex].position, Quaternion.identity);
    }

    private void Update()
    {
        if(Cooldown <= 0)
        {
            Spawn();
            Cooldown = 0.5f; // Reset cooldown after spawning
        }
        else
        {
            Cooldown -= Time.deltaTime; // Decrease cooldown over time
        }
    }
}