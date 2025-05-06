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

    public AudioSource aus;

    public void PlayDespawn(AudioClip clip, float vol, float time)
    {
        PlayDespawn_ut(clip, vol, time, cts.Token).Forget();
    }
    async UniTask PlayDespawn_ut(AudioClip clip, float vol, float time, CancellationToken token)
    {
        await UniTask.Delay(2, ignoreTimeScale: true, cancellationToken:token);
        aus.loop = false;
        aus.clip = clip;
        aus.volume = vol;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        if (!aus.enabled) aus.enabled = true;
        await UniTask.Delay(2, ignoreTimeScale: true, cancellationToken:token);
        aus.pitch = Random.Range(0.95f,1.05f);
        aus.Play();
        await UniTask.Delay((int)(1000f * (time + 0.2f)),ignoreTimeScale: true , cancellationToken:token);
        base.DeSpawn();
    }
}
