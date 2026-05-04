using UnityEngine;

public class MusicPersist : MonoBehaviour
{
    static MusicPersist instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}