using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI fishText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI statusText;

    private PlayerInventory inventory;
    private FishingManager fishing;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        fishing = FindFirstObjectByType<FishingManager>();
    }

    void Update()
    {
        fishText.text = "Fish: " + inventory.fishCount;
        goldText.text = "Gold: " + inventory.goldCount;
    }

    public void SetStatus(string message)
    {
        statusText.text = message;
    }
}