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

    public List<FishEntry> caughtFish = new List<FishEntry>();
    public int goldCount = 0;
    public bool hasHorn = false;
    public int fishNeededForHorn = 5;

    public void AddFish(FishData fish)
    {
        // Check if we already have this type
        FishEntry existing = caughtFish.Find(e => e.fishData == fish);

        if (existing != null)
        {
            existing.quantity++;
        }
        else
        {
            caughtFish.Add(new FishEntry { fishData = fish, quantity = 1 });
        }

        Debug.Log("Caught a " + fish.fishName + "!");

        if (!hasHorn && TotalFishCount() >= fishNeededForHorn)
            UnlockHorn();
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
        foreach (FishEntry e in caughtFish)
            total += e.quantity;
        return total;
    }

    public void AddGold(int amount) { goldCount += amount; }

    public bool SpendGold(int amount)
    {
        if (goldCount >= amount) { goldCount -= amount; return true; }
        return false;
    }

    void UnlockHorn()
    {
        hasHorn = true;
        Debug.Log("Horn unlocked!");
    }
}