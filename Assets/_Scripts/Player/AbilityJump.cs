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
        input = Player.Instance.pinput;
        anim = GetComponentInChildren<Animator>();
        JumpLoop(cts.Token).Forget();
    }

    void OnDisable() => UniTaskCancel();
    void OnDestroy() => UniTaskCancel();

    void UniTaskCancel()
    {
        try { cts?.Cancel(); cts?.Dispose(); } catch { }
        cts = null;
    }
    #endregion

    [SerializeField] float jumpForce = 5f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundCheckDistance = 0.2f;

    PlayerInput input;
    Rigidbody rb;
    Animator anim;

    bool isJumping = false;

    async UniTask JumpLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);

            if (input == null) continue;
            if (isJumping) continue;

            // 점프 입력이 들어온 경우
            if (input.isJump && !isJumping && IsGrounded())
            {
                isJumping = true;
                Debug.Log("a");

                // 점프 실행
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce , ForceMode.Impulse);
                anim?.SetTrigger("Jump");

                // 공중에 떠 있는 동안 대기
                await UniTask.WaitUntil(() => IsGrounded(), cancellationToken: token);

                // 착지 후 다음 점프 가능
                isJumping = false;
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }
}
