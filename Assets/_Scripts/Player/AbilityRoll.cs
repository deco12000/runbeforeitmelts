using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class AbilityRoll : Ability
{
    #region Implement Setting
    protected override bool Enabled
    {
        get => this.enabled;
        set => this.enabled = value;
    }
    float multiplier;
    public override void UpdateMultiplier(float m) => multiplier = m;
    #endregion
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
        ///////////////
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

}
