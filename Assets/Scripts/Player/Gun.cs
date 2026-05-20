using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GunAudio gunAudio;
    public UDPReceiver UDPReceiver;
    public GyroCursor gyroCursor;
    public bool isGyroEnabled = false;

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
        if (UDPReceiver.triggerPressed && !previousTriggerPressed)
        {
            Shoot();
        }
        previousTriggerPressed = UDPReceiver.triggerPressed;
    }
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
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
            Ray ray = isGyroEnabled
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
            Debug.Log("Bullets left: " + PlayerStatsManager.instance.currentBullets);
        }
    }
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed 
            && !PlayerStatsManager.instance.isBinded 
            && !PlayerStatsManager.instance.isParalyzed)
        {
            gunAudio.PlayReloadAudio();
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        PlayerStatsManager.instance.isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        PlayerStatsManager.instance.currentBullets = PlayerStatsManager.instance.maxBullets;
        PlayerStatsManager.instance.isReloading = false;
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
