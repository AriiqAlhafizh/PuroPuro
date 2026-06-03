using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Normal : EnemyBehavior
{
    [Header("Animation Controller")]
    [SerializeField] protected RuntimeAnimatorController spawnController;
    [SerializeField] protected RuntimeAnimatorController walkController;
    [SerializeField] protected RuntimeAnimatorController attackController;
    [SerializeField] protected RuntimeAnimatorController idleController;
    [SerializeField] protected RuntimeAnimatorController deathController;

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

        StartCoroutine(IdlePhase());
    }

    protected override IEnumerator AttackPhase()
    {
        OnAttack();
        animator.Rebind();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        OnIdleEnter();
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(IdlePhase());
    }

    protected void ApplyController(RuntimeAnimatorController controller)
    {
        if (animator == null || controller == null)
            return;

        if (animator.runtimeAnimatorController != controller)
        {
            animator.runtimeAnimatorController = controller;
        }
    }

    protected override void OnSpawn() { }

    protected override void OnWalkStart() { ApplyController(walkController); }

    protected override void OnWalkEnd() { }
    protected override void OnIdleEnter() { ApplyController(idleController); }

    protected override void OnAttack()
    {
        ApplyController(attackController);
    }

    protected override void AttackLand()
    {
        PlayerStatsManager.instance.TakeDamage();
    }

    protected override void OnDeath()
    {
        StopAllCoroutines();
        ApplyController(deathController);
        Destroy(gameObject, 0.5f);
    }
}
