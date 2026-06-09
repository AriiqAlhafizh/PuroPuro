using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] hearts;

    void Start()
    {
        OnHealthChange(PlayerStatsManager.instance.health);
        PlayerStatsManager.instance.OnHealthChange += OnHealthChange;
    }

    void OnDestroy()
    {
        PlayerStatsManager.instance.OnHealthChange -= OnHealthChange;
    }

    void OnHealthChange(int health)
    {
        Debug.Log("Health changed: " + health);
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
  
}
