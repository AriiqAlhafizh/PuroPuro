using UnityEngine;

public class EnemyBehavior_MustLook : MonoBehaviour
{
    private Lane currentLane = Lane.Down;

    void Start()
    {
        PlayerStatsManager.instance.isParalyzed = true;
        PlayerStatsManager.instance.OnLaneChange += OnPlayerLaneChange;
    }

    void OnDestroy()
    {
        PlayerStatsManager.instance.OnLaneChange -= OnPlayerLaneChange;
    }

    private void OnPlayerLaneChange(Lane lane)
    {
        if (currentLane == lane)
        {
            PlayerStatsManager.instance.isParalyzed = false;
            Destroy(gameObject);
        }
    }

}
