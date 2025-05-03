using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class SFX : PoolBehaviour
{
    #region UniTask
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += () => UniTaskCancel();
        //Init();
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
    public AudioClip clip;
    public bool loop;
    public float length = -1;
    public float fadeInTime = 0.1f;
    public float fadeOutTime = 0.1f;
    AudioSource aus;
    float _len;












    
}
