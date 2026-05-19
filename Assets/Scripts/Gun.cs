using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GunAudio gunAudio;

    [Header("Settings")]
    public int maxBullets = 6;
    public int currentBullets;

    public bool isReloading = false;
    public float reloadDuration = .5f;

    [Header("Debuffs")]
    public float bindDuration = 2f;
    public bool isBinded = false; // cant reload
    public float paralyzeDuration = 2f;
    public bool isParalyzed = false; // cant shoot or reload

    private void Start()
    {
        currentBullets = maxBullets;
    }
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed && currentBullets > 0 && !isReloading && !isParalyzed)
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
            currentBullets--;
            //Debug.Log("Bullets left: " + currentBullets);
        }
    }
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed && !isBinded && !isParalyzed)
        {
            gunAudio.PlayReloadAudio();
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        currentBullets = maxBullets;
        isReloading = false;
        //Debug.Log("Reloaded! Bullets: " + currentBullets);
    }

    IEnumerator BindCoroutine()
    {
        isBinded = true;
        yield return new WaitForSeconds(bindDuration);
        isBinded = false;
    }

    IEnumerator ParalyzeCoroutine()
    {
        isParalyzed = true;
        yield return new WaitForSeconds(paralyzeDuration);
        isParalyzed = false;
    }
}
