using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip gameBGM;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        bgmSource.clip = gameBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void SetPaused(bool paused)
    {
        if(paused)
            sfxSource.Pause();
        else 
            sfxSource.UnPause();
    }

    public AudioSource GetBGMSource()
    {
        return bgmSource;
    }

    public AudioSource GetSFXSource()
    {
        return sfxSource;
    }
}
