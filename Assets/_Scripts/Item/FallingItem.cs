using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("파괴 시 이펙트 프리팹 (2개까지)")]
    public GameObject[] destroyEffects;

    [Header("그림자 설정")]
    public Material shadowMaterial;
    public Vector3 shadowOffset = new Vector3(0, 0.01f, 0);
    public Vector3 shadowScaleMin = new Vector3(0.3f, 0.3f, 1f);
    public Vector3 shadowScaleMax = new Vector3(1.2f, 1.2f, 1f);

    [Header("플레이어 데미지")]
    public int damageToPlayer = 10;

    private GameObject shadowInstance;
    private Rigidbody rb;
    private float initialY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialY = transform.position.y;
        CreateShadow();
    }

    void CreateShadow()
    {
        shadowInstance = GameObject.CreatePrimitive(PrimitiveType.Quad);
        shadowInstance.name = "SimpleShadow";
        shadowInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
        shadowInstance.transform.position = new Vector3(transform.position.x, 0f, transform.position.z) + shadowOffset;

        if (shadowMaterial != null)
            shadowInstance.GetComponent<MeshRenderer>().material = shadowMaterial;

        Destroy(shadowInstance.GetComponent<Collider>());
    }

    void Update()
    {
        if (shadowInstance == null) return;

        // 높이에 따라 그림자 크기 보간
        float heightRatio = Mathf.InverseLerp(0f, initialY, transform.position.y);
        Vector3 currentScale = Vector3.Lerp(shadowScaleMax, shadowScaleMin, heightRatio);  // 낮을수록 커짐
        shadowInstance.transform.localScale = currentScale;

        shadowInstance.transform.position = new Vector3(transform.position.x, 0f, transform.position.z) + shadowOffset;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log($"플레이어가 {damageToPlayer} 데미지를 받았습니다!");
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
