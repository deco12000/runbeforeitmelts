using UnityEngine;
using DG.Tweening;

public class BoosterItem : MonoBehaviour
{
    [Header("움직임 설정")]
    public float floatDistance = 0.5f;
    public float floatDuration = 1.5f;
    public Ease floatEase = Ease.InOutSine;

    [Header("회전 설정")]
    public float rotationDuration = 4f;
    public Ease rotationEase = Ease.Linear;

    [Header("체력 회복량")]
    public int healAmount = 50;

    [Header("이펙트")]
    public GameObject idleEffectPrefab;     // 지속 이펙트
    public GameObject breakEffectPrefab;    // 파괴 이펙트

    private GameObject idleEffectInstance;  // 인스턴스 참조용

    void Start()
    {
        float randomDuration = Random.Range(rotationDuration * 0.8f, rotationDuration * 1.2f);

        transform.DOMoveY(transform.position.y + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(floatEase);

        transform.DORotate(new Vector3(0, 360, 0), randomDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(rotationEase);

        // 아이템 지속 이펙트 생성 (Y축 -0.8 아래, 부모는 this.transform)
        if (idleEffectPrefab != null)
        {
            idleEffectInstance = Instantiate(idleEffectPrefab, transform);
            idleEffectInstance.transform.localPosition = new Vector3(0f, -1.2f, 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"체력 {healAmount} 회복!");

            // 파괴 이펙트 재생
            if (breakEffectPrefab != null)
            {
                GameObject effect = Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);

                ParticleSystem ps = effect.GetComponent<ParticleSystem>() ?? effect.GetComponentInChildren<ParticleSystem>();
                if (ps != null) ps.Play();
            }

            // 아이들 이펙트 제거
            if (idleEffectInstance != null)
            {
                Destroy(idleEffectInstance);
            }

            Destroy(gameObject); // 아이템 소멸
        }
    }
}
