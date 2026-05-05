using System;
using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private EnemyStatsSO enemyStats;

    private float idleDuration = 1f;
    private float walkDuration = 2f;

    [SerializeField] private Transform target;
    private float walkDistance;

    private float walkSpeed;

    void Start()
    {
        idleDuration = enemyStats.IdleDuration;
        walkDuration = enemyStats.WalkDuration;

        if (target == null)
        {
            Debug.LogWarning("EnemyBehavior: target is not assigned.");
            return;
        }

        walkDistance = Vector3.Distance(transform.position, target.position);
        walkSpeed = walkDistance / walkDuration;

        StartCoroutine(WalkPhase());
    }

    private IEnumerator WalkPhase()
    {
        var endPosition = target.position;
        float elapsed = 0f;

        while (elapsed < walkDuration)
        {
            float step = walkSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, endPosition, step);

            elapsed += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(IdlePhase());
    }

    private IEnumerator IdlePhase()
    {
        Debug.Log("Enemy is Idling!");
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(AttackPhase());
    }

    private IEnumerator AttackPhase()
    {
        // anamation or attack logic here
        Debug.Log("Enemy is attacking!");
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(IdlePhase());
    }
}
