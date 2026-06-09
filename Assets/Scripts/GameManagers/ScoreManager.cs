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

    [SerializeField] private int baseScore;
    [SerializeField] public int combo;
    [SerializeField] private int totalScore;
    [SerializeField] private int hitShot = 0;
    [SerializeField] private int totalShot = 0;
    [SerializeField] private float accuracy;


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
