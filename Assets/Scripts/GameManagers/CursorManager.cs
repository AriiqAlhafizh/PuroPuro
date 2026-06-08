using System.Collections;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [Header("References")]
    [SerializeField] private Texture2D[] cursorSprites; // 9 sprites for rotation

    private bool isSpinning = false;

    private void Start()
    {
        SetCustomCursor(0);
    }

    private void SetCustomCursor(int index)
    {
        if (cursorSprites != null && cursorSprites.Length > 0)
        {
            int safeIndex = Mathf.Clamp(index, 0, cursorSprites.Length - 1);
            Texture2D tex = cursorSprites[safeIndex];
            Vector2 hotspot = new Vector2(tex.width / 2f, tex.height / 2f);
            Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
        }
    }

    public void ReloadCursor(float duration)
    {
        if (!isSpinning)
            StartCoroutine(SpinCursorCoroutine(duration));
    }

    private IEnumerator SpinCursorCoroutine(float duration)
    {
        isSpinning = true;
        float elapsed = 0f;
        int spriteCount = cursorSprites.Length;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Calculate which sprite to show (cycles through all sprites evenly)
            int spriteIndex = Mathf.FloorToInt(t * spriteCount) % spriteCount;
            SetCustomCursor(spriteIndex);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to the first sprite (or your preferred default)
        SetCustomCursor(0);
        isSpinning = false;
    }
}
