using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptPanel;       // The "Hold E" UI panel
    public TextMeshProUGUI promptText;
    public Image holdFillBar;            // A radial or horizontal fill image

    [Header("Settings")]
    public float holdDuration = 0.8f;    // How long to hold E
    public Transform player;

    [Header("Look Detection")]
    public Camera playerCamera;
    public float lookRange = 5f;

    private Interactable currentTarget;
    private float holdTimer = 0f;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        promptPanel.SetActive(false);
    }

    void Update()
    {
        // If any menu is open (cursor visible), skip interactions entirely
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (promptPanel.activeSelf) promptPanel.SetActive(false);
            currentTarget = null;
            holdTimer = 0f;
            if (holdFillBar != null) holdFillBar.fillAmount = 0f;
            return;
        }

        FindClosestInteractable();
        HandleHoldInput();
    }

    void FindClosestInteractable()
    {
        Interactable hit = null;

        // Cast a ray from the center of the screen forward
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hitInfo, lookRange))
        {
            // Check if what we hit has an Interactable component (on itself or parents)
            Interactable found = hitInfo.collider.GetComponentInParent<Interactable>();

            if (found != null)
            {
                // Only count it if we're within the interactable's own range
                float dist = Vector3.Distance(player.position, found.transform.position);
                if (dist <= found.range)
                    hit = found;
            }
        }

        if (hit != currentTarget)
        {
            currentTarget = hit;
            holdTimer = 0f;
            if (holdFillBar != null) holdFillBar.fillAmount = 0f;
            UpdatePromptUI();
        }
    }

    void HandleHoldInput()
    {
        if (currentTarget == null) return;

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            holdFillBar.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                currentTarget.DoInteraction();
                holdTimer = 0f;
                holdFillBar.fillAmount = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
            holdFillBar.fillAmount = 0f;
        }
    }

    void UpdatePromptUI()
    {
        if (currentTarget != null)
        {
            promptPanel.SetActive(true);
            promptText.text = "Hold [E] to " + currentTarget.interactionName;
        }
        else
        {
            promptPanel.SetActive(false);
        }
    }
}