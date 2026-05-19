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
