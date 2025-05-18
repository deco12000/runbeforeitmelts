using UnityEngine;

public class MobileTouchEffect : MonoBehaviour
{
    void Update()
    {
        // 모바일 터치 입력을 확인합니다.
        if (Input.touchCount > 0)
        {
            // 동시에 여러 터치가 발생해도 처음 2개만 처리합니다.
            for (int i = 0; i < Mathf.Min(Input.touchCount, 2); i++)
            {
                Touch touch = Input.touches[i];
                // 터치가 시작되는 순간(TouchPhase.Began)에만 이펙트를 생성합니다.
                if (touch.phase == TouchPhase.Began)
                {
                    ParticleManager.I.PlayEffectSprite("TouchTick", touch.position, 1.35f, 0.08f);
                    SoundManager.I.PlaySFX("TouchTick");
                }
            }
        }
    }
}
