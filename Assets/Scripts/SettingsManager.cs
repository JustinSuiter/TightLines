using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject settingsPanel;

    [Header("Sliders")]
    public Slider sensitivitySlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Value Texts")]
    public TextMeshProUGUI sensitivityValueText;
    public TextMeshProUGUI masterVolumeValueText;
    public TextMeshProUGUI musicVolumeValueText;
    public TextMeshProUGUI sfxVolumeValueText;

    [Header("Toggle")]
    public Toggle fullscreenToggle;

    [Header("Audio Mixer (optional)")]
    public AudioMixer audioMixer;

    void Start()
    {
        LoadSettings();

        // Wire up slider changes
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        SaveSettings();
    }

    // ── Setting change handlers ──────────────────────────

    void OnSensitivityChanged(float value)
    {
        sensitivityValueText.text = value.ToString("F1");
        ApplySensitivity(value);
    }

    void OnMasterVolumeChanged(float value)
    {
        masterVolumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
        ApplyVolume("MasterVolume", value);
    }

    void OnMusicVolumeChanged(float value)
    {
        musicVolumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
        ApplyVolume("MusicVolume", value);
    }

    void OnSFXVolumeChanged(float value)
    {
        sfxVolumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
        ApplyVolume("SFXVolume", value);
    }

    void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // ── Applying ──────────────────────────────────────────

    void ApplySensitivity(float value)
    {
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) pc.mouseSensitivity = value;
    }

    void ApplyVolume(string mixerParam, float value)
    {
        if (audioMixer != null)
        {
            // Convert 0-1 to decibels (-80 to 0)
            float db = value > 0.001f ? Mathf.Log10(value) * 20 : -80f;
            audioMixer.SetFloat(mixerParam, db);
        }
        else
        {
            // No mixer set up yet — fall back to AudioListener for master
            if (mixerParam == "MasterVolume")
                AudioListener.volume = value;
        }
    }

    // ── Save/Load ────────────────────────────────────────

    void SaveSettings()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 2f);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        // Apply on load
        OnSensitivityChanged(sensitivitySlider.value);
        OnMasterVolumeChanged(masterVolumeSlider.value);
        OnMusicVolumeChanged(musicVolumeSlider.value);
        OnSFXVolumeChanged(sfxVolumeSlider.value);
        OnFullscreenChanged(fullscreenToggle.isOn);
    }
}