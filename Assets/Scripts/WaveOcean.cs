using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveOcean : MonoBehaviour
{
    [Header("Mesh Size")]
    public int gridSize = 60;          // Vertices per side (60x60 = 3600 vertices)
    public float spacing = 4f;         // Distance between vertices (total size = gridSize * spacing)

    [Header("Wave Settings")]
    public float waveHeight1 = 0.5f;
    public float waveSpeed1 = 1f;
    public float waveLength1 = 8f;

    public float waveHeight2 = 0.3f;
    public float waveSpeed2 = 1.4f;
    public float waveLength2 = 5f;

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
        float wave1 = Mathf.Sin((x / waveLength1) + time * waveSpeed1) * waveHeight1;
        float wave2 = Mathf.Cos((z / waveLength2) + time * waveSpeed2) * waveHeight2;
        return wave1 + wave2;
    }
}