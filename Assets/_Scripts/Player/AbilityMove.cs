using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class AbilityMove : Ability
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
        MoveLoop(cts.Token).Forget();
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
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float rotateSpeed = 4f;
    bool isInit;
    Player player;
    PlayerInput input;
    Rigidbody rb;
    Animator anim;
    Vector3 camForwardXZ;
    Vector3 camRightXZ;
    Vector3 moveDir;
    float slowAngle = 1f;
    void Start()
    {
        camTr = Player.I.camTr;
        TryGetComponent(out rb);
        TryGetComponent(out player);
        player.TryGetComponent(out input);
        anim = GetComponentInChildren<Animator>();
        isInit = true;
    }
    async UniTask MoveLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken: token);
            if (!isInit) continue;
            if (Player.I.isDead) continue;
            if (anim == null)
            {
                anim = Player.I.anim;
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
                continue;
            }
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
                transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.fixedDeltaTime);
                slowAngle = 1f - Vector3.Angle(transform.forward, moveDir) * 0.00277f;
                //rb.MovePosition(transform.position += );
                //rb.linearVelocity += moveSpeed * slowAngle * moveDir.normalized * Time.fixedDeltaTime;
                rb.linearVelocity = moveSpeed * slowAngle * moveDir.normalized;
                if (player.state != "Move")
                {
                    //Debug.Log($"{player.state}->Move");
                    player.state = "Move";
                    anim.CrossFade("Move", 0.1f);
                    anim.SetFloat("Move", moveSpeed);
                }
            }
            else
            {
                if (player.state == "Move")
                {
                    //Debug.Log($"Move->Idle");
                    player.state = "Idle";
                    anim.CrossFade("Idle", 0.1f);
                }
            }
            if (player.isDead) continue;
        }
    }
}
