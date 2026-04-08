using UnityEngine;

public class BoatFloating : MonoBehaviour
{
    public float waveHeight = 0.3f;
    public float waveSpeed = 0.8f;
    public float tiltAmount = 3f;
    public float tiltSpeed = 0.5f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        float t = Time.time;
        float newY = startPosition.y + ((Mathf.Sin(t * waveSpeed) + 1f) / 2f) * waveHeight;
        float tiltX = Mathf.Sin(t * tiltSpeed * 1.3f) * tiltAmount;
        float tiltZ = Mathf.Sin(t * tiltSpeed) * tiltAmount;

        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        transform.rotation = startRotation * Quaternion.Euler(tiltX, 0f, tiltZ);
    }
}