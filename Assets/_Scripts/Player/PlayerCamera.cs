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
    Vector3 _Velocity0;

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

    async UniTask Follow(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);
            await UniTask.WaitUntil(() => target != null , cancellationToken: token);
            Vector3 smoothPos = Vector3.SmoothDamp(transform.position, target.position, ref _Velocity0, smoothTime);
            transform.position = smoothPos;
        }
    }
    
    


}
