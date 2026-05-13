using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndingManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject endingPanel;
    public Image blackFadeImage;            // The panel's background image
    public GameObject endingText;
    public GameObject subtitleText;
    public GameObject returnButton;
    public GameObject keepPlayingButton;

    [Header("Settings")]
    public float fadeDuration = 3f;
    public string mainMenuSceneName = "Main Menu";

    void Start()
    {
        endingPanel.SetActive(false);
    }

    public void TriggerEnding()
    {
        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        endingPanel.SetActive(true);
        endingText.SetActive(false);
        subtitleText.SetActive(false);
        returnButton.SetActive(false);
        keepPlayingButton.SetActive(false);

        // Fade to black
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            blackFadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Show text and buttons
        yield return new WaitForSeconds(0.5f);
        endingText.SetActive(true);
        yield return new WaitForSeconds(1f);
        subtitleText.SetActive(true);
        yield return new WaitForSeconds(1f);
        returnButton.SetActive(true);
        keepPlayingButton.SetActive(true);

        // Unlock cursor so they can click
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void KeepPlaying()
    {
        // Fade out and resume
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        endingText.SetActive(false);
        subtitleText.SetActive(false);
        returnButton.SetActive(false);
        keepPlayingButton.SetActive(false);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            blackFadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        endingPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}