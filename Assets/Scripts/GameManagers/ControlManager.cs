using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControlManager : MonoBehaviour
{
    public static ControlManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetControlType();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("ControlType")]
    public bool isGyroEnabled = false;

    [Header("References")]
    public GameObject gyroCursorPrefab;

    public void ChangeControlType(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeControlType();
        }
    }
    public void ChangeControlType()
    {
        isGyroEnabled = !isGyroEnabled;
        SetControlType();
    }

    public void SetControlType()
    {
        if (!isGyroEnabled)
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gyroCursorPrefab.SetActive(false);
    }
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gyroCursorPrefab.SetActive(true);
    }
}
