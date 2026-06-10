using System.Collections;
using UnityEngine;

public class EnemyBehavior_Mummy : EnemyBehavior
{
    [SerializeField] private float heightOffset = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Transform rootTransform;
    protected override void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning("EnemyBehavior: enemyStats is not assigned.");
            return;
        }

        animator = GetComponent<Animator>();

        cam = Camera.main;

        rootTransform = (transform.parent != null) ? transform.parent : transform;

        idleDuration = enemyStats.IdleDuration;
        walkDuration = enemyStats.WalkDuration;

        rootTransform.position = GetWalkDirection();

        StartCoroutine(SpawnPhase());
    }

    protected override IEnumerator SpawnPhase()
    {
        OnSpawn();
        float dropDuration = 0.5f; 
        float elapsed = 0f;
        startPos = rootTransform.position;
        targetPos = new Vector3(rootTransform.position.x, cam.transform.position.y - heightOffset, rootTransform.position.z);

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            rootTransform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        rootTransform.position = targetPos;

        StartCoroutine(IdlePhase());
    }

    protected override IEnumerator AttackPhase()
    {
        OnAttack();
        yield return new WaitForSeconds(0.2f);
        AttackLand();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        StartCoroutine(LeaveSaquence());
    }

    private IEnumerator LeaveSaquence()
    {
        float dropDuration = 0.5f;
        float elapsed = 0f;
        startPos = rootTransform.position;
        targetPos = startPos + new Vector3(0f, cam.transform.position.y + heightOffset, 0f);

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            rootTransform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        OnDeath();
    }
    protected override IEnumerator DeathSaquence()
    {
        float knockDuration = 0.6f;
        float elapsed = 0f;
        Vector3 start = rootTransform.position;

        Vector3 dir;
        dir = (start - cam.transform.position).normalized;

        if (dir.sqrMagnitude < 0.0001f)
            dir = transform.forward;

        float distance = Mathf.Max(1f, heightOffset * 3f);
        Vector3 target = start + dir * distance + Vector3.up * (heightOffset * 0.5f);

        while (elapsed < knockDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / knockDuration);
            // ease-out using sine
            float easeOut = Mathf.Sin(t * (Mathf.PI * 0.5f));
            rootTransform.position = Vector3.Lerp(start, target, easeOut);
            yield return null;
        }

        rootTransform.position = target;

        OnDeath();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        StopAllCoroutines();
        Destroy(gameObject);
    }
    protected override Vector3 GetWalkDirection()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return Vector3.zero;

        Vector3 targetPos = cam.transform.position;
        Vector3 delta = targetPos - rootTransform.position;

        Vector3 lookDir = new Vector3(delta.x, 0f, delta.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            rootTransform.rotation = Quaternion.LookRotation(lookDir);
        }


        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            offset = (delta.x > 0) ? -offset : offset;
            endPos = new Vector3(targetPos.x + offset, targetPos.y + heightOffset, rootTransform.position.z);
        }
        else
        {
            offset = (delta.z > 0) ? -offset : offset;
            endPos = new Vector3(rootTransform.position.x, targetPos.y + heightOffset, targetPos.z + offset);
        }

        return endPos;
    }

    protected override void OnAttack()
    {
        base.OnAttack();
        animator.Rebind();
        animator.SetTrigger("Attack");
    }
    protected override void AttackLand()
    {
        PlayerStatsManager.instance.ApplyBind();
    }
}
