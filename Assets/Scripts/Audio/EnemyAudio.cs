using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip attackClip;
    public AudioClip spawnClip;
    public AudioClip dieClip;

    [Header("References")]
    public AudioSource audioSource;

    public void PlayAttackAudio()
    {
        audioSource.PlayOneShot(attackClip);
    }

    public void PlaySpawnAudio()
    {
        audioSource.PlayOneShot(spawnClip);
    }

    public void PlayDieAudio()
    {
        audioSource.PlayOneShot(dieClip);
    }
}
