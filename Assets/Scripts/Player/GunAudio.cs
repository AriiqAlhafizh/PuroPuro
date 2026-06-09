using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    public List<AudioClip> shootClips;
    public AudioClip reloadClip;
    public AudioSource audioSource;

    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootClips[Random.Range(0, shootClips.Count)]);
    }

    public void PlayReloadAudio()
    {
        audioSource.PlayOneShot(reloadClip);
    }
}
