using System.Collections;
using UnityEngine;

public class EnemyBehavior_Ranger : EnemyBehavior_Normal
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    protected override IEnumerator WalkPhase()
    {
        offset = Vector3.Distance(transform.position, endPos) / 2f;
        float elapsed = 0f;
        OnWalkStart();

        while (elapsed < walkDuration)
        {
            float distance = Vector3.Distance(transform.position, endPos);

            if (distance < offset)
            {
                break;
            }

            float step = walkSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
            {
                transform.position += dir.normalized * step;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        OnWalkEnd();

        StartCoroutine(IdlePhase());
    }

    protected override void AttackLand()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Ranger_Bullet bulletScript = bullet.GetComponent<Ranger_Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(dir, endPos);
        }
    }
}
