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

    public int currentLevel = 0;
    public Lane currentLane = Lane.Up;

    public List<Lane> allowedDirections = new List<Lane>();
    public Dictionary<Lane, float> directionAngles = new Dictionary<Lane, float>();

    public bool isSpinning = false;

    private void Start()
    {
        // Initialize allowed directions and their corresponding angles based on the current level
        allowedDirections = LevelManager.instance.GetAllowedLanes(currentLevel);
        directionAngles = LevelManager.instance.GetAllowedAngles(currentLevel);
    }

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
                int newDir = (allowedDirections.IndexOf(currentLane) + directionDelta) % allowedDirections.Count;
                if (newDir < 0) newDir += allowedDirections.Count;
                Lane nextDirection = allowedDirections[newDir];
                currentLane = nextDirection;

                SpinCamera(nextDirection, directionDelta);
            }
        }
    }

    public void SpinCamera(Lane targetLane)
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinCameraToDirection(targetLane, 0));
        }
    }

    public void SpinCamera(Lane targetLane, int directionDelta)
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinCameraToDirection(targetLane, directionDelta));
        }
    }
    private IEnumerator SpinCameraToDirection(Lane targetLane, int directionDelta)
    {
        isSpinning = true;

        float duration = spinDuration;
        float elapsed = 0f;

        // Get the camera's current Y rotation as the start angle
        float startAngle = mainCamera.transform.eulerAngles.y + directionDelta;
        float endAngle = directionAngles[targetLane];

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
