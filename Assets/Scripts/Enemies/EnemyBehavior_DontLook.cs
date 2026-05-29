using System;
using System.Collections;
using UnityEngine;

public class EnemyBehavior_DontLook : MonoBehaviour
{
    private Lane currentLane = Lane.Down;
    private bool isLooking = false;

    void Start()
    {
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
        yield return new WaitForSeconds(2f);
        Debug.Log("EyeBat attacked the player.");
    }
}
