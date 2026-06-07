using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    private CanvasGroup canvasGroup;
    private Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scenes
            canvasGroup = GetComponent<CanvasGroup>();
            fadeImage = GetComponent<Image>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeToBlack(float duration)
    {
        StartCoroutine(FadeRoutine(1, duration));
    }

    public void FadeToClear(float duration)
    {
        StartCoroutine(FadeRoutine(0, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
