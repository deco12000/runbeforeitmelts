using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class EffectSprite : PoolBehaviour
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

    [SerializeField] Sprite[] sprites;
    Image image;
    void Awake()
    {
        TryGetComponent(out image);
    }
    public void Play(float time)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        Play_ut(time, cts.Token).Forget();
    }
    async UniTask Play_ut(float time, CancellationToken token)
    {
        float wait = time/sprites.Length;
        //image.material.color = new Color(1f, 1f, 1f, 1f);
        //image.material.DOFade(0f, time);
        for (int i = 0; i < sprites.Length; i++)
        {
            image.sprite = sprites[i];
            await UniTask.Delay((int)(1000f * wait), ignoreTimeScale: true, cancellationToken: token);
        }
        base.DeSpawn();
    }
}
