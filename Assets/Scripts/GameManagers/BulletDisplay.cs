using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletDisplay : MonoBehaviour
{
    public int bullet;
    public int maxBullet;

    public Sprite emptyBullet;
    public Sprite fullBullet;
    public Image[] bullets;

    public PlayerStatsManager playerAmmo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bullet = playerAmmo.currentBullets;
        maxBullet = playerAmmo.maxBullets;

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

            if (i < maxBullet)
            {
                bullets[i].enabled = true;
            }
            else
            {
                bullets[i].enabled = false;
            }
        }
    }
}
