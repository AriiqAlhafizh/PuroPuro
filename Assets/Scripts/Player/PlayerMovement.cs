using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public enum Lane { Up, Right, Down, Left }
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject mainCamera;

    [Header("Settings")]
    public float spinDuration = 0.5f; // Duration in seconds for any spin
    public AnimationCurve spinCurve;

    public Lane currentLane = Lane.Up;
    readonly Dictionary<Lane, float> lane = new()
    {
        { Lane.Up, 0f },
        { Lane.Right, 90f },
        { Lane.Down, 180f },
        { Lane.Left, -90f }
    };
    
    public bool isSpinning = false;

    public void CameraMovement(InputAction.CallbackContext context)
    {
        if (context.performed && !isSpinning)
        {
            Vector2 input = context.ReadValue<Vector2>();
            int directionDelta = 0;

            if (input == Vector2.right)
                directionDelta = 1; // Turn right
            else if (input == Vector2.left)
                directionDelta = -1; // Turn left
            else if (input == Vector2.down)
                directionDelta = 2; // Turn around

            if (directionDelta != 0)
            {
                // Compute next lane with wrap-around, but don't set currentLane yet
                // — SpinCameraToLane will update currentLane when the spin finishes.
                int newLane = ((int)currentLane + directionDelta) % 4;
                if (newLane < 0) newLane += 4;
                Lane nextLane = (Lane)newLane;

                SpinCamera(nextLane);
            }
        }
    }

    public void SpinCamera(Lane targetLane)
    {
        if (!isSpinning && currentLane != targetLane)
        {
            StartCoroutine(SpinCameraToLane(targetLane));

        }
    }

    private IEnumerator SpinCameraToLane(Lane targetLane)
    {
        isSpinning = true;

        float duration = spinDuration;
        float elapsed = 0f;

        // Get the camera's current Y rotation as the start angle
        float startAngle = mainCamera.transform.eulerAngles.y;
        float endAngle = lane[targetLane];

        // Handle shortest rotation direction
        float angleDelta = Mathf.DeltaAngle(startAngle, endAngle);
        float finalEndAngle = startAngle + angleDelta;

        Quaternion startCamRot = Quaternion.Euler(0, startAngle, 0);
        Quaternion endCamRot = Quaternion.Euler(0, finalEndAngle, 0);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveT = spinCurve.Evaluate(t);

            mainCamera.transform.rotation = Quaternion.Slerp(startCamRot, endCamRot, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.rotation = endCamRot;
        currentLane = targetLane;
        PlayerStatsManager.instance.ChangeLane(currentLane);
        isSpinning = false;
    }
}
