using UnityEngine;

public class RaftFloater : MonoBehaviour
{
    [Header("References")]
    public WaveOcean ocean;          // Drag the Ocean object here

    [Header("Float Settings")]
    public float floatOffset = 0.2f; // Raises the raft slightly above the wave surface
    public float tiltStrength = 30f; // How much it tilts to match wave slope
    public float tiltSmoothness = 3f; // Higher = smoother tilting

    void Update()
    {
        if (ocean == null) return;

        float t = Time.time;
        Vector3 pos = transform.position;

        // Get the wave height directly under the raft
        float waveY = ocean.GetWaveHeight(pos.x, pos.z, t);

        // Set the raft's Y position to match the wave
        transform.position = new Vector3(pos.x, waveY + floatOffset, pos.z);

        // Calculate tilt by sampling waves slightly forward and to the side
        float sampleDistance = 2f;
        float frontHeight = ocean.GetWaveHeight(pos.x, pos.z + sampleDistance, t);
        float backHeight = ocean.GetWaveHeight(pos.x, pos.z - sampleDistance, t);
        float rightHeight = ocean.GetWaveHeight(pos.x + sampleDistance, pos.z, t);
        float leftHeight = ocean.GetWaveHeight(pos.x - sampleDistance, pos.z, t);

        // Tilt forward/backward based on front-to-back wave difference
        float tiltX = (frontHeight - backHeight) * tiltStrength;
        // Tilt left/right based on side-to-side wave difference
        float tiltZ = (leftHeight - rightHeight) * tiltStrength;

        Quaternion targetRotation = Quaternion.Euler(-tiltX, 0f, tiltZ);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSmoothness);
    }
}