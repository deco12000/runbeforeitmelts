using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Deform;
//--------DOTS화--------
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
//----------------------
public class WaterDropModel : MonoBehaviour
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
        ///////////////
        Init(cts.Token).Forget();
    }
    void OnDisable() { UniTaskCancel(); }
    void OnDestroy() { UniTaskCancel(); }
    void UniTaskCancel()
    {
        try
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        cts = null;
    }
    #endregion
    //--------DOTS화--------
    public Entity waterDropEntity = Entity.Null;
    EntityManager entityManager;
    //----------------------
    [SerializeField] float thick; // 표면으로부터 얼마나 살짝 띄울 것인가
    [SerializeField] Deformable deformable;
    [SerializeField] CellularNoiseDeformer deformer;
    [SerializeField] WaterDrop waterDrop;
    Vector3 _velocity;
    public void OnTriggerStay(Collider other) => waterDrop.OnTriggetStayFromModel(other);
    MeshFilter mf;
    MeshCollider mc;
    Mesh mesh;
    Vector3[] original;
    Vector3[] verts;
    Rigidbody rb;
    float maxSettleDistance = 1.5f;
    void Start()
    {
        TryGetComponent(out mf);
        TryGetComponent(out mc);
        TryGetComponent(out rb);
        if (mf != null) // null 체크 추가
        {
            mesh = Instantiate(mf.sharedMesh); // 원본 훼손 방지
        }
        original = mesh.vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }
    async UniTask Init(CancellationToken token)
    {
        isSetting = false;
        await UniTask.DelayFrame(1, cancellationToken: token);
        //--------DOTS화--------
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // 항상 위치를 새로 세팅함
        if (!entityManager.Exists(waterDropEntity))
        {
            waterDropEntity = entityManager.CreateEntity(
                entityManager.CreateArchetype(typeof(LocalTransform))
            );
        }
        // 무조건 현재 위치로 덮어쓰기
        entityManager.SetComponentData(waterDropEntity, LocalTransform.FromPosition(waterDrop.transform.position));
        var localTransform = entityManager.GetComponentData<LocalTransform>(waterDropEntity);
        entityManager.SetComponentData(waterDropEntity, LocalTransform.FromPosition(waterDrop.transform.position));
        //----------------------
        await UniTask.DelayFrame(1, cancellationToken: token);
        mesh.vertices = original;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mf.mesh = mesh;
        mc.sharedMesh = mesh;
        //속도 랜덤?
        _velocity = Vector3.zero;
        _velocity.y = UnityEngine.Random.Range(-7f, -17f);
        deformer.MagnitudeScalar = 0.1f;
        deformer.OffsetSpeedScalar = 2f;
        deformable.UpdateMode = UpdateMode.Auto;
        isFall = true;
        await UniTask.DelayFrame(1, cancellationToken: token);
        FallLoop(cts.Token).Forget();
    }
    bool isFall = true;
    public bool isSetting = false;
    Vector3[] startVertices;
    Vector3[] targetVertices;
    bool[] isGround;
    float yMin;
    RaycastHit[] hitBuffer = new RaycastHit[10];
    RaycastHit hitGround;
    async UniTask FallLoop(CancellationToken token)
    {
        await UniTask.DelayFrame(2, cancellationToken: token);
        isSetting = true;
        while (!token.IsCancellationRequested && isFall)
        {
            await UniTask.DelayFrame(2, cancellationToken: token);
            var localTransform = entityManager.GetComponentData<LocalTransform>(waterDropEntity); // 최신화
            float3 pos = localTransform.Position;
            pos += (float3)_velocity * 2f * Time.deltaTime;
            localTransform.Position = pos;
            entityManager.SetComponentData(waterDropEntity, localTransform);
            waterDrop.transform.position = pos; // DOTS → GameObject 위치 반영
            if (waterDrop.transform.position.y < -20)
            {
                isFall = false;
                waterDrop.DeSpawn();
            }   
        }
        await UniTask.DelayFrame(1, cancellationToken: token);
        if (Physics.Raycast(waterDrop.transform.position + 4f * Vector3.up, Vector3.down, out hitGround, 4.1f, 1 << 0))
        {
            waterDrop.transform.position = hitGround.point;
            //Debug.Log($"바닥의 높이 : {hitGround.point.y}, 내 높이 : {waterDrop.transform.position.y}");
            var localTransform = entityManager.GetComponentData<LocalTransform>(waterDropEntity);
            localTransform.Position = new float3(waterDrop.transform.position.x, waterDrop.transform.position.y, waterDrop.transform.position.z);
            entityManager.SetComponentData(waterDropEntity, localTransform);
        }
    }
    //--------DOTS화--------
    void LateUpdate()
    {
        if (!isSetting) return;
        if (entityManager == null) return;
        if (waterDropEntity == Entity.Null) return;
        if (!isFall) return;
        if (entityManager.Exists(waterDropEntity))
        {
            var localTransform = entityManager.GetComponentData<LocalTransform>(waterDropEntity);
            waterDrop.transform.position = localTransform.Position; // DOTS → GameObject 위치 반영
        }
    }
    //----------------------
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 0) return;
        if (isFall)
        {
            isFall = false; // 낙하 멈춤
            waterDrop.isGround = true;
            ParticleManager.I.PlayParticle("WaterSplash", transform.position, Quaternion.identity);
            if (UnityEngine.Random.Range(0, 100) < 50)
                SoundManager.I.PlaySFX("WaterDrop", transform.position, spatial: 0.8f);
            else
                SoundManager.I.PlaySFX("WaterDrop(1)", transform.position, spatial: 0.8f);
            
            waterDrop.DespawnTime();
            deformer.MagnitudeScalar = 0.045f;
            deformer.OffsetSpeedScalar = 1f;
            deformable.UpdateMode = UpdateMode.Stop;
            verts = mesh.vertices;
            startVertices = new Vector3[verts.Length];
            targetVertices = new Vector3[verts.Length];
            Array.Copy(verts, startVertices, verts.Length);
            Array.Copy(verts, targetVertices, verts.Length);
            Method1();
            Method2();
            DeformLoop1(cts.Token).Forget();
        }
    }
    void Method1()
    {
        isGround = new bool[verts.Length];
        yMin = mc.bounds.min.y - 1f;
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 worldPos = verts[i] + transform.position;
            Vector3 ray1Origin = worldPos;
            ray1Origin.y = yMin;
            RaycastHit hit1;
            if (Physics.Raycast(ray1Origin, Vector3.up, out hit1, 100f, 1 << 4))
            {
                if ((hit1.point - worldPos).sqrMagnitude < 0.05f)
                {
                    isGround[i] = true;
                    Vector3 ray2Origin = worldPos + 0.2f * Vector3.up;
                    int numHits = Physics.RaycastNonAlloc(ray2Origin, Vector3.down, hitBuffer, maxSettleDistance, 1 << 0);
                    RaycastHit bestHit = new RaycastHit();
                    bool foundHit = false;
                    float closestDistance = float.MaxValue;
                    for (int j = 0; j < numHits; j++)
                    {
                        RaycastHit currentHit = hitBuffer[j];
                        if (!currentHit.collider.isTrigger)
                        {
                            foundHit = true;
                            if (currentHit.distance < closestDistance)
                            {
                                closestDistance = currentHit.distance;
                                bestHit = currentHit;
                            }
                            if (currentHit.distance < 0.1f)
                            {
                                break;
                            }
                        }
                    }
                    if (foundHit)
                    {
                        targetVertices[i] = bestHit.point + thick * Vector3.up - transform.position;
                    }
                    else
                    {
                        targetVertices[i] = verts[i];
                    }
                }
                else
                {
                    targetVertices[i] = verts[i];
                }
            }
            else
            {
                targetVertices[i] = verts[i];
                continue;
            }
        }
    }
    void Method2()
    {
        for (int i = 0; i < verts.Length; i++)
        {
            if (isGround[i]) continue;
            Vector3 worldPos = verts[i] + transform.position;
            Vector3 rayStartPoint = worldPos + Vector3.up * 0.2f;
            int numHits = Physics.RaycastNonAlloc(rayStartPoint, Vector3.down, hitBuffer, maxSettleDistance, 1 << 0);
            RaycastHit bestHit = new RaycastHit();
            bool foundHit = false;
            float closestDistance = float.MaxValue;
            for (int j = 0; j < numHits; j++)
            {
                RaycastHit currentHit = hitBuffer[j];
                if (!currentHit.collider.isTrigger)
                {
                    foundHit = true;
                    if (currentHit.distance < closestDistance)
                    {
                        closestDistance = currentHit.distance;
                        bestHit = currentHit;
                    }
                    if (currentHit.distance < 0.02f)
                    {
                        break;
                    }
                }
            }
            if (foundHit)
            {
                targetVertices[i] = bestHit.point + thick * 2f * Vector3.up - transform.position;
            }
            else
            {
                targetVertices[i] = verts[i];
            }
        }
    }
    async UniTask DeformLoop1(CancellationToken token)
    {
        await UniTask.DelayFrame(2, cancellationToken: token);
        Vector3[] newVerts = verts; // 캐시된 verts 사용
        for (int i = 0; i <= 20; i++)
        {
            await UniTask.DelayFrame(2, cancellationToken: token);
            for (int j = 0; j < newVerts.Length; j++)
            {
                newVerts[j] = Vector3.Lerp(newVerts[j], targetVertices[j], i * 0.05f);
            }
            mesh.vertices = newVerts;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mf.mesh = mesh;
            mc.sharedMesh = mesh;
        }
        await UniTask.DelayFrame(2, cancellationToken: token);
        deformable.UpdateMode = UpdateMode.Auto;
    }
}
