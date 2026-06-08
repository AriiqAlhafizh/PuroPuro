using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBehavior_DontLook : EnemyBehavior
{
    private Lane currentLane = Lane.Down;
    private bool isLooking = false;

    [SerializeField] private float heightOffset = 1f;
    [SerializeField] private float lookScaleMultiplier = 2f;
    [SerializeField] private float lookScaleDuration = 1f;
    [SerializeField] private float lungeDistance = 2f;
    [SerializeField] private float lungeDuration = 0.35f;
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

        PlayerStatsManager.instance.OnLaneChange += OnPlayerLaneChange;

        StartCoroutine(SpawnPhase());
    }
    protected override IEnumerator SpawnPhase()
    {
        OnSpawn();
        yield return null;
    }
    protected override void OnSpawn() {  
        
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
            if(delta.x > 0 || delta.x < 0 && PlayerStatsManager.instance.currentLane == Lane.Right)
            {
                offset = -offset;
                currentLane = Lane.Left;
            }
            else if (delta.x < 0 || delta.x > 0 && PlayerStatsManager.instance.currentLane == Lane.Left)
            {
                currentLane = Lane.Right;
            }

            endPos = new Vector3(targetPos.x + offset, transform.position.y + heightOffset, transform.position.z);
        }
        else
        {
            if (delta.z > 0 || delta.z < 0 && PlayerStatsManager.instance.currentLane == Lane.Up)
            {
                offset = -offset;
                currentLane = Lane.Down;
            }
            else if (delta.z < 0 || delta.z > 0 && PlayerStatsManager.instance.currentLane == Lane.Down)
            {
                currentLane = Lane.Up;
            }

            endPos = new Vector3(transform.position.x, transform.position.y + heightOffset, targetPos.z + offset);
        }

        return endPos;
    }

    void OnDestroy()
    {
        PlayerStatsManager.instance.OnLaneChange -= OnPlayerLaneChange;
    }

    private void OnPlayerLaneChange(Lane lane)
    {
        if(currentLane == lane && !isLooking)
        {
            isLooking = true;
            StartCoroutine(startLooking());
        }

        if(currentLane != lane && isLooking)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator startLooking()
    {
        float duration = lookScaleDuration; 
        float elapsed = 0f;

        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * lookScaleMultiplier;

        while (elapsed < duration)
        {
            Vector3 direction = cam.transform.position - transform.position;
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, ease);

            yield return null;
        }

        if (cam != null)
            transform.LookAt(cam.transform);
        transform.localScale = targetScale;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * lungeDistance;
        elapsed = 0f;

        while (elapsed < lungeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lungeDuration);
            // ease-out
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            transform.position = Vector3.Lerp(startPos, endPos, easeOut);
            yield return null;
        }

        transform.position = endPos;

        PlayerStatsManager.instance.TakeDamage();
        DeathSaquence();
    }
}
