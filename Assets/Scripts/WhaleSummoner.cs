using UnityEngine;
using System.Collections;

public class WhaleSummoner : MonoBehaviour
{
    [Header("References")]
    public GameObject whaleObject;
    public Transform spawnPoint;
    public HUDManager hud;

    [Header("Settings")]
    public float approachSpeed = 4f;
    public Vector3 finalPosition = new Vector3(10f, -0.5f, 0f);

    private PlayerInventory inventory;
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
        // Press H to blow horn — only if player has it AND whale isn't already summoned
        if (Input.GetKeyDown(KeyCode.H) && inventory.hasHorn && !whaleSummoned)
        {
            SummonWhale();
        }
    }

    void SummonWhale()
    {
        whaleSummoned = true;
        whaleObject.transform.position = spawnPoint.position;
        whaleObject.SetActive(true);

        hud.SetStatus("The ocean trembles... something approaches.");
        StartCoroutine(WhaleApproach());
    }

    IEnumerator WhaleApproach()
    {
        while (Vector3.Distance(whaleObject.transform.position, finalPosition) > 0.5f)
        {
            // Move whale toward final position
            whaleObject.transform.position = Vector3.MoveTowards(
                whaleObject.transform.position,
                finalPosition,
                approachSpeed * Time.deltaTime
            );

            // Rotate whale to face the raft as it approaches
            Vector3 lookDir = (transform.position - whaleObject.transform.position).normalized;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                whaleObject.transform.rotation = Quaternion.Slerp(
                    whaleObject.transform.rotation, targetRot, Time.deltaTime * 2f);
            }

            yield return null;
        }

        whaleArrived = true;
        hud.SetStatus("The whale waits beside your raft. Press [E] to trade.");
    }

    public bool IsWhalePresent() { return whaleArrived; }
}