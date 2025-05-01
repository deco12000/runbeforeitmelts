using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoysticMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick Components")]
    public RectTransform handle; // 손잡이 이미지
    RectTransform rt;
    Image img;
    [Header("Joystick Settings")]
    public float sensitivity = 1f;
    public float handleRange = 50f; // 손잡이 최대 이동 반경
    Vector2 center;
    Rect rect;
    Coroutine releaseCoroutine;
    PlayerInput input;
    void Awake()
    {
        TryGetComponent(out img);
        TryGetComponent(out rt);
#if UNITY_EDITOR
        img.color = new Color(1f, 1f, 1f, 0.02f);
#else
        img.color = new Color(1f, 1f, 1f, 0f);
#endif
    }
    void OnEnable()
    {
        input = PlayerGroup.Instance.pinput;
        rect = rt.GetScreenRect();
        center = new Vector2(rect.x + 0.5f * rect.width, rect.y + 0.5f * rect.height);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (releaseCoroutine != null)
            StopCoroutine(releaseCoroutine);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        releaseCoroutine = StartCoroutine(Release());
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = (eventData.position - center);
        Vector2 norm = new Vector2(delta.x / rect.width, delta.y / rect.height) * sensitivity;
        norm = Vector2.ClampMagnitude(norm, 1f);
        input.direction = norm;
        // 손잡이 이동
        if (handle != null)
            handle.anchoredPosition = norm * handleRange;
    }
    IEnumerator Release()
    {
        Vector2 startDir = input.direction;
        Vector2 startPos = handle != null ? handle.anchoredPosition : Vector2.zero;
        float duration = 0.3f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime * 3f;
            input.direction = Vector2.Lerp(startDir, Vector2.zero, t / duration);
            if (handle != null)
                handle.anchoredPosition = Vector2.Lerp(startPos, Vector2.zero, t / duration);
            yield return null;
        }
        input.direction = Vector2.zero;
        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }
}