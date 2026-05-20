using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Normal : EnemyBehavior
{
    [Header("Normal Enemy Stats List")]
    [SerializeField] protected List<EnemyStatsSO> EnemyStats;

    protected override void Start()
    {
        int randomIndex = Random.Range(0, EnemyStats.Count);
        if (randomIndex >= EnemyStats.Count)
        {
            Debug.LogWarning("EnemyBehavior: randomIndex is out of bounds.");
            return;
        }

        animator = GetComponent<Animator>();

        cam = Camera.main;

        idleDuration = EnemyStats[randomIndex].IdleDuration;
        walkDuration = EnemyStats[randomIndex].WalkDuration;

        dir = GetWalkDirection();

        StartCoroutine(SpawnPhase());
    }
}
