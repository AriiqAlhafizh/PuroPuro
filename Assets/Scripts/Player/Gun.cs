using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GunAudio gunAudio;
    public GyroCursor gyroCursor;
    public GameObject hitEffectPrefab;

    [Header("Settings")]
    public float reloadDuration = .5f;

    private void Start()
    {
        PlayerStatsManager.instance.currentBullets = PlayerStatsManager.instance.maxBullets;
    }

    private bool previousTriggerPressed = false;

    private void Update()
    {
        // Detect rising edge: triggerPressed goes from false to true
        if (UDPManager.instance.triggerPressed && !previousTriggerPressed)
        {
            Shoot();
        }
        previousTriggerPressed = UDPManager.instance.triggerPressed;

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
            && !PlayerStatsManager.instance.cantReloadnShoot)
        {
            Ray ray = ControlManager.instance.isGyroEnabled
            ? Camera.main.ScreenPointToRay(gyroCursor.crosshair.position)
            : Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    HitboxType enemyHB = hit.collider.GetComponent<HitboxType>();
                    if (enemyHB.hitbox == Hitbox.Body) // hit body
                    {
                        ScoreManager.instance.IncreaseScore(50);
                        EnemyBehavior enemy = hit.collider.GetComponentInParent<EnemyBehavior>();
                        enemy.OnBeingHit();
                        enemyHB.OnHit();
                        ScoreManager.instance.IncreaseHitShot();
                    }
                    else if (enemyHB.hitbox == Hitbox.Head) // hit head
                    {
                        ScoreManager.instance.IncreaseScore(100);
                        EnemyBehavior enemy = hit.collider.GetComponentInParent<EnemyBehavior>();
                        enemy.OnBeingHit();
                        enemyHB.OnHit();
                        ScoreManager.instance.IncreaseHitShot();
                    }
                    else if (enemyHB.hitbox == Hitbox.Shield) // hit Shield
                    {
                        gunAudio.PlayRicochetAudio();
                    }

                    HitEffect(hit);


                    //Debug.Log("Hit: " + hit.collider.name + " " + type);


                }
                ScoreManager.instance.IncreaseTotalShot();
            }

            gunAudio.PlayShootAudio();
            PlayerStatsManager.instance.currentBullets--;
            if (PlayerStatsManager.instance.currentBullets <= 0)
            {
                StartCoroutine(ReloadCoroutine());
            }

            UDPManager.instance.SendPlayerData(PlayerStatsManager.instance.currentBullets, PlayerStatsManager.instance.health);

            // Debug
            //Debug.Log("Bullets left: " + PlayerStatsManager.instance.currentBullets);
        }
    }

    public void HitEffect(RaycastHit hit)
    {
        // Get the screen position of the hit point
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(hit.point);
        // Get the center of the screen
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        // Direction vector from center to hit point
        Vector2 dir = (Vector2)screenPoint - screenCenter;

        // Calculate angle in degrees (0 = right, 90 = up, 180 = left, -90 = down)
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Add 90 so 0 points "up" from the hit point (optional, depending on your effect's default orientation)
        baseAngle += 90f;

        // Add randomization within ±15 degrees
        float randomOffset = Random.Range(-15f, 15f);
        float finalZ = baseAngle + randomOffset;

        Quaternion randomRotation = Quaternion.Euler(0, 0, finalZ);
        GameObject effect = Instantiate(hitEffectPrefab, hit.point, randomRotation);
        Destroy(effect, 0.2f);
    }
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed
            && !PlayerStatsManager.instance.isReloading
            && !PlayerStatsManager.instance.cantReload
            && !PlayerStatsManager.instance.cantReloadnShoot
            && PlayerStatsManager.instance.currentBullets < PlayerStatsManager.instance.maxBullets - 1)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        CursorManager.instance.ReloadCursor(reloadDuration);
        gunAudio.PlayReloadAudio();
        PlayerStatsManager.instance.isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        PlayerStatsManager.instance.currentBullets = PlayerStatsManager.instance.maxBullets;
        PlayerStatsManager.instance.isReloading = false;
        UDPManager.instance.SendPlayerData(PlayerStatsManager.instance.currentBullets, PlayerStatsManager.instance.health);
        Debug.Log("Reloaded! Bullets: " + PlayerStatsManager.instance.currentBullets);
    }
}
