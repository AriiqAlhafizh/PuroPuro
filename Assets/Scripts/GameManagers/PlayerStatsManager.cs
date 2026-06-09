using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager instance;

    public event Action<Lane> OnLaneChange;
    public Lane currentLane = Lane.Up;

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

    [Header("Player Stats")]
    public int health = 3;
    public int maxBullets = 6;
    public int currentBullets = 6;
    public bool isReloading = false;

    [Header("Debuffs")]
    public bool isInForcedPOV = false;
    public bool isBinded = false;
    public bool isParalyzed = false;

    public void ChangeLane(Lane newLane)
    {
        currentLane = newLane;
        OnLaneChange?.Invoke(newLane);
    }

    public void TakeDamage()
    {
        Camera.main.GetComponent<ScreenShake>().TriggerShake();
        health--;
    }
  
    public void DEBUG_TakeDamage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TakeDamage();
        }
    }
}
