using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip menuMusic;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.volume = PlayerPrefs.GetFloat("musicVolume", 0.75f);
        audioSource.Play();
    }

    public void UpdateVolume(float volume)
    {
        audioSource.volume = volume;
    }
}