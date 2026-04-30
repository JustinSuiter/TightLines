using UnityEngine;

public class PlayerBoatStabilizer : MonoBehaviour
{
    private Rigidbody rb;
    public float idleDrag = 10f;
    public float movingDrag = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        rb.linearDamping = isMoving ? movingDrag : idleDrag;
    }
}