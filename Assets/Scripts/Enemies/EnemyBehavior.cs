using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    [SerializeField] protected EnemyStatsSO enemyStats;
    [SerializeField] protected float offset = 0.5f;

    [Header("Animation Controller")]
    [SerializeField] protected RuntimeAnimatorController spawnController;
    [SerializeField] protected RuntimeAnimatorController walkController;
    [SerializeField] protected RuntimeAnimatorController attackController;
    [SerializeField] protected RuntimeAnimatorController idleController;
    [SerializeField] protected RuntimeAnimatorController deathController;

    protected float idleDuration;
    protected float walkDuration;
    protected float walkSpeed;

    protected Vector3 dir;
    protected Camera cam;
    protected Animator animator;

    protected virtual void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning("EnemyBehavior: enemyStats is not assigned.");
            return;
        }

        animator = GetComponent<Animator>();

        cam = Camera.main;

        idleDuration = enemyStats.IdleDuration;
        walkDuration = enemyStats.WalkDuration;

        dir = GetWalkDirection();

        StartCoroutine(SpawnPhase());
    }

    protected virtual IEnumerator SpawnPhase()
    {
        OnSpawn();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        StartCoroutine(WalkPhase());
    }

    protected virtual IEnumerator WalkPhase()
    {
        float elapsed = 0f;
        OnWalkStart();

        while (elapsed < walkDuration - offset)
        {
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

    protected virtual IEnumerator IdlePhase()
    {
        OnIdleEnter();
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(AttackPhase());
    }

    protected virtual IEnumerator AttackPhase()
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


    protected virtual Vector3 GetWalkDirection()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return Vector3.zero;

        Vector3 targetPos = cam.transform.position;
        Vector3 delta = targetPos - transform.position;

        Vector3 lookDir = new Vector3(delta.x, 0f, delta.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        Vector3 chosenDir;
        Vector3 endPos;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            chosenDir = new Vector3(Mathf.Sign(delta.x), 0f, 0f);
            endPos = new Vector3(targetPos.x, transform.position.y, transform.position.z);
        }
        else
        {
            chosenDir = new Vector3(0f, 0f, Mathf.Sign(delta.z));
            endPos = new Vector3(transform.position.x, transform.position.y, targetPos.z);
        }

        float distance = Vector3.Distance(transform.position, endPos);
        if (walkDuration > 0f)
        {
            walkSpeed = distance / walkDuration;
        }

        return chosenDir;
    }

    protected virtual void OnSpawn() { }

    protected virtual void OnWalkStart() { }

    protected virtual void OnWalkEnd() { }

    protected virtual void OnIdleEnter() { }

    protected virtual void OnAttack() { }

    protected virtual void AttackLand()
    {
        PlayerStatsManager.instance.TakeDamage();
    }
}
