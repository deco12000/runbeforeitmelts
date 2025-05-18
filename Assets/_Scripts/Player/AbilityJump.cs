using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
public class AbilityJump : Ability
{
    #region Implement Setting
    protected override bool Enabled
    {
        get => this.enabled;
        set => this.enabled = value;
    }
    float multiplier;
    public override void UpdateMultiplier(float m) => multiplier = m;
    #endregion
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
        ///////////////
        Init();
    }
    void OnDisable() { UniTaskCancel(); UnInit(); }
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
    [SerializeField] float jumpForce = 5f; // 키를 누르는 순간 가해지는 초기 Impulse 힘
    [SerializeField] float jumpDuration = 0.5f; // 추가 힘을 가하는 최대 시간
    [SerializeField] float maxInputHoldTime = 0.5f; // 수직 점프 힘이 최대로 증가하는 키 누름 시간
    Player player;
    Animator anim;
    AbilityGravity gravity;
    PlayerInput input;
    Rigidbody rb;
    Transform camTr;
    Vector3 camForwardXZ;
    Vector3 camRightXZ;
    void Awake()
    {
        TryGetComponent(out player);
        anim = GetComponentInChildren<Animator>();
        TryGetComponent(out input);
        TryGetComponent(out rb);
        TryGetComponent(out gravity);
    }
    void Init()
    {
        input.OnJumpDown += OnJumpDown;
        input.OnJumpUp += OnJumpUp;
        camTr = Player.I.camTr;
    }
    void UnInit()
    {
        input.OnJumpDown -= OnJumpDown;
        input.OnJumpUp -= OnJumpUp;
    }
    void OnJumpDown()
    {
        if (anim == null)
        {
            anim = Player.I.anim;
        }
        if (player.state == "JumpStart") return;
        if (player.state == "Fall") return;
        if (player.state == "Land") return;
        if (player.state == "Die") return;
        if (!player.isGround) return;
        anim.CrossFade("JumpStart", 0.15f);
        player.state = "JumpStart";
        //iputDir.y = Mathf.Clamp01(iputDir.y);
        JumpStart(cts.Token).Forget();
        SoundManager.I.PlaySFX("Jump");
        inputTime = 0f;
        inputDir = Vector2.zero;
    }
    void OnJumpUp(Vector2 inputDir, float inputTime)
    {
        this.inputDir = inputDir;
        this.inputTime = inputTime;
        //Debug.Log($"{inputDir} , {inputTime}");
    }
    Vector2 inputDir;
    float inputTime;
    async UniTask JumpStart(CancellationToken token)
    {
        float startTime = Time.time;
        player.DisableAbility<AbilityGravity>("JumpStart");
        player.DisableAbility<AbilityMove>("JumpStart");
        ParticleManager.I.PlayParticle("Jump", transform.position + 0.4f * Vector3.up, Quaternion.identity);
        if (camTr != null) camTr = Player.I.camTr;
        Vector3 lastVector = Vector3.zero;
        bool once = false;
        while (!token.IsCancellationRequested && Time.time - startTime < jumpDuration)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken: token);
            Vector3 jumpDirection;
            Vector3 _forward;
            float lerp;
            Vector2 moveInputForward = input.moveDirection;
            camForwardXZ = camTr.forward;
            camForwardXZ.y = 0;
            camRightXZ = camTr.right;
            camRightXZ.y = 0;
            if (camForwardXZ == Vector3.zero)
            {
                camForwardXZ = Quaternion.Euler(0f, -90f, 0f) * camRightXZ;
            }
            else if (camRightXZ == Vector3.zero)
            {
                camRightXZ = Quaternion.Euler(0f, 90f, 0f) * camForwardXZ;
            }
            camForwardXZ.Normalize();
            camRightXZ.Normalize();
            Vector3 moveInputForwardWorld = camForwardXZ * moveInputForward.y + camRightXZ * moveInputForward.x;
            //아직 inputDir 과 inputTime 이 결정되지 않은 경우에는 다음 실행
            if (inputTime == 0)
            {
                jumpDirection =
                Vector3.up
                + 0.3f * (input.jumpDirection.y * camForwardXZ + input.jumpDirection.x * camRightXZ)
                + 0.2f * moveInputForwardWorld
                + 0.1f * transform.forward;
                lerp = Mathf.Lerp(0.3f * jumpForce, jumpForce, (Time.time - startTime) / jumpDuration);
            }
            else
            {
                float holdFactor = Mathf.Clamp(inputTime / maxInputHoldTime, 0.13f, 1f);
                if (Time.time - startTime > jumpDuration * holdFactor)
                {
                    lastVector = rb.linearVelocity;
                    lastVector.y = 0f;
                    break;
                }
                jumpDirection
                = 0.4f * Vector3.ClampMagnitude(inputDir.x * camRightXZ + inputDir.y * camForwardXZ, 1f)
                + 0.6f * Vector3.up
                + 0.1f * transform.forward
                + 0.2f * moveInputForwardWorld;
                lerp = Mathf.Lerp(jumpForce, 0.5f * jumpForce * holdFactor, (Time.time - startTime) / jumpDuration * holdFactor);
                if (!once)
                {
                    once = true;
                    Vector3 jumpStartDirectionXZ = jumpDirection;
                    jumpStartDirectionXZ.y = 0f;
                    Vector3 _left = Quaternion.Euler(0f, -90f, 0f) * jumpStartDirectionXZ;
                    _left.Normalize();
                    RaycastHit hit;
                    RaycastHit hit2;
                    float angle = Vector3.Angle(jumpStartDirectionXZ, jumpDirection);
                    if (Physics.Raycast(transform.position - 0.02f * transform.forward, jumpDirection, out hit, 3f, ~(1 << LayerMask.NameToLayer("Player"))))
                    {
                        Vector3 dir = Vector3.zero;
                        bool test = false;
                        for (int i = 1; i < 25; i++)
                        {
                            dir = Quaternion.AngleAxis(angle + 58f * i * 0.05f, _left) * jumpStartDirectionXZ;
                            if (Physics.Raycast(transform.position - 0.02f * transform.forward, dir, out hit2, 6f, ~(1 << LayerMask.NameToLayer("Player"))))
                            {
                                Debug.DrawRay(transform.position - 0.02f * transform.forward, 6f * dir, Color.white, 10f, false);
                                continue;
                            }
                            else
                            {
                                dir = Quaternion.AngleAxis(angle + 58f * (i + 5) * 0.05f, _left) * jumpStartDirectionXZ;
                                if (Vector3.Angle(dir, jumpStartDirectionXZ) >= 90) { break; }
                                Debug.DrawRay(transform.position - 0.02f * transform.forward, 6f * dir, Color.blue, 10f, false);
                                test = true;
                                dir += 0.3f * Vector3.up;
                                break;
                            }
                        }
                        if (test) jumpDirection = dir;
                        // Debug.DrawRay(transform.position - 0.5f * transform.forward, _left, Color.blue, 10f, false);
                        // Debug.DrawRay(transform.position - 0.5f * transform.forward, 6f * jumpDirection, Color.red, 10f, false);
                        // Debug.DrawRay(transform.position - 0.5f * transform.forward, 6f * jumpStartDirectionXZ, Color.red, 10f, false);
                    }
                    else
                    {
                        // Debug.DrawRay(transform.position - 0.5f * transform.forward, _left, Color.blue, 10f, false);
                        // Debug.DrawRay(transform.position - 0.5f * transform.forward, 6f * jumpDirection, Color.green, 10f, false);
                        // Debug.DrawRay(transform.position - 0.5f * transform.forward, 6f * jumpStartDirectionXZ, Color.green, 10f, false);
                    }
                }
            }
            _forward = jumpDirection;
            _forward.y = 0f;
            transform.forward = Vector3.Lerp(transform.forward, _forward, 0.6f * (Time.time - startTime) / jumpDuration);
            rb.linearVelocity = lerp * jumpDirection;
            lastVector = rb.linearVelocity;
            lastVector.y = 0f;
        }
        //Debug.Log("점프 종료");
        //Debug.Log($"inputTime : {inputTime}");
        //Debug.Log($"fixedDelta : {Time.fixedDeltaTime}");
        player.EnableAbility<AbilityGravity>("JumpStart");
        player.EnableAbility<AbilityMove>("JumpStart");
        player.state = "Idle";
        gravity.Acceleration(lastVector, 5f);
    }




}
