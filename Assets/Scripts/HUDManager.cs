using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI statusText;

    private PlayerInventory inventory;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
    }

    void Update()
    {
        goldText.text = "Gold: " + inventory.goldCount;
    }

    public void SetStatus(string message)
    {
        statusText.text = message;
    }
}