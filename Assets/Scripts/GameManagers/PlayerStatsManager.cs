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
    public static int health = 3;
    public static int maxBullets = 6;
    public static int currentBullets = 6;
    public static bool isReloading = false;

    [Header("Debuffs")]
    public static bool isInForcedPOV = false;
    public static bool isBinded = false;
    public static bool isParalyzed = false;

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
