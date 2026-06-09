using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletDisplay : MonoBehaviour
{
    public int bullet;

    public Sprite emptyBullet;
    public Sprite fullBullet;
    public Image[] bullets;

    void Start()
    {

    }

    void Update()
    {
        bullet = PlayerStatsManager.instance.currentBullets;

        for (int i = 0; i < bullets.Length; i++)
        {
            if (i < bullet)
            {
                bullets[i].sprite = fullBullet;
            }
            else
            {
                bullets[i].sprite = emptyBullet;
            }
        }
    }
}
