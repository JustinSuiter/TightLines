using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int goldValue;
    public float spawnChance;
    public float catchDifficulty;
    public Sprite fishImage;
}