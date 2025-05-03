using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Particle : PoolBehaviour
{
    #region UniTask
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += () => UniTaskCancel();
        Init();
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
    public bool loop;
    public float legnth = -1;
    ParticleSystem ps;
    float _len;
    void Init()
    {
        _len = 0f;
        ps = GetComponentInChildren<ParticleSystem>();
        if (ps == null) return;
        if (ps.main.loop)
            _len = 0;
        else if (legnth == -1)
            _len = ps.main.duration + 0.02f;
        else
            _len = legnth;
        if (_len > 0)
            AutoHide(cts.Token).Forget();
        ps.Play();
    }
    async UniTask AutoHide(CancellationToken token)
    {
        await UniTask.Delay(Mathf.FloorToInt(_len * 1000f), cancellationToken: token);
        ps.Stop();
        base.DeSpawn();
    }
}
