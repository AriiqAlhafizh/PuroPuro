using System;
using System.Collections;
using UnityEngine;

public class EnemyBehavior_DontLook : MonoBehaviour
{
    private Lane currentLane = Lane.Down;
    private bool isLooking = false;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        PlayerStatsManager.instance.OnLaneChange += OnPlayerLaneChange;
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
        float duration = 1f;
        float elapsed = 0f;

        if (mainCamera == null)
            mainCamera = Camera.main;

        while (elapsed < duration)
        {
            if (mainCamera == null)
                yield break;

            Vector3 direction = mainCamera.transform.position - transform.position;
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure exact facing at the end
        if (mainCamera != null)
            transform.LookAt(mainCamera.transform);

        Debug.Log("EyeBat attacked the player.");
    }
}
