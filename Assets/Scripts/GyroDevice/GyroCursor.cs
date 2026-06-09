using UnityEngine;
using UnityEngine.UI;

public class GyroCursor : MonoBehaviour
{
    public RectTransform crosshair;

    // AIM RANGE
    public float yawRange = 40f;
    public float rollRange = 25f;

    // SMOOTHING
    public float smoothing = 10f;

    Vector2 currentPos;

    void Start()
    {
        currentPos =
            new Vector2(
                Screen.width / 2,
                Screen.height / 2
            );

        crosshair.position = currentPos;
    }

    void Update()
    {
        // NORMALIZE YAW
        float normalizedX =
            Mathf.Clamp(
                UDPManager.instance.yaw / yawRange,
                -1f,
                1f
            );

        // NORMALIZE ROLL
        float normalizedY =
            Mathf.Clamp(
                UDPManager.instance.roll / rollRange,
                -1f,
                1f
            );

        // CONVERT TO SCREEN SPACE
        float targetX =
            ((normalizedX + 1f) / 2f)
            * Screen.width;

        float targetY =
            ((normalizedY + 1f) / 2f)
            * Screen.height;

        // SMOOTH MOVEMENT
        currentPos =
            Vector2.Lerp(
                currentPos,
                new Vector2(targetX, targetY),
                Time.deltaTime * smoothing
            );

        // APPLY TO CROSSHAIR
        crosshair.position = currentPos;
    }
}