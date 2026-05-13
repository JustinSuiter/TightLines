using UnityEngine;
using TMPro;

public class UpgradeShop : MonoBehaviour
{
    [Header("References")]
    public FishingManager fishingManager;
    public GameObject shopPanel;
    public TextMeshProUGUI goldText;

    [Header("Upgrade Costs (base)")]
    public int rodBaseCost = 10;
    public int reelBaseCost = 15;
    public int baitBaseCost = 20;

    [Header("Upgrade State")]
    public int rodLevel = 0;
    public int reelLevel = 0;
    public int baitLevel = 0;
    public int maxLevel = 3;

    [Header("Interaction")]
    public Transform player;
    public float interactionDistance = 3f;
    public HUDManager hud;

    [Header("Row Texts")]
    public TextMeshProUGUI rodInfoText;
    public TextMeshProUGUI reelInfoText;
    public TextMeshProUGUI baitInfoText;

    private PlayerInventory inventory;
    private bool shopOpen = false;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        hud = FindFirstObjectByType<HUDManager>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        shopPanel.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        if (shopOpen && dist > interactionDistance + 2f)
            CloseShop();
    }

    public void OpenShop()
    {
        shopOpen = true;
        shopPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RefreshShop();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ── BUTTON ACTIONS ──────────────────────────────

    public void TryUpgradeRod()
    {
        if (rodLevel >= maxLevel) return;
        int cost = GetCost(rodBaseCost, rodLevel);
        if (inventory.SpendGold(cost))
        {
            rodLevel++;
            fishingManager.reelSpeedBonus += 1.5f; // Reduces wait time
            RefreshShop();
            CheckForEnding();
        }
    }

    public void TryUpgradeReel()
    {
        if (reelLevel >= maxLevel) return;
        int cost = GetCost(reelBaseCost, reelLevel);
        if (inventory.SpendGold(cost))
        {
            reelLevel++;
            fishingManager.catchBonus += 0.4f; // Bigger bite window
            RefreshShop();
            CheckForEnding();
        }
    }

    public void TryUpgradeBait()
    {
        if (baitLevel >= maxLevel) return;
        int cost = GetCost(baitBaseCost, baitLevel);
        if (inventory.SpendGold(cost))
        {
            baitLevel++;
            // Bait level affects fish rarity — we'll wire this into FishingManager
            fishingManager.rarityBonus += 0.1f;
            RefreshShop();
            CheckForEnding();
        }
    }

    int GetCost(int baseCost, int currentLevel)
    {
        // Each level costs more — base cost * (level + 1)
        return baseCost * (currentLevel + 1);
    }

    void RefreshShop()
    {
        goldText.text = "Gold: " + inventory.goldCount;

        string rodCost = rodLevel >= maxLevel ? "MAX" : GetCost(rodBaseCost, rodLevel) + "g";
        string reelCost = reelLevel >= maxLevel ? "MAX" : GetCost(reelBaseCost, reelLevel) + "g";
        string baitCost = baitLevel >= maxLevel ? "MAX" : GetCost(baitBaseCost, baitLevel) + "g";

        rodInfoText.text = $"Rod    Lv {rodLevel}/{maxLevel}    {rodCost}";
        reelInfoText.text = $"Reel   Lv {reelLevel}/{maxLevel}    {reelCost}";
        baitInfoText.text = $"Bait   Lv {baitLevel}/{maxLevel}    {baitCost}";
    }

        void CheckForEnding()
    {
        if (rodLevel >= maxLevel && reelLevel >= maxLevel && baitLevel >= maxLevel)
        {
            EndingManager ending = FindFirstObjectByType<EndingManager>();
            if (ending != null)
            {
                CloseShop(); // Close shop first
                ending.TriggerEnding();
            }
        }
    }
}