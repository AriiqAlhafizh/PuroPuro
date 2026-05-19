using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GunAudio gunAudio;

    [Header("Settings")]
    public float reloadDuration = .5f;

    [Header("Debuffs")]
    public float bindDuration = 2f;
    public float paralyzeDuration = 2f;

    private void Start()
    {
        PlayerStatsManager.currentBullets = PlayerStatsManager.maxBullets;
    }
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed && 
            PlayerStatsManager.currentBullets > 0 
            && !PlayerStatsManager.isReloading 
            && !PlayerStatsManager.isParalyzed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                if(hit.collider.name == "Head" || hit.collider.name == "Body")
                {
                    EnemyBehavior enemy = hit.collider.GetComponentInParent<EnemyBehavior>();
                    if (enemy != null)
                    {
                        enemy.OnBeingHit();
                    }
                }
                Debug.Log("Hit: " + hit.collider.name);
            }

            gunAudio.PlayShootAudio();
            PlayerStatsManager.currentBullets--;
            Debug.Log("Bullets left: " + PlayerStatsManager.currentBullets);
        }
    }
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed 
            && !PlayerStatsManager.isBinded 
            && !PlayerStatsManager.isParalyzed)
        {
            gunAudio.PlayReloadAudio();
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        PlayerStatsManager.isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        PlayerStatsManager.currentBullets = PlayerStatsManager.maxBullets;
        PlayerStatsManager.isReloading = false;
        Debug.Log("Reloaded! Bullets: " + PlayerStatsManager.currentBullets);
    }

    IEnumerator BindCoroutine()
    {
        PlayerStatsManager.isBinded = true;
        yield return new WaitForSeconds(bindDuration);
        PlayerStatsManager.isBinded = false;
    }

    IEnumerator ParalyzeCoroutine()
    {
        PlayerStatsManager.isParalyzed = true;
        yield return new WaitForSeconds(paralyzeDuration);
        PlayerStatsManager.isParalyzed = false;
    }
}
