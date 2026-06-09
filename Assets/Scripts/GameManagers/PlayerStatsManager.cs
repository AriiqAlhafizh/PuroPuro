using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager instance;

    public event Action<Lane> OnLaneChange;

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
    public bool inIFrame = false;

    [Header("Debuffs")]
    public bool isInForcedPOV = false;
    public bool cantReload = false;       // gabisa reload doang
    public bool cantReloadnShoot = false;    // gabisa nembak & reload
    public float bindDuration = 2f;

    public void ChangeLane(Lane newLane)
    {
        OnLaneChange?.Invoke(newLane);
    }

    public void TakeDamage()
    {
        Camera.main.GetComponent<ScreenShake>().TriggerShake();
        if (!inIFrame)
        {
            health--;
            if (health <= 0)
            {
                // logic mati
            }
        }
    }

    public void StartIFrame()
    {
        inIFrame = true;
    }

    public void EndIFrame()
    {
        inIFrame = false;
    }

    public void ApplyBind()
    {
        if (!cantReload)
            StartCoroutine(BindCoroutine());
    }
    private IEnumerator BindCoroutine()
    {
        cantReload = true;
        yield return new WaitForSeconds(bindDuration);
        cantReload = false;
    }

    public void ApplyParalyze()
    {
        cantReloadnShoot = true;
    }

    public void RemoveParalyze()
    {
        cantReloadnShoot = false;
    }

    public void DEBUG_TakeDamage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TakeDamage();
        }
    }
}
