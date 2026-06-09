using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    public List<AudioClip> shootClips;
    public AudioClip reloadClip;
    public AudioClip ricochetClip;
    public AudioSource audioSource;

    private void Start()
    {
        AudioManager.instance.OnSFXVolumeChange += GetSFXVolume;
    }

    private void OnDisable()
    {
        AudioManager.instance.OnSFXVolumeChange -= GetSFXVolume;
    }

    private void GetSFXVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootClips[Random.Range(0, shootClips.Count)]);
    }

    public void PlayReloadAudio()
    {
        audioSource.PlayOneShot(reloadClip);
    }

    public void PlayRicochetAudio()
    {
        audioSource.PlayOneShot(ricochetClip);
    }
}
