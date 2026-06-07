using UnityEngine;

public class EnemyBehavior_Knight : EnemyBehavior_Normal
{
    [Header("Knight Settings")]
    [SerializeField] private bool isShielded = true;

    protected override void OnWalkEnd() 
    {
        isShielded = false;
    }

    public override void OnBeingHit()
    {
        if(isShielded)
        {
            Debug.Log("Knight's shield absorbed the hit!");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(DeathSaquence());
        }
    }
}
