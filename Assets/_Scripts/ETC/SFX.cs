using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class SFX : PoolBehaviour
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
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

            Debug.Log(e.Message);
        }
        cts = null;
    }
    #endregion
    public AudioSource aus;
    public void Play(AudioClip clip, float vol, float time, float spatial)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        Play_ut(clip, vol, time, spatial, cts.Token).Forget();
    }
    async UniTask Play_ut(AudioClip clip, float vol, float time, float spatial, CancellationToken token)
    {
        await UniTask.Delay(2, ignoreTimeScale: true, cancellationToken:token);
        aus.loop = false;
        aus.clip = clip;
        aus.volume = vol;
        aus.spatialBlend = spatial;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        if (!aus.enabled) aus.enabled = true;
        await UniTask.Delay(2, ignoreTimeScale: true, cancellationToken:token);
        aus.pitch = Random.Range(0.97f,1.03f);
        aus.Play();
        await UniTask.Delay((int)(1000f * (time + 0.1f)),ignoreTimeScale: true , cancellationToken:token);
        base.DeSpawn();
    }
}
