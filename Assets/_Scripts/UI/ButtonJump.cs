using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle; // 버튼 손잡이 이미지
    public Image pressedImage;  // 버튼 눌린 이미지
    private RectTransform rt;
    private Image img;
    public float sensitivity = 1f;
    public float handleRange = 10f; // Y축 최대 이동 범위
    public float handleRangeX = 5f; // X축 최대 이동 범위 (Y축의 절반 정도로 설정)
    private Coroutine releaseCo;
    private PlayerInput input;
    private Vector2 center = Vector2.zero;

    void Awake()
    {
        TryGetComponent(out img);
        TryGetComponent(out rt);

#if UNITY_EDITOR
        img.color = new Color(1f, 1f, 1f, 0.02f);
#else
        img.color = new Color(1f, 1f, 1f, 0f);
#endif

        if (pressedImage != null)
            pressedImage.gameObject.SetActive(false);  // 초기에는 눌린 이미지 비활성화
    }

    void OnEnable()
    {
        input = Player.Instance.pinput;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (releaseCo != null)
            StopCoroutine(releaseCo);
        
        // 눌린 이미지 활성화
        if (pressedImage != null)
            pressedImage.gameObject.SetActive(true);

        // 현재 눌린 위치를 버튼 중심으로 사용
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out center);
        // 처음 누를 때도 바로 반응
        OnDrag(eventData);
        input.isJump = true;
        StopCoroutine("BoolRelease");
        StartCoroutine("BoolRelease");
    }

    IEnumerator BoolRelease()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        input.isJump = false;
    }
    



    public void OnPointerUp(PointerEventData eventData)
    {
        // 눌린 이미지 비활성화
        if (pressedImage != null)
            pressedImage.gameObject.SetActive(false);
        
        releaseCo = StartCoroutine(Release());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return;

        if (input == null) input = Player.Instance.pinput;

        // 입력 벡터: 현재 위치 - 중심
        Vector2 delta = localPoint - center;

        // Y축 방향만 처리 (위아래로만 움직이도록 제한)
        float normY = delta.y / (rt.sizeDelta.y * 0.5f) * sensitivity;
        normY = Mathf.Clamp(normY, -1f, 1f);

        // X축 방향은 Y축보다 작은 반경으로 이동 (Y축의 절반 크기)
        float normX = delta.x / (rt.sizeDelta.x * 0.5f) * sensitivity;
        normX = Mathf.Clamp(normX, -1f, 1f);

        // X축은 handleRangeX로 반경 제한
        normX = normX * handleRangeX;

        // 2D 방향 벡터로 저장
        input.jumpDirection = new Vector2(normX, normY);

        if (handle != null)
            handle.anchoredPosition = new Vector2(normX * handleRangeX, normY * handleRange);
    }

    IEnumerator Release()
    {
        Vector2 startDir = input.jumpDirection;
        Vector2 startPos = handle != null ? handle.anchoredPosition : Vector2.zero;
        float duration = 0.3f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime * 3f;
            input.jumpDirection = Vector2.Lerp(startDir, Vector2.zero, t / duration);
            if (handle != null)
                handle.anchoredPosition = Vector2.Lerp(startPos, Vector2.zero, t / duration);
            yield return null;
        }

        input.jumpDirection = Vector2.zero;
        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }
}
