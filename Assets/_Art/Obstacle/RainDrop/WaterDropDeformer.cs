using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Rigidbody), typeof(MeshCollider))]
public class WaterDropDeformer : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;

    private Vector3[] originalVertices;   // 로컬 좌표계 기준
    private Vector3[] deformedVertices;
    private Vector3[] vertexVelocities;

    private Rigidbody rb;

    [Header("Physics")]
    public float springStrength = 20f;
    public float damping = 4f;

    [Header("Deformation Control")]
    public float deformationThreshold = 0.001f;   // 너무 가까우면 복원 X
    public float velocityThreshold = 0.001f;      // 정지 판정 속도

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();

        mesh = meshFilter.mesh;
        originalVertices = mesh.vertices;
        deformedVertices = new Vector3[originalVertices.Length];
        vertexVelocities = new Vector3[originalVertices.Length];
        originalVertices.CopyTo(deformedVertices, 0);

        // Collider 업데이트용
        meshCollider.sharedMesh = mesh;
    }

    void Update()
    {
        // 리지드바디 속도 또는 트랜스폼 이동 여부로 변형 조건 결정
        bool allowDeform = rb.velocity.magnitude > velocityThreshold || transform.hasChanged;
        transform.hasChanged = false;

        for (int i = 0; i < deformedVertices.Length; i++)
        {
            Vector3 localPos = deformedVertices[i];
            Vector3 original = originalVertices[i];

            Vector3 toOriginal = original - localPos;
            float dist = toOriginal.magnitude;

            if (!allowDeform && dist < deformationThreshold)
                continue;

            // 복원력 (스프링)
            Vector3 force = springStrength * toOriginal;

            // 감쇠력
            force -= damping * vertexVelocities[i];

            // 뉴턴 운동 방정식 (질량 1 가정)
            Vector3 acceleration = force;
            vertexVelocities[i] += acceleration * Time.deltaTime;
            deformedVertices[i] += vertexVelocities[i] * Time.deltaTime;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // 콜라이더 업데이트
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    public void ResetMesh()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            deformedVertices[i] = originalVertices[i];
            vertexVelocities[i] = Vector3.zero;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
}
