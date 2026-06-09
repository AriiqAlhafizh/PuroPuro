using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

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

    public float SFXVolume = 0.5f;
    public float BGMVolume = 0.1f;

    public Slider SFXslider;

    private void Start()
    {
        SFXslider.value = SFXVolume;
    }

    public event Action<float> OnSFXVolumeChange;
    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        OnSFXVolumeChange?.Invoke(SFXVolume);
    }

    public event Action<float> OnBGMVolumeChange;
    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume * 0.1f;
        OnBGMVolumeChange?.Invoke(BGMVolume);
    }
}
