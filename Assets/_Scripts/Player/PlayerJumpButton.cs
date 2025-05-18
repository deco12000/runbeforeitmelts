using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
public class PlayerJumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region UniTask Setting
    private CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
    }
    void OnDisable()
    {
        UniTaskCancel();
    }

    void OnDestroy()
    {
        UniTaskCancel();
    }
    void UniTaskCancel()
    {
        // 메인 CTS 취소 및 정리
        try { cts?.Cancel(); } catch (System.Exception e) { Debug.Log(e); }
        try { cts?.Dispose(); } catch (System.Exception e) { Debug.Log(e); }
        try { _activePressTaskCts?.Cancel(); } catch (System.Exception e) { Debug.Log($"PressTask CTS Cancel Exception: {e}"); }
        try { _activePressTaskCts?.Dispose(); } catch (System.Exception e) { Debug.Log($"PressTask CTS Dispose Exception: {e}"); }
        try { _activeReleaseTaskCts?.Cancel(); } catch (System.Exception e) { Debug.Log($"ReleaseTask CTS Cancel Exception: {e}"); }
        try { _activeReleaseTaskCts?.Dispose(); } catch (System.Exception e) { Debug.Log($"ReleaseTask CTS Dispose Exception: {e}"); }
        cts = null;
        _activePressTaskCts = null;
        _activeReleaseTaskCts = null;
    }
    #endregion
    [Header("UI Elements")]
    public RectTransform handle; // 조이스틱 핸들 RectTransform
    public Image pressedImage; // 버튼이 눌렸을 때 표시할 이미지
    [Header("Joystick Settings")]
    [SerializeField] float sensitivity = 1f; // 드래그 민감도
    [SerializeField] float handleRange = 10f; // 핸들의 최대 이동 반경 (수직)
    [SerializeField] float handleRangeX = 5f; // 핸들의 최대 이동 반경 (수평, 드래그 정규화에도 사용)
    [Header("Jump Button Timings")]
    [SerializeField] float autoReleaseDelay = 0.23f; // 버튼 자동 해제까지의 시간
    [SerializeField] float smoothReleaseDuration = 0.3f; // 손 뗀 후 핸들/방향 부드러운 복귀 애니메이션 시간 (기존 코드 기반 0.3f/3f)
    // Private fields for internal state and components
    private RectTransform rt; // 버튼 배경 RectTransform
    private Image img; // 버튼 배경 Image
    private PlayerInput input; // PlayerInput 컴포넌트 (Player.I에 있다고 가정)
    private Vector2 center = Vector2.zero; // 터치 시작 시 로컬 좌표 기준점
    private Vector2 lastDirection = Vector2.zero; // 손 떼기 전 마지막 드래그 방향
    private float startTime; // 버튼 누르기 시작한 시간
    private bool isPressed = false; // 현재 버튼이 눌린 상태인지 (UI 이벤트 핸들러에서 관리)

    // 특정 UniTask 작업을 관리하기 위한 CancellationTokenSource들
    private CancellationTokenSource _activePressTaskCts; // 버튼 누름 상태 및 자동 해제 타이머 관리 작업용
    private CancellationTokenSource _activeReleaseTaskCts; // 핸들/방향 부드러운 복귀 애니메이션 작업용
    void Awake()
    {
        TryGetComponent(out img);
        TryGetComponent(out rt);
        if (Player.I != null)
        {
            Player.I.TryGetComponent(out input);
        }
        else
        {
             Debug.LogError("Player.I is null or does not have PlayerInput component. PlayerJumpButton requires Player.I with PlayerInput.");
        }
        if (transform.Find("Handle") != null)
        {
             transform.Find("Handle").TryGetComponent(out handle);
        }
        else
        {
            Debug.LogError("PlayerJumpButton requires a child object named 'Handle' with a RectTransform.");
        }
#if UNITY_EDITOR
        if (img != null) img.color = new Color(1f, 1f, 1f, 0.02f); // Editor에서는 약간 보이게
#else
        if (img != null) img.color = new Color(1f, 1f, 1f, 0f); // Build에서는 투명하게
#endif
        if (pressedImage != null) pressedImage.gameObject.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        try
        {
        _activePressTaskCts?.Cancel(); // 이전 누름 상태/자동 해제 작업 취소
        _activePressTaskCts?.Dispose(); // 해당 CTS 정리
        }
        catch
        {

        }
        _activePressTaskCts = null; // 참조 해제

        try
        {
        _activeReleaseTaskCts?.Cancel(); // 이전 부드러운 복귀 애니메이션 작업 취소
        _activeReleaseTaskCts?.Dispose(); // 해당 CTS 정리
        }
        catch
        {
            
        }
        _activeReleaseTaskCts = null; // 참조 해제
        isPressed = true; // 버튼 눌림 상태 플래그 설정
        startTime = Time.time; // 누르기 시작한 시간 기록
        if (pressedImage != null) pressedImage.gameObject.SetActive(true);
        if (rt != null) // RectTransform 유효성 확인
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out center);
        }
        OnDrag(eventData); // 초기 드래그 정보를 처리하여 direction 및 handle 위치 설정
        input?.OnJumpDown?.Invoke();
        _activePressTaskCts = new CancellationTokenSource();
        var linkedPressToken = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, _activePressTaskCts.Token);
        HandlePressAndReleaseLifecycle(linkedPressToken.Token).Forget(); // 비동기 실행 (완료 대기 안 함)
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return; // 이미 '안 눌린' 상태라면 더 이상 처리하지 않음
        isPressed = false;
    }    public void OnDrag(PointerEventData eventData)
    {
        if (!isPressed) return; // 눌린 상태가 아니면 드래그 처리 안 함
        if (rt == null || !RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return; // 변환 실패 시 리턴
        Vector2 delta = localPoint - center;
        float normY = Mathf.Clamp(delta.y / handleRange * sensitivity, -0.1f, 1f);
        float normX = Mathf.Clamp(delta.x / handleRangeX * sensitivity * 0.1f, -1f, 1f);
        Vector2 direction = new Vector2(normX, normY);
        if(input != null)
             input.jumpDirection = direction;
        lastDirection = direction; // 손 뗄 때 사용할 마지막 방향을 저장
        if (handle != null)
            handle.anchoredPosition = new Vector2(normX * handleRangeX, normY * handleRange); // 계산된 norm 값과 핸들 반경을 사용하여 위치 설정
    }
    private async UniTask HandlePressAndReleaseLifecycle(CancellationToken token)
    {
        bool wasAutoReleased = false;
        try
        {
            float timer = 0f; // 자동 해제 타이머
            while (isPressed && timer < autoReleaseDelay && !token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                timer += Time.deltaTime;
            }
            if (!token.IsCancellationRequested)
            {
                if (timer >= autoReleaseDelay && isPressed)
                {
                    wasAutoReleased = true; // 자동 해제 플래그 설정
                    isPressed = false; // 상태를 '안 눌림'으로 변경합니다.
                }
                if (pressedImage != null) pressedImage.gameObject.SetActive(false);
                float heldTime = wasAutoReleased ? autoReleaseDelay : (Time.time - startTime);
                input?.OnJumpUp?.Invoke(lastDirection, heldTime);
                 _activeReleaseTaskCts?.Cancel();
                 _activeReleaseTaskCts?.Dispose();
                 _activeReleaseTaskCts = null; // 참조 해제
                _activeReleaseTaskCts = new CancellationTokenSource();
                var linkedReleaseToken = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, _activeReleaseTaskCts.Token);
                SmoothReleaseAnimation(linkedReleaseToken.Token).Forget(); // 비동기 실행
            }
             else
             {
             }
        }
        catch
        {

        }
    }
    // 핸들 및 입력 방향을 0으로 부드럽게 복귀시키는 애니메이션 작업
    private async UniTask SmoothReleaseAnimation(CancellationToken token)
    {
        Vector2 startDir = input != null ? input.jumpDirection : Vector2.zero;
        Vector2 startPos = handle != null ? handle.anchoredPosition : Vector2.zero;
        float timer = 0f;
        try
        {
            while (timer < smoothReleaseDuration && !token.IsCancellationRequested)
            {
                float lerpFactor = timer / smoothReleaseDuration;
                if (input != null)
                     input.jumpDirection = Vector2.Lerp(startDir, Vector2.zero, lerpFactor);
                if (handle != null)
                     handle.anchoredPosition = Vector2.Lerp(startPos, Vector2.zero, lerpFactor);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                timer += Time.deltaTime;
            }
            if (!token.IsCancellationRequested)
            {
                if (input != null)
                     input.jumpDirection = Vector2.zero;
                if (handle != null)
                     handle.anchoredPosition = Vector2.zero;
                //Debug.Log("부드러운 복귀 애니메이션 완료.");
            }
            else
            {

            }
        }
        catch (System.Exception e)
        {
        }
    }
}