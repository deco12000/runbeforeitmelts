using UnityEngine;
using DG.Tweening;

public class HealItem : MonoBehaviour
{
    [Header("움직임 설정")]
    public float floatDistance = 0.5f;       // 위아래 이동 거리
    public float floatDuration = 1.5f;       // 위아래 이동 속도
    public Ease floatEase = Ease.InOutSine;  // 위아래 움직임의 이징

    [Header("회전 설정")]
    public float rotationDuration = 4f;      // 360도 회전하는 데 걸리는 시간
    public Ease rotationEase = Ease.Linear;  // 회전 이징

    [Header("체력 회복량")]
    public int healAmount = 50;

    void Start()
    {
        // 떠오르기 애니메이션
        transform.DOMoveY(transform.position.y + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(floatEase);

        // 회전 애니메이션
        transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(rotationEase);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"체력 {healAmount} 회복!");

            Destroy(gameObject); // 아이템 소멸
        }
    }
}

