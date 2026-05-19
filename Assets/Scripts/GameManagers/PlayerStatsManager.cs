using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager instance;

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

    public void TakeDamage()
    {
        Camera.main.GetComponent<ScreenShake>().TriggerShake();
    }
  
    public void DEBUG_TakeDamage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TakeDamage();
        }
    }
}
