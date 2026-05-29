using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject mainCamera;

    [Header("Settings")]
    public float spinDuration = 0.5f; // Duration in seconds for any spin
    public AnimationCurve spinCurve;

    public enum Direction { Up, Right, Down, Left }
    public Direction currentDirection = Direction.Up;
    readonly Dictionary<Direction, float> direction = new()
    {
        { Direction.Up, 0f },
        { Direction.Right, 90f },
        { Direction.Down, 180f },
        { Direction.Left, -90f }
    };
    
    private bool isSpinning = false;

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
                // Update currentDirection with wrap-around
                int newDir = ((int)currentDirection + directionDelta) % 4;
                if (newDir < 0) newDir += 4;
                currentDirection = (Direction)newDir;

                float angle = direction[(Direction)(((int)Direction.Up + directionDelta + 4) % 4)];
                StartCoroutine(SpinCamera(angle));
            }
        }
    }

    private IEnumerator SpinCamera(float angle)
    {
        isSpinning = true;

        float duration = spinDuration; // Use fixed duration
        float elapsed = 0f;

        Quaternion startCamRot = mainCamera.transform.rotation;
        Quaternion endCamRot = Quaternion.AngleAxis(angle, Vector3.up) * startCamRot;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveT = spinCurve.Evaluate(t);

            mainCamera.transform.rotation = Quaternion.Slerp(startCamRot, endCamRot, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.rotation = endCamRot;
        isSpinning = false;
    }
}
