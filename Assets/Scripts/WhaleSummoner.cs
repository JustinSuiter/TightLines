using UnityEngine;
using UnityEngine.Playables;

public class WhaleSummoner : MonoBehaviour
{
    public PlayableDirector whaleTimeline;
    public GameObject whaleObject;
    public Camera playerCamera;
    public Camera cinematicCamera;

    private PlayerInventory inventory;
    private HUDManager hud;
    private bool whaleSummoned = false;
    private bool whaleArrived = false;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        hud = FindFirstObjectByType<HUDManager>();
        whaleObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && inventory.hasHorn && !whaleSummoned)
            SummonWhale();
    }

    void SummonWhale()
    {
        whaleSummoned = true;
        whaleObject.SetActive(true);

        // Swap cameras
        playerCamera.gameObject.SetActive(false);
        cinematicCamera.gameObject.SetActive(true);

        whaleTimeline.Play();
        hud.SetStatus("The ocean trembles...");

        Invoke(nameof(OnWhaleArrived), (float)whaleTimeline.duration);
    }

    void OnWhaleArrived()
    {
        whaleArrived = true;

        // Swap back to player camera
        cinematicCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        hud.SetStatus("The whale waits beside your raft. Press [E] to trade.");
    }

    public bool IsWhalePresent() { return whaleArrived; }
}