using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Right, Down, Left }
public class LevelManager : MonoBehaviour
{

    public Dictionary<Direction, float> direction = new()
    {
        { Direction.Up, 0f },
        { Direction.Right, 90f },
        { Direction.Down, 180f },
        { Direction.Left, -90f }
    };
}

