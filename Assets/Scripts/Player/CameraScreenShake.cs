using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    [Header("Camera Reference")]
    public GameObject cam;

    [Header("Shake Settings")]
    public AnimationCurve shakeCurve;
    public float duration = 1f;

    public void TriggerShake()
    {
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = cam.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Get strength from curve (0 to 1 range)
            float strength = shakeCurve.Evaluate(elapsedTime / duration);

            // Apply random offset scaled by strength
            cam.transform.position = startPosition + Random.insideUnitSphere * strength;

            yield return null;
        }

        transform.position = startPosition; // Reset position
    }
}