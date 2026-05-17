using System.Collections;
using UnityEngine;

public class EnemyBehavior_Ranger : EnemyBehavior
{
    protected override IEnumerator WalkPhase()
    {
        offset = Vector3.Distance(transform.position, endPos) / 2f;
        float elapsed = 0f;
        OnWalkStart();

        while (elapsed < walkDuration)
        {
            float distance = Vector3.Distance(transform.position, endPos);

            if (distance < offset)
            {
                break;
            }

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

    protected override void AttackLand()
    {
        int rand = Random.Range(0, 100);
        if (rand < 50) 
        {
            Debug.Log("Ranger attack hit the player!");
            PlayerStatsManager.instance.TakeDamage();
        }
        else 
        {
            Debug.Log("Ranger attack missed the player!");
        }
    }
}
