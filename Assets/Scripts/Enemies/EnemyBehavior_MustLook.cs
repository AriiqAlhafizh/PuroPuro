using System.Collections;
using UnityEngine;

public class EnemyBehavior_MustLook : EnemyBehavior
{
    private Lane currentLane = Lane.Down;

    [Header("Animation Controller")]
    [SerializeField] protected RuntimeAnimatorController spawnController;
    [SerializeField] protected RuntimeAnimatorController idleController;
    [SerializeField] protected RuntimeAnimatorController deathController;

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

        PlayerStatsManager.instance.ApplyParalyze();
        PlayerStatsManager.instance.OnLaneChange += OnPlayerLaneChange;

        StartCoroutine(SpawnPhase());
    }
    protected override IEnumerator SpawnPhase()
    {
        OnSpawn();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        OnIdleEnter();
    }
    protected override void OnSpawn()
    {
        ApplyController(spawnController);
    }

    protected override void OnIdleEnter()
    {
        ApplyController(idleController);
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
            if (delta.x > 0)
            {
                offset = -offset;
                currentLane = Lane.Left;
            }
            else
            {
                currentLane = Lane.Right;
            }
            endPos = new Vector3(targetPos.x + offset, transform.position.y, transform.position.z);
        }
        else
        {
            if (delta.z > 0)
            {
                offset = -offset;
                currentLane = Lane.Down;
            }
            else
            {
                currentLane = Lane.Up;
            }
            endPos = new Vector3(transform.position.x, transform.position.y, targetPos.z + offset);
        }

        return endPos;
    }

    void OnDestroy()
    {
        PlayerStatsManager.instance.OnLaneChange -= OnPlayerLaneChange;
    }

    private void OnPlayerLaneChange(Lane lane)
    {
        if (currentLane == lane)
        {
            PlayerStatsManager.instance.RemoveParalyze();
            StartCoroutine(DeathPhase());
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private void ApplyController(RuntimeAnimatorController controller)
    {
        if (animator == null || controller == null)
            return;

        if (animator.runtimeAnimatorController != controller)
        {
            animator.runtimeAnimatorController = controller;
        }
    }

    private IEnumerator DeathPhase()
    {
        OnDeath();
        yield return null;
    }

    protected override void OnDeath()
    {
        animator.Rebind();
        ApplyController(deathController);
    }

}
