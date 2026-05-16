using System.Collections;
using UnityEngine;

public class EnemyBehavior_Brawler : EnemyBehavior
{
    protected override IEnumerator SpawnPhase()
    {
        OnSpawn();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        StartCoroutine(IdleAfterSpawnPhase());
    }

    private IEnumerator IdleAfterSpawnPhase()
    {
        OnIdleEnter();
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(WalkPhase());
    }
    protected override IEnumerator WalkPhase()
    {
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

        StartCoroutine(AttackPhase());
    }
}
