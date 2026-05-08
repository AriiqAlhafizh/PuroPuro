using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Normal : EnemyBehavior
{
    protected override void OnWalkStart() {
        
    }

    protected override void OnWalkEnd() { }

    protected override void OnIdleEnter() { 
        
        Debug.Log("Enemy is Idling!"); 
    
    }

    protected override void OnAttack()
    {
        
    }
}
