using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class PlayerCamera : MonoBehaviour
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += () => UniTaskCancel();
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
    
    public Transform target;
    public Camera cam;
    Transform camTr;
    void Awake()
    {
        Player.Instance.cam = this;
        camTr = transform.GetChild(0);
        camTr.TryGetComponent(out cam);
    }
    Vector3 playerFollowPos;
    Vector3 trackFollowLook;
    public void Start()
    {
        PlayerFollow(cts.Token).Forget();
        TrackFollow(cts.Token).Forget();
    }
    void Update()
    {
        FinalSetting();
    }
    public async UniTask PlayerFollow(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (target != null)
            {
                Vector3 targetPos = target.position;
                // 너무 가까우면 이동 생략
                if (Vector3.Distance(playerFollowPos, targetPos) > 0.2f)
                {
                    playerFollowPos = Vector3.Slerp(playerFollowPos, targetPos, 3f * Time.deltaTime);
                }
            }
            await UniTask.DelayFrame(1, cancellationToken: token);
        }
    }
    [HideInInspector] public Vector2Int selectPath;
    [HideInInspector] public Vector3[,] camPivotLRots;
    [HideInInspector] public Vector3 trackForward;
    public async UniTask TrackFollow(CancellationToken token)
    {
        trackFollowLook = transform.forward;
        while (!token.IsCancellationRequested)
        {
            if (camPivotLRots != null)
            {
                Vector3 targetLook = Quaternion.Euler(camPivotLRots[selectPath.x, selectPath.y]) * trackForward;
                trackFollowLook = Vector3.Slerp(trackFollowLook, targetLook, 0.6f * Time.deltaTime);
            }
            await UniTask.DelayFrame(1, cancellationToken: token);
        }
    }
    // Camera의 Parent Transform 은 1.플레이어팔로우, 2.트랙스크롤, 3.플레이어의 특정동작, 4. 컷씬의 영향을 받게 만든다.
    // 각각의 가중치는 여러번 해보면서 조정한다.
    public void FinalSetting()
    {
        transform.position = playerFollowPos;
        transform.forward = trackFollowLook;
    }

    // Camera의 본인의 자체Transform 은 Breath Sway 와 Hit Shake 상황에서만 영향을 받게 만든다.
    





    
    


}
