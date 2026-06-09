using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;
    
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake()
    {
        // Singleton pattern: keeps this object alive across scene loads
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Automatically fade in when the game starts
        StartCoroutine(FadeIn());
    }

    // Call this method from buttons or triggers to change scenes
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    public IEnumerator TransitionToScene(int sceneNumber)
    {
        yield return FadeOutAndLoad(sceneNumber);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = 1f;

        currentLevel = LevelManager.instance.currentLevel;
        levelText.text = $"Level {currentLevel}";

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = 0f;

        currentLevel = LevelManager.instance.currentLevel;
        levelText.text = $"Level {currentLevel}";

        // Fade Out (Screen turns solid color)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Load the new scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade In (Screen returns to clear view)
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOutAndLoad(int sceneNumber)
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = 0f;

        currentLevel = LevelManager.instance.currentLevel;
        levelText.text = $"Level {currentLevel}";

        // Fade Out (Screen turns solid color)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Load the new scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);

        // Wait until the scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade In (Screen returns to clear view)
        yield return StartCoroutine(FadeIn());
    }
}
