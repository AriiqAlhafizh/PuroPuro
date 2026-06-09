using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip attackClip;
    public AudioClip spawnClip;
    public AudioClip idleClip;
    public AudioClip dieClip;

    [Header("References")]
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

    public void PlayAttackAudio()
    {
        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);
    }

    public void PlaySpawnAudio()
    {
        if (spawnClip != null)
            audioSource.PlayOneShot(spawnClip);
    }

    public void PlayIdleAudio()
    {
        if (idleClip != null)
            audioSource.PlayOneShot(idleClip);
    }

    public void PlayDieAudio()
    {
        if (dieClip != null)
            audioSource.PlayOneShot(dieClip);
    }
}
