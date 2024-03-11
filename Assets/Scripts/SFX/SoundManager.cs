using UnityEngine;

public class SoundManager : MonoBehaviour       
{
    public static SoundManager INSTANCE;

    [SerializeField] private bool isDisabled;
    [SerializeField] private SoundBase soundBase;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
    }

    private void Update()
    {
        return; // todo later
        if (!musicSource.isPlaying)
        {
            PlayMusic();
        }
    }

    static internal void PlayMusic()
    {
        if (INSTANCE.isDisabled) return;

        AudioClip clip = INSTANCE.GetSound("Music");
        if (clip != null)
        {
            INSTANCE.musicSource.PlayOneShot(clip);
        }
    }

    static internal void PlaySound(string clipType)
    {
        if (INSTANCE.isDisabled) return;
        AudioClip clip = INSTANCE.GetSound(clipType);
        if (clip != null)
        {
            INSTANCE.Play(clip);
        }
    }

    static internal void StopMusic()
    {
        INSTANCE.musicSource.Stop();
        INSTANCE.musicSource.Play();
    }

    private void Play(AudioClip clip)
    {   
        sfxSource.PlayOneShot(clip);
    }

    internal void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    internal void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    private AudioClip GetSound(string soundName) 
    {
        AudioClip clip = soundBase.GetAudioClip(soundName);
        if (clip != null) return clip;

        clip = soundBase.GetAudioClip(soundName);
        if (clip != null) return clip;

    //    Debug.LogWarning("no sound for key " + soundName);      
        // throw new System.Exception("no sound for key " + soundName);
        return null;
    }
}
