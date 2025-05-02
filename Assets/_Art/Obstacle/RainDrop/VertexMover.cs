using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VertexMover : MonoBehaviour
{
    public int longitudeSegments = 24; // 경도
    public int latitudeSegments = 16;  // 위도
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateSphere();
    }

    void CreateSphere()
    {
        int vertCount = (longitudeSegments + 1) * (latitudeSegments + 1);
        originalVertices = new Vector3[vertCount];
        vertices = new Vector3[vertCount];
        triangles = new int[longitudeSegments * latitudeSegments * 6];

        int vert = 0;
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float a1 = Mathf.PI * lat / latitudeSegments;
            float y = Mathf.Cos(a1);
            float sinA1 = Mathf.Sin(a1);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float a2 = 2 * Mathf.PI * lon / longitudeSegments;
                float x = Mathf.Cos(a2) * sinA1;
                float z = Mathf.Sin(a2) * sinA1;

                Vector3 pos = new Vector3(x, y, z);
                originalVertices[vert] = pos;
                vertices[vert] = pos;
                vert++;
            }
        }

        int tri = 0;
        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int current = lat * (longitudeSegments + 1) + lon;
                int next = current + longitudeSegments + 1;

                triangles[tri++] = current;
                triangles[tri++] = next;
                triangles[tri++] = current + 1;

                triangles[tri++] = current + 1;
                triangles[tri++] = next;
                triangles[tri++] = next + 1;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void Update()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 dir = originalVertices[i].normalized;
            float offset = Mathf.Sin(Time.time * 2f + i * 0.05f) * 0.05f;
            vertices[i] = dir * (1f + offset); // 구 방향 진동
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
