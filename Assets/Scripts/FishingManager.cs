using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public GameObject bobber;
    public float castDistance = 8f;
    public float minWait = 3f;
    public float maxWait = 10f;
    public float biteWindow = 1.5f;
    public FishData[] fishTypes;

    private PlayerInventory inventory;
    private HUDManager hud;
    private FishData currentFish;

    private enum State { Idle, Casting, Biting }
    private State currentState = State.Idle;
    private float timer;

    [Header("Upgrade Bonuses")]
    public float reelSpeedBonus = 0f;
    public float catchBonus = 0f;
    public float rarityBonus = 0f;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        hud = FindFirstObjectByType<HUDManager>();
        hud.SetStatus("Press F to cast!");
    }

    void Update()
    {
        if (currentState == State.Idle)
        {
            if (Input.GetKeyDown(KeyCode.F))
                Cast();
        }
        else if (currentState == State.Casting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f) Bite();
            if (Input.GetKeyDown(KeyCode.F)) CancelCast();
        }
        else if (currentState == State.Biting)
        {
            timer -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) { Catch(); return; }
            if (timer <= 0f) Missed();
        }
    }

    void Cast()
    {
        currentState = State.Casting;
        Camera cam = Camera.main;
        Vector3 pos = cam.transform.position + cam.transform.forward * castDistance;
        pos.y = 0.1f;
        bobber.transform.position = pos;
        bobber.SetActive(true);
        timer = Mathf.Max(1f, Random.Range(minWait, maxWait) - reelSpeedBonus);
        hud.SetStatus("Waiting for a bite...");
    }

    void Bite()
    {
        currentState = State.Biting;
        timer = biteWindow + catchBonus;

        // If player has caught 5 fish, the next catch is the horn!
        if (inventory.ShouldCatchHorn())
        {
            currentFish = null; // Special signal that this is the horn
            hud.SetStatus("Something HUGE is biting! CLICK!");
        }
        else
        {
            currentFish = PickRandomFish();
            hud.SetStatus("Something's biting! CLICK!");
        }
    }

    void Catch()
    {
        currentState = State.Idle;
        bobber.SetActive(false);

        if (currentFish == null)
        {
            // It's the horn!
            inventory.CatchHorn();
            hud.SetStatus("You caught a Mysterious Horn! Check your inventory.");
        }
        else
        {
            inventory.AddFish(currentFish);
            hud.SetStatus("You caught a " + currentFish.fishName + "! Press F to cast again.");
        }
    }

    void Missed()
    {
        currentState = State.Idle;
        bobber.SetActive(false);
        hud.SetStatus("The fish got away... Press F to try again.");
    }

    void CancelCast()
    {
        currentState = State.Idle;
        bobber.SetActive(false);
        hud.SetStatus("Press F to cast!");
    }

    FishData PickRandomFish()
    {
        float total = 0f;
        foreach (FishData f in fishTypes) total += f.spawnChance;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (FishData f in fishTypes)
        {
            cumulative += f.spawnChance;
            if (roll <= cumulative) return f;
        }

        return fishTypes[0];
    }
}