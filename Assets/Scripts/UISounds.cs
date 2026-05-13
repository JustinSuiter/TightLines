using UnityEngine;

public class UISounds : MonoBehaviour
{
    public static UISounds Instance;

    public AudioSource source;
    public AudioClip clickSound;

    void Awake()
    {
        // Singleton — only one instance ever exists, even across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicates if scene reloads
        }
    }

    public void PlayClick()
    {
        if (clickSound != null && source != null)
            source.PlayOneShot(clickSound);
    }
}