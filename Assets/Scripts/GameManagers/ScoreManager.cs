using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int baseScore;
    public int totalScore;
    public int hitShot = 0;
    public int totalShot = 0;
    public float accuracy;


    public void IncreaseScore(int newScore)
    {
        baseScore += newScore;
    }
    public void IncreaseHitShot()
    {
        hitShot++;
        CalculateAccuracy();
    }
    public void IncreaseTotalShot()
    {
        totalShot++;
        CalculateAccuracy();
    }
    public void CalculateAccuracy()
    {
        accuracy = (float)hitShot / totalShot;
    }
    public void CalculateTotalScore()
    {

    }
}
