using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class AbilityGravity : Ability
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
        ///////////////////
        FallLoop(cts.Token).Forget();
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
    Transform camTr;
    [SerializeField] float moveSpeedAir = 3.5f;
    [SerializeField] float rotateSpeedAir = 0.3f;
    [SerializeField] float groundCheckRadius = 0.3f;
    [SerializeField] float gravityAcc = -9.8f;
    [SerializeField] float gravityMaxSpeed = -20f;
    bool isInit;
    Player player;
    PlayerInput input;
    Rigidbody rb;
    Animator anim;
    float fallTime;
    float startTimeLand;
    Collider lastCollider;
    float gravitySpeed = 0f;
    Vector3 camForwardXZ;
    Vector3 camRightXZ;
    Vector3 moveDir;
    float slowAngle = 1f;
    float test;
    void Start()
    {
        camTr = Player.I.camTr;
        TryGetComponent(out rb);
        TryGetComponent(out player);
        player.TryGetComponent(out input);
        anim = GetComponentInChildren<Animator>();
        isInit = true;
        test = Time.time;
    }
    async UniTask FallLoop(CancellationToken token)
    {
        float landTime = 0.2f;
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken: token);
            if (!isInit) continue;
            if (anim == null)
            {
                anim = Player.I.anim;
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
                continue;
            }
            if (!IsGround())
                {
                    gravitySpeed += gravityAcc * Time.fixedDeltaTime;
                    gravitySpeed = Mathf.Clamp(gravitySpeed, gravityMaxSpeed, 0f);
                    if (input.moveDirection != Vector2.zero)
                    {
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
                        moveDir = input.moveDirection.x * camRightXZ + input.moveDirection.y * camForwardXZ;
                        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeedAir * Time.fixedDeltaTime);
                    }
                    else moveDir = Vector3.zero;
                    slowAngle = 1f - Vector3.Angle(transform.forward, moveDir) * 0.00277f;
                    //rb.MovePosition(transform.position + (moveSpeedAir * slowAngle * moveDir +  gravitySpeed * Vector3.up) * Time.fixedDeltaTime);
                    rb.linearVelocity = (moveSpeedAir * slowAngle * moveDir + gravitySpeed * Vector3.up);
                    if (player.state != "Fall" && player.state != "Land" && player.state != "Die")
                    {
                        //Debug.Log($"{player.state}->Fall");
                        anim.CrossFade("Fall", 0.15f);
                        player.state = "Fall";
                        player.DisableAbility<AbilityMove>("Fall");
                        fallTime = Time.time;
                    }
                }
                else
                {
                    if (player.state == "Fall")
                    {
                        //Debug.Log(Time.time - test);
                        //Debug.Log("Fall->Land");
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                        anim.CrossFade("Land", 0.15f);
                        player.state = "Land";
                        ParticleManager.I.PlayParticle("LandDust", transform.position, Quaternion.identity);
                        //rb.linearVelocity = 0.6f * rb.linearVelocity;
                        startTimeLand = Time.time;
                        fallTime = Time.time - fallTime;
                        if (fallTime > 0.2f)
                        {
                            SoundManager.I.PlaySFX("Land");
                        }
                        if (fallTime < 0.2f) landTime = 0f;
                        else if (fallTime < 1f) landTime = 0.2f + (fallTime * 0.1f);
                        else landTime = 0.4f;
                    }
                    if (player.state == "Land")
                    {
                        //fallTime시간이 클수록 Land시간이 길어지게 처리
                        //Debug.Log(Time.time - startTimeLand);
                        if (Time.time - startTimeLand > landTime)
                        {
                            //Debug.Log($"{player.state}->Idle");
                            anim.CrossFade("Idle", 0.3f);
                            player.state = "Idle";
                            player.EnableAbility<AbilityMove>("Fall");
                            //player.EnableAbility<AbilityJump>("Fall");
                            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                        }
                    }
                    if (gravitySpeed != 0) gravitySpeed = 0f;
                }
            if (player.isDead) continue;
        }
    }
    Collider[] collsGround = new Collider[30];
    bool IsGround()
    {
        if (transform.position.y <= -0.2)
        {
            player.isGround = true;
            return true;
        }

        int count = Physics.OverlapSphereNonAlloc(transform.position + (groundCheckRadius - 0.07f) * Vector3.up, groundCheckRadius * 0.9f, collsGround);
#if UNITY_EDITOR
        DebugExtension.DebugWireSphere(transform.position + (groundCheckRadius - 0.07f) * Vector3.up, groundCheckRadius * 0.9f);
#endif
        for (int i = 0; i < count; i++)
        {
            if (collsGround[i] == null) continue;
            if (collsGround[i].isTrigger) continue;
            if (collsGround[i].Root() == transform) continue;
            lastCollider = collsGround[i];
            player.isGround = true;
            return true;
        }
        player.isGround = false;
        return false;
    }
    public void Acceleration(Vector3 force, float time)
    {
        Acceleration_ut(force, time, cts.Token).Forget();
    }
    async UniTask Acceleration_ut(Vector3 force, float time, CancellationToken token)
    {
        float startTime = Time.time;
        while (!token.IsCancellationRequested && Time.time - startTime < time)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken: token);
            Vector3 lerp = Vector3.Lerp(force, 0.1f * force, (Time.time - startTime) / time);
            rb.linearVelocity = new Vector3(lerp.x, rb.linearVelocity.y, lerp.z);
            if (player.state == "Land" || player.state == "Land") break;
        }
    }




}
