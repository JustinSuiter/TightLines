using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveOcean : MonoBehaviour
{
    [Header("Mesh Size")]
    public int gridSize = 60;          // Vertices per side (60x60 = 3600 vertices)
    public float spacing = 4f;         // Distance between vertices (total size = gridSize * spacing)

    [Header("Wave Settings")]
    public float wave1Height = 0.5f;
    public float wave1Speed = 1f;
    public float wave1Length = 8f;
    public Vector2 wave1Direction = new Vector2(1f, 0.3f);

    public float wave2Height = 0.3f;
    public float wave2Speed = 1.4f;
    public float wave2Length = 5f;
    public Vector2 wave2Direction = new Vector2(-0.5f, 1f);

    public float wave3Height = 0.15f;
    public float wave3Speed = 1.8f;
    public float wave3Length = 3f;
    public Vector2 wave3Direction = new Vector2(0.7f, -0.7f);

    [Header("Choppiness")]
    public float noiseStrength = 0.1f;
    public float noiseScale = 0.3f;

    private Mesh mesh;
    private Vector3[] baseVertices;    // Original flat positions
    private Vector3[] currentVertices; // Animated positions

    void Start()
    {
        GenerateMesh();
    }

    void Update()
    {
        AnimateWaves();
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // allows large meshes
        GetComponent<MeshFilter>().mesh = mesh;

        int vertexCount = (gridSize + 1) * (gridSize + 1);
        baseVertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];

        // Build the grid of vertices
        float halfSize = (gridSize * spacing) / 2f;
        for (int z = 0, i = 0; z <= gridSize; z++)
        {
            for (int x = 0; x <= gridSize; x++, i++)
            {
                baseVertices[i] = new Vector3(x * spacing - halfSize, 0f, z * spacing - halfSize);
                uvs[i] = new Vector2((float)x / gridSize, (float)z / gridSize);
            }
        }

        // Build triangles connecting the vertices
        int[] triangles = new int[gridSize * gridSize * 6];
        for (int z = 0, t = 0, v = 0; z < gridSize; z++, v++)
        {
            for (int x = 0; x < gridSize; x++, v++)
            {
                triangles[t++] = v;
                triangles[t++] = v + gridSize + 1;
                triangles[t++] = v + 1;
                triangles[t++] = v + 1;
                triangles[t++] = v + gridSize + 1;
                triangles[t++] = v + gridSize + 2;
            }
        }

        currentVertices = (Vector3[])baseVertices.Clone();

        mesh.vertices = baseVertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    void AnimateWaves()
    {
        float t = Time.time;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            float y = GetWaveHeight(v.x, v.z, t);
            currentVertices[i] = new Vector3(v.x, y, v.z);
        }

        mesh.vertices = currentVertices;
        mesh.RecalculateNormals();
    }

    // PUBLIC — the boat will call this to know how high to float
    public float GetWaveHeight(float x, float z, float time)
    {
        float total = 0f;

        total += DirectionalWave(x, z, time, wave1Direction, wave1Height, wave1Speed, wave1Length);
        total += DirectionalWave(x, z, time, wave2Direction, wave2Height, wave2Speed, wave2Length);
        total += DirectionalWave(x, z, time, wave3Direction, wave3Height, wave3Speed, wave3Length);

        // Add Perlin noise for randomness — breaks up the pattern
        float noise = (Mathf.PerlinNoise((x + time) * noiseScale, (z + time) * noiseScale) - 0.5f) * 2f;
        total += noise * noiseStrength;

        return total;
    }

    float DirectionalWave(float x, float z, float time, Vector2 direction, float height, float speed, float length)
    {
        Vector2 dir = direction.normalized;
        float dot = x * dir.x + z * dir.y;
        return Mathf.Sin((dot / length) + time * speed) * height;
    }
    
}