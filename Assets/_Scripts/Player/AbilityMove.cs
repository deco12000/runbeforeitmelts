using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class AbilityMove : MonoBehaviour, IAblity
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
        input = Player.Instance.input;
        cam = Camera.main;
        Move(cts.Token).Forget();
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

    [SerializeField] float moveSpeed = 4f;
    PlayerInput input;
    Animator anim;
    Camera cam;
    Vector3 camForwardXZ;
    Vector3 camRightXZ;
    Vector3 moveDir;
    Rigidbody rb;
    float speed = 0f;
    async UniTask Move(CancellationToken token)
    {
        await UniTask.DelayFrame(10, cancellationToken: token);
        while (!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);
            if (input == null) input = Player.Instance.input;
            if (cam == null) cam = Camera.main;
            if(anim == null) anim = GetComponentInChildren<Animator>();
            if (input == null || cam == null || !cam.enabled || !cam.gameObject.activeInHierarchy)
            {
                await UniTask.DelayFrame(50, cancellationToken: token);
                continue;
            }
            if(input.moveDirection == Vector2.zero)
            {
                speed = Mathf.Lerp(speed, 0f, 10f * Time.deltaTime);
                if(anim != null) anim.SetFloat("Move", speed);
                if(Player.Instance.state == "Move")
                {
                    Player.Instance.prevState = "Move";
                    Player.Instance.state = "Idle";
                }
                continue;
            }
            if(Player.Instance.state != "Move")
            {
                Player.Instance.prevState = Player.Instance.state;
                Player.Instance.state = "Move";
            }
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                anim.Play("Move");
            }
            camForwardXZ = cam.transform.forward;
            camForwardXZ.y = 0;
            camRightXZ = cam.transform.right;
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
            MoveDirection();
            speed = moveSpeed * angleSlow;
            anim.SetFloat("Move", speed);
            rb.MovePosition(transform.position += speed * moveDir * Time.deltaTime);
            Player.Instance.ctrl.lastMoveSpeed = moveSpeed;
        }
    }
    void MoveDirection()
    {
        transform.forward = Vector3.Slerp(transform.forward, moveDir, 3.3f * Time.deltaTime);
        float angle = Vector3.Angle(transform.forward, moveDir);
        angleSlow = 1f - angle * 0.00277f;
    }
    float angleSlow = 1f;


}
