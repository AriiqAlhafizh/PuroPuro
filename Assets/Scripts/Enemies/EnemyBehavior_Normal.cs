using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Normal : EnemyBehavior
{
    protected override void OnSpawn() { ApplyController(spawnController); }

    protected override void OnWalkStart() { ApplyController(walkController); }

    protected override void OnWalkEnd() { } 
    protected override void OnIdleEnter() { ApplyController(idleController); }

    protected override void OnAttack()
    {
        ApplyController(attackController);
    }
}
