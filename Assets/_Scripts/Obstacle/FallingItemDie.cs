using UnityEngine;

public class FallingItemDie : MonoBehaviour
{
    [Header("파괴 시 이펙트 프리팹 (2개까지)")]
    public GameObject[] destroyEffects;

    [Header("그림자 설정")]
    public Material shadowMaterial;
    public Vector3 shadowOffset = new Vector3(0, 0.01f, 0);
    public Vector2 shadowScaleMin = new Vector2(0.3f, 0.3f);
    public Vector2 shadowScaleMax = new Vector2(1.2f, 1.2f);

    [Tooltip("그림자 크기 배수 (X: 가로, Y: 세로)")]
    public Vector2 shadowScaleMultiplier = new Vector2(1f, 1f);

    private GameObject shadowInstance;
    private Rigidbody rb;
    private float initialY;
    private Renderer objectRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectRenderer = GetComponentInChildren<Renderer>();
        initialY = transform.position.y;
        CreateShadow();
    }

    void CreateShadow()
    {
        shadowInstance = GameObject.CreatePrimitive(PrimitiveType.Quad);
        shadowInstance.name = "SimpleShadow";
        shadowInstance.transform.rotation = Quaternion.Euler(90, 0, 0);

        if (shadowMaterial != null)
            shadowInstance.GetComponent<MeshRenderer>().material = shadowMaterial;

        Destroy(shadowInstance.GetComponent<Collider>());
    }

    void Update()
    {
        if (shadowInstance == null || objectRenderer == null) return;

        float heightRatio = Mathf.InverseLerp(0f, initialY, transform.position.y);
        Vector2 baseScale = Vector2.Lerp(shadowScaleMax, shadowScaleMin, heightRatio);

        Vector3 adjustedScale = new Vector3(
            baseScale.x * transform.localScale.x * shadowScaleMultiplier.x,
            baseScale.y * transform.localScale.z * shadowScaleMultiplier.y,
            1f
        );

        shadowInstance.transform.localScale = adjustedScale;

        // 오브젝트 렌더링 기준 중심을 기반으로 그림자 위치 지정
        Vector3 center = objectRenderer.bounds.center;
        shadowInstance.transform.position = new Vector3(center.x, 0f, center.z) + shadowOffset;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("플레이어가 사망했습니다.");
            }

            PlayEffects();
            DestroyShadow();
            Destroy(gameObject);
        }
    }

    void PlayEffects()
    {
        foreach (GameObject effectPrefab in destroyEffects)
        {
            if (effectPrefab != null)
            {
                GameObject fx = Instantiate(effectPrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = fx.GetComponent<ParticleSystem>() ?? fx.GetComponentInChildren<ParticleSystem>();
                if (ps != null) ps.Play();

                Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    void DestroyShadow()
    {
        if (shadowInstance != null)
        {
            Destroy(shadowInstance);
        }
    }
}
