using TMPro;
using UnityEngine;

public class WinScore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public PlayerStatsManager PlayerStats;
    public TextMeshProUGUI AccuracyValue;
    public TextMeshProUGUI HealthValue;
    public TextMeshProUGUI HitShotValue;
    public TextMeshProUGUI TotalShotValue;
    public TextMeshProUGUI ScoreValue;   
    public TextMeshProUGUI TotalScoreValue;


    void Update()
    {
       AccuracyValue.text = ScoreManager.instance.accuracy.ToString();
       HealthValue.text = PlayerStatsManager.instance.health.ToString();
       HitShotValue.text = ScoreManager.instance.hitShot.ToString();
       TotalShotValue.text = ScoreManager.instance.totalShot.ToString();
       TotalScoreValue.text = ScoreManager.instance.baseScore.ToString();
       ScoreValue.text = ScoreManager.instance.totalScore.ToString();
    }
    
}
