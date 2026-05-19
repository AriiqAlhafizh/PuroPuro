using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerManager : MonoBehaviour
{

    [SerializeField] private List<GameObject> SpawnPointsParent;

    public static SpawnerManager Instance { get; private set; }


    void Start()
    {
        TimerManager.instance.OnDifficultyChanged += OnDifficultyChanged;
    }
    void OnDifficultyChanged(Difficulty difficulty)
    {
        Debug.Log("Difficulty changed to: " + difficulty);
    }
}