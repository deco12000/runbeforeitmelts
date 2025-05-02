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
    public float smoothTime = 0.25f;
    Transform camTr;
    Vector3 _velocity;
    Transform _lastTarget;
    float _threshold = 0.1f;


    void Awake()
    {
        Player.Instance.pcam = this;
        camTr = transform.GetChild(0);
    }

    public void SetCamera(Vector3 start, Vector3 forward, bool setActive = true, bool smooth = false)
    {
        camTr.gameObject.SetActive(setActive);
        camTr.position = start;
        camTr.forward = forward;
    }
    
    public void SetCamera(Vector3 start, Quaternion euler, bool setActive = true, bool smooth = false)
    {
        camTr.gameObject.SetActive(setActive);
        camTr.position = start;
        camTr.rotation = euler;
    }
    public void SetCamera(Transform target, bool setActive = true, bool smooth = false)
    {
        camTr.gameObject.SetActive(setActive);
        camTr.forward = target.position - transform.position;
    }

    public void StartFollow()
    {
        StopFollow();
        cts = new CancellationTokenSource();
        Follow(cts.Token).Forget();
    }

    public void StopFollow()
    {
        try
        {
            cts?.Cancel();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        cts = null;
    }

    public async UniTask Follow(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 타겟이 null일 경우 기다림
            await UniTask.WaitUntil(() => target != null, cancellationToken: token);

            // 타겟이 바뀌었으면 속도 초기화
            if (target != _lastTarget)
            {
                _velocity = Vector3.zero;
                _lastTarget = target;
            }

            // 타겟이 존재하는 동안 추적
            while (target != null && !token.IsCancellationRequested)
            {
                Vector3 current = transform.position;
                Vector3 targetPos = target.position;

                // 너무 가까우면 이동 생략 (덜덜거림 방지)
                if (Vector3.Distance(current, targetPos) > _threshold)
                {
                    transform.position = Vector3.SmoothDamp(current, targetPos, ref _velocity, smoothTime);
                }

                await UniTask.Yield(); // 매 프레임 대기
            }
        }
    }
    
    


}
