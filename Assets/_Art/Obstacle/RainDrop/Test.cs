using UnityEngine;

public class WaterDropDeformation : MonoBehaviour
{
    public MeshFilter meshFilter;  // Ico Sphere의 MeshFilter
    private Mesh mesh;             // Ico Sphere의 Mesh
    private Vector3[] vertices;    // 버텍스 배열
    private int[] triangles;       // 삼각형 배열
    private Vector3[] originalVertices; // 원본 버텍스 (초기값 저장용)
    private Rigidbody rb;          // 물리 엔진에서 사용하는 Rigidbody
    private MeshCollider meshCollider; // 메쉬 콜라이더

    private float radius = 1f;     // 구의 반지름
    private float surfaceTensionStrength = 0.1f; // 표면장력 강도

    // Start() 메서드: 초기화
    void Start()
    {
        // 메쉬, 리지드바디, 메쉬 콜라이더 가져오기
        mesh = meshFilter.mesh;
        rb = GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();

        vertices = mesh.vertices;
        triangles = mesh.triangles;
        originalVertices = (Vector3[])vertices.Clone(); // 원본 버텍스 저장
    }

    // Update() 메서드: 매 프레임마다 호출
    void Update()
    {
        // 리지드바디 속도가 0일 경우 구 형태 유지
        if (rb.velocity.magnitude == 0)
        {
            MaintainSphericalShape();
        }
        else
        {
            // 물리적 힘과 속도에 따른 변형 적용
            ApplySurfaceTension();
        }

        // 버텍스 업데이트
        mesh.vertices = vertices;
        mesh.RecalculateNormals();  // 렌더링을 위해 노멀 재계산
    }

    // 구 형태 유지
    private void MaintainSphericalShape()
    {
        Vector3 transformPosition = transform.position;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = (originalVertices[i] + transformPosition).normalized * radius;
        }
    }

    // 표면장력 적용하여 버텍스 변형
    private void ApplySurfaceTension()
    {
        // 표면장력 강도를 적용하여 버텍스를 변형시킴
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexPos = vertices[i];
            Vector3 directionToCenter = (vertexPos - transform.position).normalized;

            // 표면장력을 적용하여 각 버텍스를 조정
            vertices[i] = (vertexPos + directionToCenter * surfaceTensionStrength).normalized * radius;
        }
    }

    // 충돌 시 버텍스 변형 (충돌점 근처의 버텍스를 변형)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 0.1f)
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            DeformVerticesBasedOnCollision(collisionPoint);
        }
    }

    // 충돌 지점에 따라 물방울 버텍스를 변형
    private void DeformVerticesBasedOnCollision(Vector3 collisionPoint)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // 충돌점으로부터의 거리 계산
            float distance = Vector3.Distance(collisionPoint, vertices[i]);

            // 가까운 버텍스일수록 더 많이 변형
            if (distance < 0.2f)
            {
                Vector3 directionToCollision = (vertices[i] - collisionPoint).normalized;
                vertices[i] += directionToCollision * 0.05f; // 변형 강도
            }
        }
    }
}
