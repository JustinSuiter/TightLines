using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public WaveOcean ocean;
    public GameObject bobber;
    public float castDistance = 8f;
    public float minWait = 3f;
    public float maxWait = 10f;
    public float biteWindow = 1.5f;
    public FishData[] fishTypes;
    public ReelingMinigame reelingMinigame;
    public RodManager rodManager;

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
        reelingMinigame.onCatchSuccess = OnMinigameSuccess;
        reelingMinigame.onCatchFail = OnMinigameFail;
        inventory = FindFirstObjectByType<PlayerInventory>();
        hud = FindFirstObjectByType<HUDManager>();
        hud.SetStatus("Press F to cast!");
    }

    void Update()
    {
        // Block all input while minigame is active
        if (reelingMinigame != null && reelingMinigame.IsActive)
            return;

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
        hud.SetStatus("Casting...");

        // Play the cast animation, spawn bobber when it "releases"
        rodManager.PlayCastAnimation(SpawnBobber);
    }

    void SpawnBobber()
    {
        Camera cam = Camera.main;
        Vector3 pos = cam.transform.position + cam.transform.forward * castDistance;
        bobber.transform.position = pos;
        bobber.SetActive(true);
        AudioManager.Instance.PlaySplash();

        timer = Mathf.Max(1f, Random.Range(minWait, maxWait) - reelSpeedBonus);
        hud.SetStatus("Waiting for a bite...");
    }

    void Bite()
    {
        currentState = State.Biting;

        if (inventory.ShouldCatchHorn())
        {
            currentFish = null;
            // Horn doesn't use minigame — instant catch on click
            timer = biteWindow;
            hud.SetStatus("Something HUGE is biting! CLICK!");
        }
        else
        {
            currentFish = PickRandomFish();
            // Start the minigame instead of waiting for click
            reelingMinigame.StartMinigame(currentFish);
            currentState = State.Idle; // Minigame takes over now
            hud.SetStatus("Reel it in!");
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
        // Boost spawn chances based on bait level
        // Rare fish (low spawn chance) get a bigger relative boost
        float total = 0f;
        foreach (FishData f in fishTypes)
        {
            float effective = f.spawnChance + (1f - f.spawnChance) * rarityBonus;
            total += effective;
        }

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (FishData f in fishTypes)
        {
            float effective = f.spawnChance + (1f - f.spawnChance) * rarityBonus;
            cumulative += effective;
            if (roll <= cumulative) return f;
        }

        return fishTypes[0];
    }

        void OnMinigameSuccess()
    {
        AudioManager.Instance.PlayCatchSuccess();
        bobber.SetActive(false);
        inventory.AddFish(currentFish);
        hud.SetStatus("You caught a " + currentFish.fishName + "! Press F to cast again.");
    }

    void OnMinigameFail()
    {
        AudioManager.Instance.PlayFishEscape();
        bobber.SetActive(false);
        hud.SetStatus("The fish got away... Press F to try again.");
    }

        void LateUpdate()
    {
        // Make the bobber float on the waves while it's cast
        if (bobber.activeInHierarchy && ocean != null)
        {
            Vector3 pos = bobber.transform.position;
            float waveY = ocean.GetWaveHeight(pos.x, pos.z, Time.time);
            bobber.transform.position = new Vector3(pos.x, waveY + 0.05f, pos.z);
        }
    }
}