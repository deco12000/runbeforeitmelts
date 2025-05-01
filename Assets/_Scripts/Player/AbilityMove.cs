using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class AbilityMove : MonoBehaviour , IAblity
{
    #region IAblity Implement
    bool IAblity.enabled
    {
        get => this.enabled;
        set => this.enabled = value;
    }
    float multiplier;
    void IAblity.MultiplierUpdate(float total)
    {
        multiplier = total;
    }
    #endregion

    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += () => UniTaskCancel();
        // ↓ Init ↓
        TryGetComponent(out rb);
        input = Player.Instance.pinput;
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
    Camera cam;
    Vector3 camForwardXZ;
    Vector3 camRightXZ;
    Vector3 moveDir;
    Rigidbody rb;

    async UniTask Move(CancellationToken token)
    {
        while(!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);

            if(input == null) input = Player.Instance.pinput;
            if(cam == null) cam = Camera.main;

            if(input == null || cam == null || !cam.enabled || !cam.gameObject.activeInHierarchy)
            {
                await UniTask.DelayFrame(50, cancellationToken: token);
                continue;
            }

            camForwardXZ = cam.transform.forward;
            camForwardXZ.y = 0;
            camRightXZ = cam.transform.right;
            camRightXZ.y = 0;
            if(camForwardXZ == Vector3.zero)
            {
                camForwardXZ = Quaternion.Euler(0f,-90f,0f) * camRightXZ;
            }
            else if(camRightXZ == Vector3.zero)
            {
                camRightXZ = Quaternion.Euler(0f,90f,0f) * camForwardXZ;
            }
            camForwardXZ.Normalize();
            camRightXZ.Normalize();
            

            moveDir = input.direction.x * camRightXZ + input.direction.y * camForwardXZ;
            rb.MovePosition(transform.position += moveSpeed * moveDir * Time.deltaTime);

        }
    }
    

}
