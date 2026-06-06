using UnityEngine;

using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI bulletText;
    public TextMeshProUGUI HealthText;
    public PlayerStatsManager playerStats;

    void Update()
    {
        bulletText.text = $"{playerStats.currentBullets}/{playerStats.maxBullets}";
        HealthText.text = playerStats.health.ToString();
    }
}
