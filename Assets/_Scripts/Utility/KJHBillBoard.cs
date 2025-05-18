using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class KJHBillBoard : MonoBehaviour
{
    
    #region  UniTask
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += () => UniTaskCancel();
        Run(cts.Token).Forget();
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
    public bool Z_Flip;
    public bool isIsometric;
    Vector3 originalScale;
    public bool half_BillBoard = true;
    public bool ignore_Perspective = false;
    public float fixDistance = 7.35f;
    public bool half_Ignore_Perspective = false;
    Camera camMain;
    void Awake()
    {
        originalScale = transform.localScale;
    }
    async UniTask Run(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if(camMain == null || camMain.gameObject.activeSelf == false)
            {
                camMain = Camera.main;
                await UniTask.DelayFrame(20, cancellationToken: token);
                continue;
            }
            await UniTask.WaitUntil(() => camMain != null && camMain.gameObject.activeSelf, cancellationToken: token);
            Vector3 pos = camMain.transform.position;
            float distance = Vector3.Distance(pos, transform.position);
            if (distance > camMain.farClipPlane || distance < camMain.nearClipPlane) 
            { 
                continue; 
            }
            int a = (Z_Flip) ? -1 : 1;
            Vector3 vec = transform.forward;
            if (!isIsometric)
            {
                if (half_BillBoard)
                {
                    vec = a * new Vector3(0.9f * (pos.x - transform.position.x), 0.6f * (pos.y - transform.position.y), 0.9f * (pos.z - transform.position.z));
                }
                else
                {
                    vec = a * new Vector3(pos.x - transform.position.x, pos.y - transform.position.y, pos.z - transform.position.z);
                }
            }
            else
            {
                if (transform.localScale == Vector3.zero) { transform.localScale = originalScale; }
                vec = a * camMain.transform.forward;
            }
            transform.LookAt(vec,camMain.transform.up);
            if (!isIsometric)
            {
                if (ignore_Perspective && fixDistance > 0)
                {

                    float a0 = distance / fixDistance;

                    if (!half_Ignore_Perspective)
                    {
                        if (distance >= fixDistance)
                        {
                            transform.localScale = new Vector3(a0 * originalScale.x, a0 * originalScale.y, 1 * originalScale.z);
                        }
                        else
                        {
                            transform.localScale = new Vector3(a0 * originalScale.x, a0 * originalScale.y, 1 * originalScale.z);
                        }
                    }
                    else
                    {
                        if (distance >= fixDistance)
                        {
                            float a1 = (0.5f) + (0.5f * a0);
                            transform.localScale = new Vector3(a1 * originalScale.x, a1 * originalScale.y, 1 * originalScale.z);
                        }
                        else
                        {
                            transform.localScale = new Vector3(a0 * originalScale.x, a0 * originalScale.y, 1 * originalScale.z);
                        }
                    }
                }
            }
            await UniTask.DelayFrame(2, cancellationToken: token);
        }
    }

}
