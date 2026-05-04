using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class FishEntry
    {
        public FishData fishData;
        public int quantity;
    }

    [System.Serializable]
    public class ItemEntry
    {
        public ItemData itemData;
        public int quantity;
    }

    public List<FishEntry> caughtFish = new List<FishEntry>();
    public List<ItemEntry> ownedItems = new List<ItemEntry>();
    public ItemData hornItemData;
    public int goldCount = 0;
    public bool hasHorn = false;
    public bool hornCaught = false;
    public int fishNeededForHorn = 5;

    public void AddFish(FishData fish)
    {
        FishEntry existing = caughtFish.Find(e => e.fishData == fish);
        if (existing != null) existing.quantity++;
        else caughtFish.Add(new FishEntry { fishData = fish, quantity = 1 });

        Debug.Log("Caught a " + fish.fishName + "!");
    }

    public bool ShouldCatchHorn()
    {
        // Returns true if total fish >= 5 and we haven't caught the horn yet
        return !hornCaught && TotalFishCount() >= fishNeededForHorn;
    }

    public void CatchHorn()
    {
        hornCaught = true;
        hasHorn = true;
        AddItem(hornItemData);
        Debug.Log("Mysterious Horn caught!");
    }

    public bool SpendFish(FishData fish, int amount)
    {
        FishEntry existing = caughtFish.Find(e => e.fishData == fish);
        if (existing != null && existing.quantity >= amount)
        {
            existing.quantity -= amount;
            return true;
        }
        return false;
    }

    public int TotalFishCount()
    {
        int total = 0;
        foreach (FishEntry e in caughtFish) total += e.quantity;
        return total;
    }

    public void AddGold(int amount) { goldCount += amount; }

    public bool SpendGold(int amount)
    {
        if (goldCount >= amount) { goldCount -= amount; return true; }
        return false;
    }

        public void AddItem(ItemData item)
    {
        ItemEntry existing = ownedItems.Find(e => e.itemData == item);
        if (existing != null) existing.quantity++;
        else ownedItems.Add(new ItemEntry { itemData = item, quantity = 1 });
    }
}