using System.Collections.Generic;
using UnityEngine;

public enum Direction { 
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

    public List<List<Direction>> levelAllowedDirection = new()
        {
        // Level 1: Only Up 
        new List<Direction> { Direction.Up },
        // Level 2: Up, Down
        new List<Direction> { Direction.Up, Direction.Down },
        // Level 3: Up, Right, Left
        new List<Direction> { Direction.Up, Direction.Right, Direction.Left },
        // Level 4: All directions
        new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left }
    };

    public List<Dictionary<Direction, float>> levelDirectionAngles = new()
    {
        // Level 1: Only Up 
        {   new Dictionary<Direction, float>
            {
                { Direction.Up, 0f }
            }
        },
        // Level 2: Up, Down
        {   new Dictionary<Direction, float>
            {
                { Direction.Up, 0f },
                { Direction.Down, 180f }
            }
        },
        // Level 3: Up, Left, Right
        {   new Dictionary<Direction, float>
            {
                { Direction.Up, 0f },
                { Direction.Right, 90f },
                { Direction.Left, -90f }
            }
        },
        // Level 4: All directions
        {   new Dictionary<Direction, float>
            {
                { Direction.Up, 0f },
                { Direction.Right, 90f },
                { Direction.Down, 180f },
                { Direction.Left, -90f }
            }
        }
    };
    
    public List<Direction> GetAllowedDirections(int level)
    {
        return levelAllowedDirection.Count >= level
            ? levelAllowedDirection[level - 1]
            : new List<Direction>();
    }
    public Dictionary<Direction, float> GetAllowedAngles(int level)
    {
        return levelDirectionAngles.Count >= level
            ? levelDirectionAngles[level - 1]
            : new Dictionary<Direction, float>();
    }
}

