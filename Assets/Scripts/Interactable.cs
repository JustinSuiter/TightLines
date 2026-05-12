using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string interactionName = "Interact";
    public float range = 3f;
    public UnityEvent onInteract;

    public void DoInteraction()
    {
        onInteract?.Invoke();
    }
}