using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WhaleTrader : MonoBehaviour
{
    [Header("Trade UI")]
    public GameObject tradePanel;
    public Transform tradeSlotParent;       // The Content of the TradeScrollView
    public GameObject tradeSlotPrefab;

    [Header("Settings")]
    public float interactionDistance = 5f;
    public Transform player;

    private PlayerInventory inventory;
    private WhaleSummoner whaleSummoner;
    private HUDManager hud;
    private bool isOpen = false;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        whaleSummoner = FindFirstObjectByType<WhaleSummoner>();
        hud = FindFirstObjectByType<HUDManager>();
        tradePanel.SetActive(false);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Just handle auto-close when walking too far
        if (!whaleSummoner.IsWhalePresent()) return;
        float dist = Vector3.Distance(player.position, transform.position);
        if (isOpen && dist > interactionDistance + 2f)
            CloseTrade();
    }

    public void OpenTrade()
    {
        isOpen = true;
        tradePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RefreshTradeList();
    }

    public void CloseTrade()
    {
        isOpen = false;
        tradePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void RefreshTradeList()
    {
        // Clear old slots
        foreach (Transform child in tradeSlotParent)
            Destroy(child.gameObject);

        // Make a slot for each fish type the player owns
        foreach (PlayerInventory.FishEntry entry in inventory.caughtFish)
        {
            if (entry.quantity <= 0) continue;

            GameObject slot = Instantiate(tradeSlotPrefab, tradeSlotParent);

            // Set info text
            TextMeshProUGUI infoText = slot.transform.Find("InfoText")?.GetComponent<TextMeshProUGUI>();
            if (infoText != null)
                infoText.text = entry.fishData.fishName + " x" + entry.quantity + "\n" + entry.fishData.goldValue + "g each";

            // Set icon if available
            Transform iconTransform = slot.transform.Find("Icon");
            if (iconTransform != null && entry.fishData.fishImage != null)
                iconTransform.GetComponent<Image>().sprite = entry.fishData.fishImage;

            // Wire the Sell button
            Button sellBtn = slot.transform.Find("SellButton")?.GetComponent<Button>();
            if (sellBtn != null)
            {
                FishData fishToSell = entry.fishData; // Capture for closure
                sellBtn.onClick.AddListener(() => SellOne(fishToSell));
            }
        }
    }

    void SellOne(FishData fish)
    {
        if (inventory.SpendFish(fish, 1))
        {
            inventory.AddGold(fish.goldValue);
            Debug.Log("Sold a " + fish.fishName + " for " + fish.goldValue + "g");
            RefreshTradeList(); // Refresh display
        }
    }
}