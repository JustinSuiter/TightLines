using UnityEngine;
using UnityEngine.UI;

public class ReelingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public RectTransform track;
    public RectTransform indicator;
    public RectTransform catchZone;
    public Image progressBar;

    [Header("Difficulty")]
    public float indicatorSpeed = 200f;
    public float pullForce = 400f;
    public float gravityForce = 250f;
    public float catchZoneWidth = 80f;
    public float catchSpeed = 0.5f;
    public float escapeSpeed = 0.3f;

    [Header("Fail Condition")]
    public float maxTimeOutOfZone = 4f;

    public bool IsActive => isActive;

    private float indicatorPos = 0f;
    private float indicatorVelocity = 0f;
    private float catchZonePos = 0f;
    private float catchZoneTargetPos = 0f;
    private float progress = 0f;
    private float catchZoneWanderSpeed = 0.5f;
    private float patience = 0f;
    private bool isActive = false;

    public System.Action onCatchSuccess;
    public System.Action onCatchFail;

    public void StartMinigame(FishData fish)
    {
        panel.SetActive(true);
        isActive = true;
        progress = 0f;
        indicatorPos = -1f;
        indicatorVelocity = 0f;

        float difficulty = fish.catchDifficulty; // 0 = easy, 1 = hard

        // Smaller catch zone for harder fish
        catchZoneWidth = Mathf.Lerp(130f, 80f, difficulty);
        catchZone.sizeDelta = new Vector2(catchZoneWidth, catchZone.sizeDelta.y);

        // Harder fish thrash around faster
        pullForce = Mathf.Lerp(550f, 700f, difficulty);
        gravityForce = Mathf.Lerp(350f, 450f, difficulty);

        // Catch zone wanders faster for harder fish
        catchZoneWanderSpeed = Mathf.Lerp(0.5f, 0.9f, difficulty);

        // Progress fills slower, drains faster on harder fish
        catchSpeed = Mathf.Lerp(0.5f, 0.35f, difficulty);
        escapeSpeed = Mathf.Lerp(0.25f, 0.4f, difficulty);

        // Less patience on harder fish
        patience = Mathf.Lerp(maxTimeOutOfZone, maxTimeOutOfZone * 0.75f, difficulty);

        catchZoneTargetPos = Random.Range(-0.7f, 0.7f);
    }

    void Update()
    {
        if (!isActive) return;

        UpdateIndicator();
        UpdateCatchZone();
        UpdateProgress();
        UpdateVisuals();
    }

    void UpdateIndicator()
    {
        if (Input.GetMouseButton(0))
            indicatorVelocity += pullForce * Time.deltaTime;
        else
            indicatorVelocity -= gravityForce * Time.deltaTime;

        indicatorPos += indicatorVelocity * Time.deltaTime * 0.008f;
        indicatorPos = Mathf.Clamp(indicatorPos, -1f, 1f);

        if (indicatorPos == -1f || indicatorPos == 1f)
            indicatorVelocity = 0f;
    }

    void UpdateCatchZone()
    {
        catchZonePos = Mathf.MoveTowards(catchZonePos, catchZoneTargetPos, Time.deltaTime * catchZoneWanderSpeed);
        if (Mathf.Abs(catchZonePos - catchZoneTargetPos) < 0.05f)
            catchZoneTargetPos = Random.Range(-0.7f, 0.7f);
    }

    void UpdateProgress()
    {
        float zoneHalfWidth = (catchZoneWidth / 2f) / (track.rect.width / 2f);
        bool inZone = Mathf.Abs(indicatorPos - catchZonePos) < zoneHalfWidth;

        if (inZone)
        {
            progress += catchSpeed * Time.deltaTime;
            patience = Mathf.Min(patience + Time.deltaTime * 0.5f, maxTimeOutOfZone);
        }
        else
        {
            progress -= escapeSpeed * Time.deltaTime;
            patience -= Time.deltaTime;
        }

        progress = Mathf.Clamp01(progress);

        if (progress >= 1f)
            EndMinigame(true);
        else if (patience <= 0f)
            EndMinigame(false);
    }

    void UpdateVisuals()
    {
        float trackWidth = track.rect.width / 2f;
        indicator.anchoredPosition = new Vector2(indicatorPos * trackWidth, 0);
        catchZone.anchoredPosition = new Vector2(catchZonePos * trackWidth, 0);
        progressBar.fillAmount = progress;
    }

    void EndMinigame(bool success)
    {
        isActive = false;
        panel.SetActive(false);

        if (success) onCatchSuccess?.Invoke();
        else onCatchFail?.Invoke();
    }

    public void CancelMinigame()
    {
        if (isActive) EndMinigame(false);
    }
}