using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
public class AbilityGravity : MonoBehaviour, IAblity
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
        Application.quitting += () => UniTaskCancel();
        // ↓ Init ↓
        TryGetComponent(out rb);
        Fall(cts.Token).Forget();
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

            Debug.Log(e);
        }
        cts = null;
    }
    #endregion
    Rigidbody rb;
    Player player;
    PlayerInput input;
    Transform camTr;
    bool isFall;
    bool isLand;
    float startTime;
    Collider lastColl;
    public float airControlSpeed = 3.5f;
    public float airControlRotate = 0.3f;
    async UniTask Fall(CancellationToken token)
    {
        await UniTask.DelayFrame(10, cancellationToken: token);
        while (!token.IsCancellationRequested)
        {
            if (player == null) player = Player.Instance;
            if (input == null) input = Player.Instance.input;
            if (camTr == null) camTr = Player.Instance.cam.cam.transform;
            await UniTask.DelayFrame(1, cancellationToken: token);
            if (IsGround())
            {
                if (player.state == "Fall" && Time.time - startTime > 0.065f)
                {
                    if (isFall) isFall = false;
                    if (player.ctrl.anim) player.ctrl.anim.SetTrigger("Land");
                    player.prevState = "Fall";
                    player.state = "Land";
                    await UniTask.Delay(250, cancellationToken: token);
                    player.prevState = "Land";
                    player.state = "Idle";
                    player.ctrl.EnableAblity<AbilityMove>("Fall");
                    if(input.moveDirection != Vector2.zero)
                    {
                        player.ctrl.anim.SetFloat("Move", 2f);
                    }
                }
                // 버그 픽스
                else if(player.state == "Idle")
                {
                    if(player.ctrl.anim.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                    {
                        player.ctrl.anim.Play("Land");
                        player.prevState = "Fall";
                        player.state = "Land";
                    }
                }
                continue;
            }
            bool isDontGravityState = false;
            isDontGravityState |= player.state == "JumpStart";
            isDontGravityState |= player.state == "TissueSkill";
            if (isDontGravityState)
            {
                if (isFall) isFall = false;
                if (rb.useGravity) rb.useGravity = false;
                continue;
            }
            if (!isFall)
            {
                isFall = true;
                startTime = Time.time;
                if (lastColl)
                {
                    Vector3 forceDir;
                    Vector3 vector1 = transform.position - lastColl.ClosestPoint(transform.position);
                    Vector3 vector2 = transform.position - lastColl.transform.position;
                    vector1.y = 0f;
                    vector2.y = 0f;
                    forceDir = (0.7f * vector1 + 0.3f * vector2).normalized;
                    if (player.prevState != "JumpStart") rb.AddForce(forceDir * 3f, ForceMode.VelocityChange);
                }
                player.ctrl.DisableAblity<AbilityMove>("Fall");
                if (!rb.useGravity) rb.useGravity = true;
                if (player.state != "Fall")
                {
                    player.prevState = player.state;
                    player.state = "Fall";
                }
                if (player.ctrl.anim) player.ctrl.anim.SetTrigger("Fall");
            }
            else
            {
                Vector3 camForwardXZ = camTr.forward;
                camForwardXZ.y = 0f;
                Vector3 camRightXZ = Quaternion.Euler(0f, 90f, 0f) * camForwardXZ;
                if(camForwardXZ == Vector3.zero)
                {
                    camRightXZ = camTr.right;
                    camRightXZ.y = 0f;
                    camForwardXZ = Quaternion.Euler(0f,-90f,0f) * camRightXZ;
                }
                Vector3 controlDir = input.moveDirection.y * camForwardXZ + input.moveDirection.x * camRightXZ;
                controlDir.Normalize();
                rb.MovePosition(transform.position + airControlSpeed * controlDir * Time.deltaTime);
                transform.forward = Vector3.Lerp(transform.forward, controlDir, airControlRotate * Time.deltaTime);

            }
        }
    }
    bool IsGround()
    {
        for (int i = 0; i < 30; i++) _colls[i] = null;
        Physics.OverlapSphereNonAlloc(transform.position + 0.22f * Vector3.up, 0.23f, _colls);
        for (int i = 0; i < 30; i++)
        {
            if (_colls[i] == null) continue;
            if (_colls[i].isTrigger) continue;
            if (_colls[i].Root() == transform) continue;
            lastColl = _colls[i];
            player.ctrl.isGround = true;
            return true;
        }
        player.ctrl.isGround = false;
        return false;
    }
    Collider[] _colls = new Collider[30];






}
