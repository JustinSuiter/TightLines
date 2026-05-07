using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;
    public TextMeshProUGUI valueLabel;

    [Header("Settings")]
    public float minSensitivity = 0.1f;
    public float maxSensitivity = 10f;
    public float defaultSensitivity = 5f;

    private const string PREF_KEY = "sensitivity";

    void Start()
    {
        slider.minValue = minSensitivity;
        slider.maxValue = maxSensitivity;

        float saved = PlayerPrefs.GetFloat(PREF_KEY, defaultSensitivity);
        slider.value = saved;
        UpdateLabel(saved);

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        UpdateLabel(value);
        PlayerPrefs.SetFloat(PREF_KEY, value);
        PlayerPrefs.Save();
    }

    void UpdateLabel(float value)
    {
        if (valueLabel != null)
            valueLabel.text = value.ToString("F1");
    }

    public float GetSensitivity()
    {
        return PlayerPrefs.GetFloat(PREF_KEY, defaultSensitivity);
    }
}