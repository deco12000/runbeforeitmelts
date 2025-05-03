using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
public class EffectSprite : PoolBehaviour
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
    public List<Sprite> sprites = new List<Sprite>();
    public int fps = 10;
    Image img;
    void Init()
    {
        TryGetComponent(out img);
        img.sprite = sprites[0];
        Play(cts.Token).Forget();
    }
    async UniTask Play(CancellationToken token)
    {
        for(int i=0; i<sprites.Count; i++)
        {
            img.sprite = sprites[i];
            await UniTask.Delay((int)(1000 * (1f/fps)), true, cancellationToken:token);
        }
        base.DeSpawn();
    }
}
