using UnityEngine;
using DG.Tweening;

public class HealItem : MonoBehaviour
{
    [Header("움직임 설정")]
    public float floatDistance = 0.5f;
    public float floatDuration = 1.5f;
    public Ease floatEase = Ease.InOutSine;

    [Header("회전 설정")]
    public float rotationDuration = 4f;
    public Ease rotationEase = Ease.Linear;

    [Header("체력 회복량")]
    public int healAmount = 25;

    [Header("이펙트")]
    public GameObject idleEffectPrefab;     // 아이템 존재 중 이펙트
    public GameObject breakEffectPrefab;    // 아이템 파괴 시 이펙트

    private GameObject idleEffectInstance;  // 인스턴스 저장용

    void Start()
    {
        // 회전 속도에 랜덤성 추가
        float randomDuration = Random.Range(rotationDuration * 0.8f, rotationDuration * 1.2f);

        // 부드러운 위아래 움직임
        transform.DOMoveY(transform.position.y + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(floatEase);

        // 지속 회전
        transform.DORotate(new Vector3(0, 360, 0), randomDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(rotationEase);

        // // 아이템 지속 이펙트 생성 (아이템보다 아래, 부모 없음)
        // if (idleEffectPrefab != null)
        // {
        //     Vector3 offsetPosition = transform.position + new Vector3(0f, -0.8f, 0f);
        //     idleEffectInstance = Instantiate(idleEffectPrefab, offsetPosition, Quaternion.identity);
        // }

        
    }




    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"체력 {healAmount} 회복!");


            GetItemData data = new GetItemData("Heal");
            EventHub.Instance.Invoke<EventGetItem>(data);










            // // 파괴 이펙트 재생
            // if (breakEffectPrefab != null)
            // {
            //     GameObject effect = Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);

            //     // 수동 재생 (Play On Awake가 꺼져 있는 경우 대비)
            //     ParticleSystem ps = effect.GetComponent<ParticleSystem>() ?? effect.GetComponentInChildren<ParticleSystem>();
            //     if (ps != null) ps.Play();
            // }

            // // 지속 이펙트 제거
            // if (idleEffectInstance != null)
            // {
            //     Destroy(idleEffectInstance);
            // }

            // 아이템 제거
            Destroy(gameObject);
        }
    }
}
