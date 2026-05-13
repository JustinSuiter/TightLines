using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Audio Clips")]
    public AudioClip splashSound;
    public AudioClip reelSound;
    public AudioClip catchSuccessSound;
    public AudioClip fishEscapeSound;
    public AudioClip uiClickSound;
    public AudioClip oceanAmbient;
    public AudioClip backgroundMusic;

    void Awake()
    {
        // Simple singleton pattern so AudioManager.Instance works anywhere
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        PlayMusic();
        PlayAmbient();
    }

    // ── PUBLIC API — call these from other scripts ─────────

    public void PlaySplash() => PlaySFX(splashSound);
    public void PlayReel() => PlaySFX(reelSound);
    public void PlayCatchSuccess() => PlaySFX(catchSuccessSound);
    public void PlayFishEscape() => PlaySFX(fishEscapeSound);
    public void PlayUIClick() => PlaySFX(uiClickSound);

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    void PlayMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void PlayAmbient()
    {
        if (oceanAmbient != null)
        {
            ambientSource.clip = oceanAmbient;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }
}