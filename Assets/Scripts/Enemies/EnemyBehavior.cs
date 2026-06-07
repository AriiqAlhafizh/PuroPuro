using System;
using System.Collections;
using UnityEngine;


public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] protected EnemyStatsSO enemyStats;
    [SerializeField] protected float offset = 0.5f;

    protected int score = 50;

    protected float idleDuration;
    protected float walkDuration;
    protected float walkSpeed;

    protected Vector3 dir;
    protected Vector3 endPos;
    protected Camera cam;
    protected Animator animator;

    protected virtual void Start()
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

        dir = GetWalkDirection();

        StartCoroutine(SpawnPhase());
    }

    protected virtual IEnumerator SpawnPhase()
    {
        OnSpawn();
        yield return null;
        StartCoroutine(WalkPhase());
    }

    protected virtual IEnumerator WalkPhase()
    {
        OnWalkStart();
        yield return new WaitForSeconds(walkDuration);
        OnWalkEnd();

        StartCoroutine(IdlePhase());
    }

    protected virtual IEnumerator IdlePhase()
    {
        OnIdleEnter();
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(AttackPhase());
    }

    protected virtual IEnumerator AttackPhase()
    {
        OnAttack();
        yield return null;
        StartCoroutine(IdlePhase());
    }

    protected virtual Vector3 GetWalkDirection()
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

        Vector3 chosenDir;
        
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            chosenDir = new Vector3(Mathf.Sign(delta.x), 0f, 0f);
            endPos = new Vector3(targetPos.x, transform.position.y, transform.position.z);
        }
        else
        {
            chosenDir = new Vector3(0f, 0f, Mathf.Sign(delta.z));
            endPos = new Vector3(transform.position.x, transform.position.y, targetPos.z);
        }

        float distance = Vector3.Distance(transform.position, endPos);
        if (walkDuration > 0f)
        {
            walkSpeed = distance / walkDuration;
        }

        return chosenDir;
    }

    protected virtual void OnSpawn() { }

    protected virtual void OnWalkStart() { }

    protected virtual void OnWalkEnd() { }
    protected virtual void OnIdleEnter() { }

    protected virtual void OnAttack()
    {
       
    }

    protected virtual void AttackLand()
    {
        
    }

    public virtual void OnBeingHit() {
        StopAllCoroutines();
        StartCoroutine(DeathSaquence());
    }
    protected virtual void OnDeath() 
    {
        
    }

    protected virtual IEnumerator DeathSaquence()
    {
        OnDeath();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }


}
