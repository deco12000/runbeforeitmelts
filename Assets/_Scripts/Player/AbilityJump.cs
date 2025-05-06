using System.Collections;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AbilityJump : MonoBehaviour, IAblity
{
    #region IAblity Implement
    bool IAblity.enabled
    {
        get => this.enabled;
        set => this.enabled = value;
    }
    float multiplier;
    void IAblity.MultiplierUpdate(float total) => multiplier = total;
    #endregion
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        TryGetComponent(out rb);
        input = Player.Instance.input;
        anim = GetComponentInChildren<Animator>();
        if (input == null) input = Player.Instance.input;
        StartCoroutine("Init");
    }
    void OnDisable()
    {
        UniTaskCancel();
        if (input == null) input = Player.Instance.input;
        if (init)
        {
            input.OnJumpUp -= OnJumpUp;
            input.OnJumpDown -= OnJumpDown;
            input.OnJumpAutoRelease -= OnJumpAutoRelease;
            init = false;
        }
    }
    void OnDestroy() => UniTaskCancel();
    void UniTaskCancel()
    {
        try { cts?.Cancel(); cts?.Dispose(); } catch { }
        cts = null;
    }
    #endregion
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpDuration = 1f;
    [SerializeField] float turnSpeed = 5f;
    PlayerInput input;
    PlayerControl ctrl;
    Rigidbody rb;
    Animator anim;
    Transform cam;
    bool isJump = false;
    float startTime;
    bool condition = false;
    bool init;
    IEnumerator Init()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        if (init) yield break;
        if (input == null) input = Player.Instance.input;
        if (ctrl == null) ctrl = Player.Instance.ctrl;
        if (rb == null) ctrl.TryGetComponent(out rb);
        if (anim == null) anim = ctrl.GetComponentInChildren<Animator>();
        if (cam == null) cam = Player.Instance.cam.cam.transform;
        Player.Instance.ctrl.lastMoveSpeed = 5.5f;
        input.OnJumpUp += OnJumpUp;
        input.OnJumpAutoRelease += OnJumpAutoRelease;
        input.OnJumpDown += OnJumpDown;
        init = true;
    }
    void OnJumpDown()
    {
        if (isJump) return;
        if (Player.Instance.state != "Idle" && Player.Instance.state != "Land" && Player.Instance.state != "Move") return;
        if (!ctrl.isGround) return;
        if (!condition) condition = true;
    }
    void OnJumpAutoRelease(Vector2 dir, float time)
    {
        if (isJump) return;
        if (Player.Instance.state != "Idle" && Player.Instance.state != "Move") return;
        if (!ctrl.isGround) return;
        if (!condition) return;
        cts?.Cancel();
        cts = new CancellationTokenSource();
        Jump(dir, time, cts.Token).Forget();
    }
    void OnJumpUp(Vector2 dir, float time)
    {
        if (isJump) return;
        if (Player.Instance.state != "Idle" && Player.Instance.state != "Move") return;
        if (!ctrl.isGround) return;
        if (!condition) return;
        cts?.Cancel();
        cts = new CancellationTokenSource();
        Jump(dir, time, cts.Token).Forget();
    }
    // 점프 로직
    async UniTask Jump(Vector2 dir, float iTime, CancellationToken token)
    {
        isJump = true;
        iTime *= 3f;
        iTime = Mathf.Clamp(iTime, 0.3f, 0.6f); // 최소 점프시간, 최대 점프시간
        bool isSlide = dir.y < -0.5f;
        startTime = Time.time;
        Vector3 camForwardXZ = cam.forward;
        camForwardXZ.y = 0f;
        Vector3 camRightXZ = Quaternion.Euler(0f,90f,0f) * cam.forward;
        if(camForwardXZ == Vector3.zero)
        {
            camRightXZ = cam.transform.right;
            camRightXZ.y = 0f;
            camForwardXZ = Quaternion.Euler(0f,-90f,0f) * camRightXZ;
        }
        Vector3 inputDir = (dir.x * 0.66f * camRightXZ +  2f * Vector3.up + dir.y * camForwardXZ).normalized;
        if (!isSlide)
        {
            Player.Instance.prevState = Player.Instance.state;
            Player.Instance.state = "JumpStart";
            ctrl.anim.SetTrigger("JumpStart");
            while (!token.IsCancellationRequested && Time.time - startTime < jumpDuration * iTime)
            {
                // 초기 점프 입력방향으로 인한 영향
                float lerp = Mathf.Lerp(2f * jumpForce, 0f, (Time.time - startTime)/jumpDuration);
                rb.AddForce(lerp * 100f * Time.deltaTime * inputDir);
                await UniTask.DelayFrame(1, cancellationToken: token);
                Vector3 _forward = inputDir;
                _forward.y = 0f;
                transform.forward = Vector3.Lerp(transform.forward, _forward, turnSpeed * (Time.time - startTime)/jumpDuration);
            }
            Player.Instance.prevState = Player.Instance.state;
            Player.Instance.state = "Fall";
            rb.useGravity = true;
        }
        else
        {
            // iTime = Mathf.Clamp(iTime * 2f,0.7f,1f);
            // Player.Instance.prevState = Player.Instance.state;
            // Player.Instance.state = "Slide";
            // ctrl.anim.SetTrigger("Slide");
            // while (!token.IsCancellationRequested && Time.time - startTime < jumpDuration * iTime)
            // {
            //     Slide(inputDir,jumpDuration * iTime * 1.3f);
            //     await UniTask.DelayFrame(1, cancellationToken: token);
            // }
        }
        isJump = false;
        condition = false;
    }


    // 슬라이드 (옵션)
    void Slide(Vector2 dir, float t)
    {
        dir.y = 0f;
        Vector3 slideDir = transform.forward;
        slideDir.y = 0f;
        float slidePower = 1.8f;
        rb.AddForce(dir * slidePower * 100f * Time.deltaTime, ForceMode.Impulse);
    }
}
