using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GunAudio gunAudio;
    public UDPManager UDPManager;
    public GyroCursor gyroCursor;

    [Header("Settings")]
    public float reloadDuration = .5f;

    [Header("Debuffs")]
    public float bindDuration = 2f;
    public float paralyzeDuration = 2f;

    private void Start()
    {
        PlayerStatsManager.instance.currentBullets = PlayerStatsManager.instance.maxBullets;
    }

    private bool previousTriggerPressed = false;

    private void Update()
    {
        // Detect rising edge: triggerPressed goes from false to true
        if (UDPManager.triggerPressed && !previousTriggerPressed)
        {
            Shoot();
        }
        previousTriggerPressed = UDPManager.triggerPressed;

    }
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed && !ControlManager.instance.isGyroEnabled)
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        if (PlayerStatsManager.instance.currentBullets > 0
            && !PlayerStatsManager.instance.isReloading
            && !PlayerStatsManager.instance.isParalyzed)
        {
            Ray ray = ControlManager.instance.isGyroEnabled
            ? Camera.main.ScreenPointToRay(gyroCursor.crosshair.position)
            : Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Hitbox type = hit.collider.GetComponent<HitboxType>().hitbox;
                    if (type == Hitbox.Body) // hit body
                    {
                        ScoreManager.instance.IncreaseScore(50);
                    }
                    else // hit head
                    {
                        ScoreManager.instance.IncreaseScore(100);
                    }

                    EnemyBehavior enemy = hit.collider.GetComponentInParent<EnemyBehavior>();
                    enemy.OnBeingHit();
                    Debug.Log("Hit: " + hit.collider.name + " " + type);

                    ScoreManager.instance.IncreaseHitShot();
                }
                ScoreManager.instance.IncreaseTotalShot();
            }

            gunAudio.PlayShootAudio();
            PlayerStatsManager.instance.currentBullets--;
            if (PlayerStatsManager.instance.currentBullets <= 0)
            {
                StartCoroutine(ReloadCoroutine());
            }

            UDPManager.SendPlayerData(PlayerStatsManager.instance.currentBullets, PlayerStatsManager.instance.health);

            // Debug
            Debug.Log("Bullets left: " + PlayerStatsManager.instance.currentBullets);
        }
    }
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed 
            && !PlayerStatsManager.instance.isReloading
            && !PlayerStatsManager.instance.isBinded 
            && !PlayerStatsManager.instance.isParalyzed
            && PlayerStatsManager.instance.currentBullets < PlayerStatsManager.instance.maxBullets - 1)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        gunAudio.PlayReloadAudio();
        PlayerStatsManager.instance.isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        PlayerStatsManager.instance.currentBullets = PlayerStatsManager.instance.maxBullets;
        PlayerStatsManager.instance.isReloading = false;
        UDPManager.SendPlayerData(PlayerStatsManager.instance.currentBullets, PlayerStatsManager.instance.health);
        Debug.Log("Reloaded! Bullets: " + PlayerStatsManager.instance.currentBullets);
    }

    IEnumerator BindCoroutine()
    {
        PlayerStatsManager.instance.isBinded = true;
        yield return new WaitForSeconds(bindDuration);
        PlayerStatsManager.instance.isBinded = false;
    }

    IEnumerator ParalyzeCoroutine()
    {
        PlayerStatsManager.instance.isParalyzed = true;
        yield return new WaitForSeconds(paralyzeDuration);
        PlayerStatsManager.instance.isParalyzed = false;
    }
}
