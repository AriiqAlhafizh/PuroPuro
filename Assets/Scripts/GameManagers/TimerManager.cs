using System;
using UnityEngine;

public enum Difficulty
{
    Easy = 0,
    Normal = 1,
    Medium = 2,
    Hard = 3
}

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance { get; private set; }

    // Fired every frame when the timer updates. Provides the raw elapsed TimeSpan.
    public event Action<TimeSpan> OnTimeUpdated;
    // Fired every frame when the timer updates. Provides a formatted string (mm:ss.fff) suitable for UI display.
    public event Action<string> OnTimeFormattedUpdated;

    public bool useUnscaledTime = true;

    [Header("Difficulty progression (seconds)")]
    [SerializeField] private float secondsToNormal = 30f;
    [SerializeField] private float secondsToMedium = 90f;
    [SerializeField] private float secondsToHard = 180f;

    public bool autoDifficultyEnabled = true;
    public event Action<Difficulty> OnDifficultyChanged;

    // current difficulty state
    private Difficulty currentDifficulty = Difficulty.Easy;
    public Difficulty CurrentDifficulty => currentDifficulty;

    [Header("Runtime")]
    [SerializeField] private double elapsedSeconds = 0.0;
    private bool isRunning = false;

    // Public read-only accessors for UI or other systems
    public bool IsRunning => isRunning;
    public TimeSpan Elapsed => TimeSpan.FromSeconds(elapsedSeconds);

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
        // initialize difficulty based on initial elapsed time
        UpdateDifficulty(invokeEvent: true);
    }

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        // Use unscaled or scaled delta time depending on configuration
        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        elapsedSeconds += delta;

        // Notify listeners so UI Toolkit can update in real-time
        InvokeTimeEvents();
        // update difficulty based on new elapsed time
        UpdateDifficulty();
    }

    // Start or resume the stopwatch
    public void StartTimer()
    {
        isRunning = true;
        // Immediately push an update so UI reflects the current state right away
        InvokeTimeEvents();
    }

    // Pause the stopwatch (keeps elapsed time)
    public void PauseTimer()
    {
        isRunning = false;
        InvokeTimeEvents();
    }

    // Stop and reset the stopwatch
    public void ResetTimer(bool startAfterReset = false)
    {
        elapsedSeconds = 0.0;
        isRunning = startAfterReset;
        InvokeTimeEvents();
    }

    // Set the timer to an arbitrary time (in seconds)
    public void SetTime(double seconds, bool play = false)
    {
        elapsedSeconds = Math.Max(0.0, seconds);
        isRunning = play;
        InvokeTimeEvents();
        UpdateDifficulty();
    }

    // Convenience: formatted mm:ss.fff
    public static string FormatTime(double totalSeconds)
    {
        var ts = TimeSpan.FromSeconds(totalSeconds);
        // show total minutes (including hours) to avoid resetting minutes at 60
        int minutes = ts.Minutes + ts.Hours * 60;
        return string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, ts.Seconds, ts.Milliseconds);
    }

    private void InvokeTimeEvents()
    {
        var ts = Elapsed;
        OnTimeUpdated?.Invoke(ts);
        OnTimeFormattedUpdated?.Invoke(FormatTime(elapsedSeconds));
    }

    // Determine which difficulty should be active based on elapsedSeconds and thresholds.
    // If invokeEvent is true, OnDifficultyChanged will be invoked even if it's the initial setting.
    private void UpdateDifficulty(bool invokeEvent = false)
    {
        if (!autoDifficultyEnabled)
            return;

        Difficulty newDiff;
        double s = elapsedSeconds;
        if (s < Math.Max(0, secondsToNormal))
            newDiff = Difficulty.Easy;
        else if (s < Math.Max(secondsToNormal, secondsToMedium))
            newDiff = Difficulty.Normal;
        else if (s < Math.Max(secondsToMedium, secondsToHard))
            newDiff = Difficulty.Medium;
        else
            newDiff = Difficulty.Hard;

        CheckLevelTime(s);

        if (newDiff != currentDifficulty || invokeEvent)
        {
            bool changed = newDiff != currentDifficulty;
            currentDifficulty = newDiff;
            if (changed || invokeEvent)
                OnDifficultyChanged?.Invoke(currentDifficulty);
        }
    }

    private void CheckLevelTime(double s)
    {
        if (LevelManager.instance == null)
            return;

        if (s > LevelManager.instance.GetLevelTime(LevelManager.instance.currentLevel))
            LevelManager.instance.AdvanceLevel();
    }


    private void OnValidate()
    {
        secondsToNormal = Math.Max(0f, secondsToNormal);
        secondsToMedium = Math.Max(secondsToNormal, secondsToMedium);
        secondsToHard = Math.Max(secondsToMedium, secondsToHard);
    }

}
