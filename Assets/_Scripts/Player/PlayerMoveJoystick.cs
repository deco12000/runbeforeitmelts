using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
public class PlayerMoveJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
        ///////////////
    }
    void OnDisable() { UniTaskCancel(); }
    void OnDestroy() { UniTaskCancel(); }
    void UniTaskCancel()
    {
        try
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
        cts = null;
    }
    #endregion
    [SerializeField] private PlayerInput input;
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float handleRange = 50f;
    RectTransform handle;
    RectTransform rt;
    Image img;
    Vector2 center = Vector2.zero;
    void Awake()
    {
        TryGetComponent(out img);
        TryGetComponent(out rt);
#if UNITY_EDITOR
        img.color = new Color(1f, 1f, 1f, 0.02f);
#else
        img.color = new Color(1f, 1f, 1f, 0f);
#endif
        transform.Find("Handle").TryGetComponent(out handle);
    }

#if UNITY_EDITOR
    void Update()
    {
        Vector2 keyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); ;
        input.moveDirection = keyboard;
        Vector2 norm = new Vector2(keyboard.x / (rt.sizeDelta.x * 0.5f), keyboard.y / (rt.sizeDelta.y * 0.5f)) * sensitivity;
        norm = Vector2.ClampMagnitude(norm, 1f);
        handle.anchoredPosition = Vector2.Lerp(handle.anchoredPosition, norm * 100f * handleRange, 4f * Time.deltaTime);
    }
#endif

    public void OnPointerDown(PointerEventData eventData)
    {
        ctsRelease?.Cancel();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out center);
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        try
        {
            ctsRelease?.Cancel();
            ctsRelease?.Dispose();
        }
        catch
        {

        }
        ctsRelease = new CancellationTokenSource();
        var ctsCombine = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctsRelease.Token);
        Release(ctsCombine.Token).Forget();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) return;
        Vector2 delta = localPoint - center;
        Vector2 norm = new Vector2(delta.x / (rt.sizeDelta.x * 0.5f), delta.y / (rt.sizeDelta.y * 0.5f)) * sensitivity;
        norm = Vector2.ClampMagnitude(norm, 1f);
        input.moveDirection = norm;
        if (handle != null) handle.anchoredPosition = norm * handleRange;
    }
    CancellationTokenSource ctsRelease;
    async UniTask Release(CancellationToken token)
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
            await UniTask.DelayFrame(1, cancellationToken: token);
        }
        input.moveDirection = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}