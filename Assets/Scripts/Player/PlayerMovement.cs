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
                // Update currentDirection with wrap-around
                int newDir = ((int)currentDirection + directionDelta) % 4;
                if (newDir < 0) newDir += 4;
                Direction nextDirection = (Direction)newDir;
                currentDirection = nextDirection;

                SpinCamera(nextDirection);
            }
        }
    }

    public void SpinCamera(Direction targetDirection)
    {
        if (!isSpinning && currentDirection != targetDirection)
        {
            StartCoroutine(SpinCameraToDirection(targetDirection));
        }
    }

    private IEnumerator SpinCameraToDirection(Direction targetDirection)
    {
        isSpinning = true;

        float duration = spinDuration;
        float elapsed = 0f;

        // Get the camera's current Y rotation as the start angle
        float startAngle = mainCamera.transform.eulerAngles.y;
        float endAngle = direction[targetDirection];

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
        currentDirection = targetDirection;
        isSpinning = false;
    }
}
