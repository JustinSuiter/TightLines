using UnityEngine;
using System.Collections;

public class RodManager : MonoBehaviour
{
    [Header("Rod Models")]
    public GameObject[] rodModels;

    [Header("References")]
    public UpgradeShop upgradeShop;
    public LineRenderer fishingLine;
    public GameObject bobber;

    [Header("Animation")]
    public float castDuration = 0.6f;
    public float windUpAngle = -60f;   // How far back the rod tilts
    public float swingAngle = 30f;     // How far forward the rod swings

    private int lastSeenLevel = -1;
    private bool isAnimating = false;

    void Update()
    {
        if (upgradeShop != null && upgradeShop.rodLevel != lastSeenLevel)
        {
            SwapRod(upgradeShop.rodLevel);
            lastSeenLevel = upgradeShop.rodLevel;
        }

        // Update the fishing line position every frame
        UpdateFishingLine();
    }

    void SwapRod(int level)
    {
        for (int i = 0; i < rodModels.Length; i++)
            rodModels[i].SetActive(false);

        if (level >= 0 && level < rodModels.Length)
            rodModels[level].SetActive(true);
    }

    public Transform GetCurrentRodTip()
    {
        for (int i = 0; i < rodModels.Length; i++)
        {
            if (rodModels[i].activeSelf)
                return rodModels[i].transform.Find("RodTip");
        }
        return null;
    }

    // ── ANIMATION ────────────────────────────────────────

    public void PlayCastAnimation(System.Action onCastComplete)
    {
        if (!isAnimating)
            StartCoroutine(CastAnimationRoutine(onCastComplete));
    }

    IEnumerator CastAnimationRoutine(System.Action onCastComplete)
    {
        isAnimating = true;
        Transform activeRod = GetActiveRodTransform();
        if (activeRod == null) { isAnimating = false; yield break; }

        Quaternion startRot = activeRod.localRotation;
        Quaternion windUpRot = startRot * Quaternion.Euler(windUpAngle, 0, 0);
        Quaternion swingRot = startRot * Quaternion.Euler(swingAngle, 0, 0);

        // Wind up (rod tilts back)
        float t = 0f;
        while (t < castDuration * 0.4f)
        {
            t += Time.deltaTime;
            activeRod.localRotation = Quaternion.Slerp(startRot, windUpRot, t / (castDuration * 0.4f));
            yield return null;
        }

        // Swing forward
        t = 0f;
        while (t < castDuration * 0.3f)
        {
            t += Time.deltaTime;
            activeRod.localRotation = Quaternion.Slerp(windUpRot, swingRot, t / (castDuration * 0.3f));
            yield return null;
        }

        // Release — call back to FishingManager to spawn bobber
        onCastComplete?.Invoke();

        // Return to rest position
        t = 0f;
        while (t < castDuration * 0.3f)
        {
            t += Time.deltaTime;
            activeRod.localRotation = Quaternion.Slerp(swingRot, startRot, t / (castDuration * 0.3f));
            yield return null;
        }

        activeRod.localRotation = startRot;
        isAnimating = false;
    }

    Transform GetActiveRodTransform()
    {
        for (int i = 0; i < rodModels.Length; i++)
            if (rodModels[i].activeSelf) return rodModels[i].transform;
        return null;
    }

    // ── FISHING LINE ─────────────────────────────────────

    void UpdateFishingLine()
    {
        if (fishingLine == null || bobber == null) return;

        if (bobber.activeInHierarchy)
        {
            Transform tip = GetCurrentRodTip();
            if (tip != null)
            {
                fishingLine.gameObject.SetActive(true);
                fishingLine.SetPosition(0, tip.position);
                fishingLine.SetPosition(1, bobber.transform.position);
            }
        }
        else
        {
            fishingLine.gameObject.SetActive(false);
        }
    }
}