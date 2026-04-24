using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject inventoryPanel;

    [Header("Tab Contents")]
    public GameObject fishContent;
    public GameObject itemsContent;
    public GameObject upgradesContent;

    [Header("Fish Display")]
    public Transform fishGridParent;    // The Content inside FishScrollView
    public GameObject fishSlotPrefab;

    [Header("Items Display")]
    public GameObject hornIcon;         // Simple Image for the horn

    private PlayerInventory inventory;
    private bool isOpen = false;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            RefreshFishGrid();
            RefreshItems();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ── TAB SWITCHING (wire these to buttons) ───────
    public void ShowFishTab()
    {
        fishContent.SetActive(true);
        itemsContent.SetActive(false);
        upgradesContent.SetActive(false);
    }

    public void ShowItemsTab()
    {
        fishContent.SetActive(false);
        itemsContent.SetActive(true);
        upgradesContent.SetActive(false);
    }

    public void ShowUpgradesTab()
    {
        fishContent.SetActive(false);
        itemsContent.SetActive(false);
        upgradesContent.SetActive(true);
    }

    // ── REFRESH DISPLAYS ────────────────────────────
    void RefreshFishGrid()
    {
        // Clear existing slots
        foreach (Transform child in fishGridParent)
            Destroy(child.gameObject);

        // Spawn a slot for each fish the player has
        foreach (PlayerInventory.FishEntry entry in inventory.caughtFish)
        {
            GameObject slot = Instantiate(fishSlotPrefab, fishGridParent);

            TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI t in texts)
            {
                if (t.name == "NameText") t.text = entry.fishData.fishName;
                if (t.name == "QuantityText") t.text = "x" + entry.quantity;
            }

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            if (entry.fishData.fishImage != null)
                icon.sprite = entry.fishData.fishImage;
        }
    }

    void RefreshItems()
    {
        if (hornIcon != null)
            hornIcon.SetActive(inventory.hasHorn);
    }
}