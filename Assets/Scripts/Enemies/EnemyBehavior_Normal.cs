using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Normal : EnemyBehavior
{
    protected override void OnAttack()
    {
        Debug.Log("EnemyBehavior_Normal: OnAttack called.");
    }
}
