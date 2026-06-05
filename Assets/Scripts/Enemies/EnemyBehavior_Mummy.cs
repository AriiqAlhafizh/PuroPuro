using System.Collections;
using UnityEngine;

public class EnemyBehavior_Mummy : EnemyBehavior
{
    [SerializeField] private float heightOffset = 1f;

    private Vector3 startPos;
    private Vector3 targetPos;
    protected override void Start()
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

        transform.position = GetWalkDirection();

        StartCoroutine(SpawnPhase());
    }

    protected override IEnumerator SpawnPhase()
    {
        OnSpawn();
        float dropDuration = 0.5f; 
        float elapsed = 0f;
        startPos = transform.position;
        targetPos = startPos - new Vector3(0f, heightOffset + (offset/2), 0f);

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.position = targetPos;

        StartCoroutine(IdlePhase());
    }

    protected override IEnumerator AttackPhase()
    {
        OnAttack();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        OnIdleEnter();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        StartCoroutine(LeaveSaquence());
    }

    private IEnumerator LeaveSaquence()
    {
        float dropDuration = 0.5f;
        float elapsed = 0f;
        startPos = transform.position;
        targetPos = startPos + new Vector3(0f, heightOffset + (offset/2), 0f);

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        OnDeath();
    }
    protected override IEnumerator DeathSaquence()
    {
        float knockDuration = 0.6f;
        float elapsed = 0f;
        Vector3 start = transform.position;

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
            transform.position = Vector3.Lerp(start, target, easeOut);
            yield return null;
        }

        transform.position = target;

        OnDeath();
    }

    protected override void OnDeath()
    {
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
        Vector3 delta = targetPos - transform.position;

        Vector3 lookDir = new Vector3(delta.x, 0f, delta.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }


        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            offset = (delta.x > 0) ? -offset : offset;
            endPos = new Vector3(targetPos.x + offset, targetPos.y + heightOffset, transform.position.z);
        }
        else
        {
            offset = (delta.z > 0) ? -offset : offset;
            endPos = new Vector3(transform.position.x, targetPos.y + heightOffset, targetPos.z + offset);
        }

        return endPos;
    }
    protected override void AttackLand()
    {
        PlayerStatsManager.instance.isBinded = true;
    }
}
