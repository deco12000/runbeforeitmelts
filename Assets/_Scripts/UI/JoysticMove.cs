using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class JoysticMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle;
    private RectTransform rt;
    private Image img;
    public float sensitivity = 1f;
    public float handleRange = 50f;
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
    }
    void OnEnable()
    {
        input = Player.Instance.input;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (releaseCo != null)
            StopCoroutine(releaseCo);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out center);
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        releaseCo = StartCoroutine(Release());
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return;

        if(input == null) input = Player.Instance.input;
        Vector2 delta = localPoint - center;
        Vector2 norm = new Vector2(
            delta.x / (rt.sizeDelta.x * 0.5f),
            delta.y / (rt.sizeDelta.y * 0.5f)
        ) * sensitivity;
        norm = Vector2.ClampMagnitude(norm, 1f);
        input.moveDirection = norm;
        if (handle != null)
            handle.anchoredPosition = norm * handleRange;
    }
    IEnumerator Release()
    {
        Vector2 startDir = input.moveDirection;
        Vector2 startPos = handle != null ? handle.anchoredPosition : Vector2.zero;
        float duration = 0.3f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime * 3f;
            input.moveDirection = Vector2.Lerp(startDir, Vector2.zero, t / duration);
            if (handle != null)
                handle.anchoredPosition = Vector2.Lerp(startPos, Vector2.zero, t / duration);
            yield return null;
        }
        input.moveDirection = Vector2.zero;
        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }
}