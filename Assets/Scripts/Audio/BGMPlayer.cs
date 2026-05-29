using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip BGM;

    private void Start()
    {
        StartBGM();
    }
    public void StartBGM()
    {
        audioSource.volume = AudioManager.instance.BGMVolume;
        audioSource.clip = BGM;
        audioSource.Play();
    }
}
