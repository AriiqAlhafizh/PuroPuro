using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum Lane { 
    Up, 
    Right, 
    Down,
    Left 
}
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<List<Lane>> levelAllowedDirection = new()
        {
        // Level 1: Only Up 
        new List<Lane> { Lane.Up },
        // Level 2: Up, Down
        new List<Lane> { Lane.Up, Lane.Down },
        // Level 3: Up, Down, Left
        new List<Lane> { Lane.Up, Lane.Down, Lane.Left },
        // Level 4: All directions
        new List<Lane> { Lane.Up, Lane.Right, Lane.Down, Lane.Left }
    };

    public List<Dictionary<Lane, float>> levelDirectionAngles = new()
    {
        // Level 1: Only Up 
        {   new Dictionary<Lane, float>
            {
                { Lane.Up, 0f }
            }
        },
        // Level 2: Up, Down
        {   new Dictionary<Lane, float>
            {
                { Lane.Up, 0f },
                { Lane.Down, 180f }
            }
        },
        // Level 3: Up, Down, Left
        {   new Dictionary<Lane, float>
            {
                { Lane.Up, 0f },
                { Lane.Down, -180f },
                { Lane.Left, -90f }
            }
        },
        // Level 4: All directions
        {   new Dictionary<Lane, float>
            {
                { Lane.Up, 0f },
                { Lane.Right, 90f },
                { Lane.Down, 180f },
                { Lane.Left, -90f }
            }
        }
    };

    public List<float> levelTimes = new()
    {
        45f,   
        60f,   
        90f,   
        180f   
    };
    public float GetLevelTime(int level)
    {
        return levelTimes.Count >= level
            ? levelTimes[level - 1]
            : 60f; // default time if level exceeds defined times
    }

    public List<Lane> GetAllowedLanes(int level)
    {
        return levelAllowedDirection.Count >= level
            ? levelAllowedDirection[level - 1]
            : new List<Lane>();
    }
    public Dictionary<Lane, float> GetAllowedAngles(int level)
    {
        return levelDirectionAngles.Count >= level
            ? levelDirectionAngles[level - 1]
            : new Dictionary<Lane, float>();
    }

    [Header("Level Settings")]
    public int currentLevel = 1;

    public void AdvanceLevel()
    {
        StartCoroutine(AdvanceLevelCoroutine());
    }

    public IEnumerator AdvanceLevelCoroutine()
    {
        currentLevel++;
        PlayerStatsManager.instance.StartIFrame();
        SpawnerManager.Instance.spawningEnabled = false;
        TimerManager.instance.ResetTimer();
        yield return SceneTransitionManager.Instance.TransitionToScene(currentLevel);
        TimerManager.instance.StartTimer();
        PlayerStatsManager.instance.EndIFrame();
        SpawnerManager.Instance.spawningEnabled = true;
    }
}

