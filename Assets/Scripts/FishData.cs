using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int goldValue;
    public float spawnChance;    // Higher = more common (Bluegill 0.6, Salmon 0.3, Tuna 0.1)
    public float catchDifficulty; // How hard the reeling minigame is (0-1)
    public Sprite fishImage;      // Your friends drop art in here later
}