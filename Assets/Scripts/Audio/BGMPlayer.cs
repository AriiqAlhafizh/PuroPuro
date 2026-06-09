using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer instance;
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
    public AudioSource audioSource;
    public AudioClip BGM;

    private void Start()
    {
        AudioManager.instance.OnBGMVolumeChange += GetBGMVolume;
        StartBGM();
    }

    private void OnDisable()
    {
        AudioManager.instance.OnBGMVolumeChange -= GetBGMVolume;
    }

    private void GetBGMVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public void StartBGM()
    {
        audioSource.volume = AudioManager.instance.BGMVolume;
        audioSource.clip = BGM;
        audioSource.Play();
    }
}
