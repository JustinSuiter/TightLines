using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public GameObject bobber;
    public float castDistance = 8f;
    public float minWait = 3f;
    public float maxWait = 10f;
    public float biteWindow = 1.5f;
    private PlayerInventory inventory;
    private HUDManager hud;

    private enum State { Idle, Casting, Biting }
    private State currentState = State.Idle;
    private float timer;

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
            if (timer <= 0f)
                Bite();

            if (Input.GetKeyDown(KeyCode.F))
                CancelCast();
        }
        else if (currentState == State.Biting)
        {
            timer -= Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                Catch();
                return;
            }

            if (timer <= 0f)
                Missed();
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
        timer = Random.Range(minWait, maxWait);
        hud.SetStatus("your message here")
    }

    void Bite()
    {
        currentState = State.Biting;
        timer = biteWindow;
        hud.SetStatus("your message here")
    }

    void Catch()
    {
        currentState = State.Idle;
        bobber.SetActive(false);
        hud.SetStatus("your message here")
        inventory.AddFish(1)
    }

    void Missed()
    {
        currentState = State.Idle;
        bobber.SetActive(false);
        hud.SetStatus("your message here");
    }

    void CancelCast()
    {
        currentState = State.Idle;
        bobber.SetActive(false);
        hud.SetStatus("your message here");
    }
}