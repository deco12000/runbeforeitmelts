using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ButtonJump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle;
    public Image pressedImage;
    private RectTransform rt;
    private Image img;
    public float sensitivity = 1f;
    public float handleRange = 10f;
    public float handleRangeX = 5f;
    private Coroutine releaseCo;
    private Coroutine autoReleaseCo;
    private Coroutine boolReleaseCo;
    private PlayerInput input;
    private Vector2 center = Vector2.zero;
    private Vector2 lastDirection = Vector2.zero;
    private float startTime;
    private bool isPressed = false;
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
            pressedImage.gameObject.SetActive(false);
    }
    void OnEnable()
    {
        input = Player.Instance.input;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (releaseCo != null) StopCoroutine(releaseCo);
        if (autoReleaseCo != null) StopCoroutine(autoReleaseCo);
        if (boolReleaseCo != null) StopCoroutine(boolReleaseCo);
        isPressed = true;
        startTime = Time.time;
        if (pressedImage != null)
            pressedImage.gameObject.SetActive(true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out center);
        OnDrag(eventData);
        input.isJump = true;
        input.OnJumpDown.Invoke();
        boolReleaseCo = StartCoroutine(BoolRelease());
        autoReleaseCo = StartCoroutine(AutoReleaseAfterDelay(0.235f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;
        if (autoReleaseCo != null) StopCoroutine(autoReleaseCo);
        if (boolReleaseCo != null) StopCoroutine(boolReleaseCo);
        if (pressedImage != null)
            pressedImage.gameObject.SetActive(false);
        input.OnJumpUp?.Invoke(lastDirection, Time.time - startTime);
        releaseCo = StartCoroutine(Release());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPressed) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return;
        if (input == null) input = Player.Instance.input;
        Vector2 delta = localPoint - center;
        float normY = Mathf.Clamp(delta.y / (rt.sizeDelta.y * 0.5f) * sensitivity, -1f, 1f);
        float normX = Mathf.Clamp(delta.x / (rt.sizeDelta.x * 0.5f) * sensitivity * 0.1f, -1f, 1f);
        Vector2 direction = new Vector2(normX, normY);
        input.jumpDirection = direction;
        lastDirection = direction;
        if (handle != null)
            handle.anchoredPosition = new Vector2(normX * handleRangeX, normY * handleRange);
    }

    IEnumerator BoolRelease()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        input.isJump = false;
    }

    IEnumerator AutoReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isPressed) yield break;
        isPressed = false;
        input.OnJumpAutoRelease?.Invoke(lastDirection, delay);
        if (pressedImage != null)
            pressedImage.gameObject.SetActive(false);
        releaseCo = StartCoroutine(Release());
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
