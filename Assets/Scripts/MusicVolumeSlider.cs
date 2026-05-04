using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicVolumeSlider : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;
    public TextMeshProUGUI valueLabel;
    public MainMenuMusic musicManager;

    private const string PREF_KEY = "musicVolume";

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;

        float saved = PlayerPrefs.GetFloat(PREF_KEY, 0.75f);
        slider.value = saved;
        UpdateLabel(saved);

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        UpdateLabel(value);
        PlayerPrefs.SetFloat(PREF_KEY, value);
        PlayerPrefs.Save();

        if (musicManager != null)
            musicManager.UpdateVolume(value);
    }

    void UpdateLabel(float value)
    {
        if (valueLabel != null)
            valueLabel.text = Mathf.RoundToInt(value * 100) + "%";
    }
}