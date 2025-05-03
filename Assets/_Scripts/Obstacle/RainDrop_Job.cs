using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Profiling;
public class RainDrop_Job : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;
    private Vector3[] originalVertices;
    private NativeArray<Vector3> deformedVertices;
    private NativeArray<Vector3> nativeVertices;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float maxVelocityY = -5f;
    private float velocityY = 0f;
    private bool isGrounded = false;
    [SerializeField] private float yDeform = 0.4f;
    [SerializeField] private float xzDeform = 1.3f;
    [SerializeField] private float recoverySpeed = 0.1f;
    [SerializeField] private float protruding = 1f;
    [SerializeField] private float rounded = 1f;
    [SerializeField] private float maxTime = 0.5f;
    private float startTime = 0f;
    private NativeArray<float> directions;
    private static readonly int groundLayerMask = 1 << 3;
    void Start()
    {
        mesh = Instantiate(GetComponent<MeshFilter>().mesh);
        GetComponent<MeshFilter>().mesh = mesh;
        TryGetComponent(out meshCollider);
        originalVertices = mesh.vertices;

        nativeVertices = new NativeArray<Vector3>(originalVertices, Allocator.Persistent);
        deformedVertices = new NativeArray<Vector3>(originalVertices.Length, Allocator.Persistent);
        directions = new NativeArray<float>(AnglesOnCircle(UnityEngine.Random.Range(4, 7)), Allocator.Persistent);
    }
    void Update()
    {
        if (!isGrounded)
        {
            ApplyGravity();
            if (CheckGroundHit(out float groundY))
            {
                transform.position = new Vector3(
                    transform.position.x,
                    groundY + transform.localScale.y * 0.15f,
                    transform.position.z
                );
                isGrounded = true;
                DeformMeshAsync(groundY);
            }
        }
        else
        {
            RelaxMesh();
        }
    }
    private void ApplyGravity()
    {
        velocityY += gravity * Time.deltaTime;
        velocityY = Mathf.Clamp(velocityY, maxVelocityY, 0f);
        transform.position += Vector3.up * velocityY * Time.deltaTime;
    }
    private bool CheckGroundHit(out float groundY)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.2f, groundLayerMask))
        {
            groundY = hit.point.y;
            return true;
        }
        groundY = 0f;
        return false;
    }
    private void DeformMeshAsync(float groundY)
    {
        Profiler.BeginSample("DeformMeshAsync");
        var deformJob = new DeformJob
        {
            originalVertices = nativeVertices,
            groundY = groundY,
            yDeform = yDeform,
            xzDeform = xzDeform,
            protruding = protruding,
            rounded = rounded,
            directions = directions,
            deformedVertices = deformedVertices
        };
        JobHandle handle = deformJob.Schedule(nativeVertices.Length, 64);
        handle.Complete();
        mesh.vertices = deformedVertices.ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        Profiler.EndSample();
    }
    private void RelaxMesh()
    {
        if (startTime < 0.01f) startTime = Time.time;
        if (Time.time - startTime > maxTime) return;
        float lerpT = Time.deltaTime * recoverySpeed;
        bool changed = false;
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            Vector3 from = deformedVertices[i];
            Vector3 to = originalVertices[i];
            Vector3 v = Vector3.Lerp(from, to, lerpT);

            if ((v - to).sqrMagnitude > 0.0001f)
                changed = true;

            deformedVertices[i] = v;
        }
        if (changed)
        {
            mesh.vertices = deformedVertices.ToArray();
            mesh.RecalculateNormals();
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }
    public float[] AnglesOnCircle(int count)
    {
        if (count < 2) return new float[] { 0f };
        float[] angles = new float[count];
        float step = 360f / count;

        for (int i = 0; i < count; i++)
            angles[i] = -180f + i * step;
        return angles;
    }
    [BurstCompile]
    struct DeformJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> originalVertices;
        [ReadOnly] public float groundY;
        [ReadOnly] public float yDeform;
        [ReadOnly] public float xzDeform;
        [ReadOnly] public float protruding;
        [ReadOnly] public float rounded;
        [ReadOnly] public NativeArray<float> directions;
        public NativeArray<Vector3> deformedVertices;
        public void Execute(int index)
        {
            Vector3 v = originalVertices[index];
            float factor = 1f - math.abs(v.y - groundY);
            v.y = math.lerp(v.y, groundY, factor * yDeform);
            float angleRad = math.atan2(v.z, v.x);
            float angleDeg = angleRad * 57.29578f; // Mathf.Rad2Deg
            float minDelta = 180f;
            for (int i = 0; i < directions.Length; i++)
            {
                float delta = math.abs(angleDeg - directions[i]);
                delta = delta > 180f ? 360f - delta : delta;
                minDelta = math.min(minDelta, delta);
            }
            float angleWeight = math.pow(1f - minDelta * rounded * 0.01f, 3f);
            float deformScale = 1f + factor * (angleWeight * protruding + 1f) * xzDeform;
            v.x *= deformScale;
            v.z *= deformScale;
            deformedVertices[index] = v;
        }
    }
    private void OnDestroy()
    {
        if (deformedVertices.IsCreated)
            deformedVertices.Dispose();
        if (nativeVertices.IsCreated)
            nativeVertices.Dispose();
        if (directions.IsCreated)
            directions.Dispose();
    }
}
