using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject mainCamera;

    [Header("Settings")]
    public float spinDuration = 0.5f; // Duration in seconds for any spin
    public AnimationCurve spinCurve;

    private bool isSpinning = false;

    public void CameraMovement(InputAction.CallbackContext context)
    {
        if (context.performed && !isSpinning)
        {
            Vector2 input = context.ReadValue<Vector2>();
            float angle = 0f;

            if (input == Vector2.right)
                angle = 90f;
            else if (input == Vector2.left)
                angle = -90f;
            else if (input == Vector2.down)
                angle = 180f;

            if (angle != 0f)
                StartCoroutine(SpinCamera(angle));
        }
    }

    private IEnumerator SpinCamera(float angle)
    {
        isSpinning = true;

        float duration = spinDuration; // Use fixed duration
        float elapsed = 0f;

        Quaternion startCamRot = mainCamera.transform.rotation;
        Quaternion endCamRot = Quaternion.AngleAxis(angle, Vector3.up) * startCamRot;

        Quaternion startPlayerRot = player.transform.rotation;
        Quaternion endPlayerRot = Quaternion.AngleAxis(angle, Vector3.up) * startPlayerRot;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveT = spinCurve.Evaluate(t);

            mainCamera.transform.rotation = Quaternion.Slerp(startCamRot, endCamRot, curveT);
            player.transform.rotation = Quaternion.Slerp(startPlayerRot, endPlayerRot, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.rotation = endCamRot;
        player.transform.rotation = endPlayerRot;
        isSpinning = false;
    }
}
