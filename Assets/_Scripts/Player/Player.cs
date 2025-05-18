using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Player : SingletonBehaviour<Player>
{
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
    protected override bool IsDontDestroy() => true;
    public bool isDead = false;
    public bool isGround;
    public string state;
    public float size = 1f;
    public bool tempInvincible;
    public Transform camTr;
    public Animator anim;
    public void ChangeModel(int index)
    {

    }
    public void DisableAbility<T>(string reason) where T : Ability
    {
        string TName = typeof(T).Name;
        int find = currentDisables.FindIndex(o => o.AbilityName == TName && o.reason == reason);
        if (find != -1) return;
        Disable d = new Disable(TName, reason);
        currentDisables.Add(d);
        for (int i = 0; i < ablities.Length; i++)
            if (ablities[i].GetType().Name == TName) { ablities[i].enabled = false; return; }
    }
    public void EnableAbility<T>(string reason) where T : Ability
    {
        string TName = typeof(T).Name;
        int find = currentDisables.FindIndex(o => o.AbilityName == TName && o.reason == reason);
        if (find == -1) return;
        currentDisables.RemoveAt(find);
        find = currentDisables.FindIndex(o => o.AbilityName == TName);
        if(find == -1)
            for (int i = 0; i < ablities.Length; i++)
                if (ablities[i].GetType().Name == TName) { ablities[i].enabled = true; break; }
    }
    public void MultiflyAblity<T>(string reason, float smoothTime) where T : Ability
    {

    }
    public void UndoMultiflyAblity<T>(string reason, float smoothTime) where T : Ability
    {

    }
    protected override void Awake()
    {
        base.Awake();
        ablities = GetComponents<Ability>();
    }
    Ability[] ablities;
    [SerializeField] List<Disable> currentDisables = new List<Disable>();
    [SerializeField] List<Multiflier> currentMultifliers = new List<Multiflier>();
    [System.Serializable]
    public struct Disable
    {
        public string AbilityName;
        public string reason;
        public Disable(string AbilityName, string reason)
        {
            this.AbilityName = AbilityName;
            this.reason = reason;
        }
    }
    [System.Serializable]
    public struct Multiflier
    {
        public string AbilityName;
        public float multiflier;
        public string reason;
    }
}
