using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class PlayerHealth : MonoBehaviour
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
    bool isCooltime = false;
    Animator anim;
    public float maxHP = 100;
    public float currHP = 100;
    public float maxSP = 100;
    public float currSP = 100;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        EventHub.I.Register<EventAttack>(OnAttack);
        EventHub.I.Register<EventGetItem>(OnGetItem);
    }
    void OnGetItem(EventData ed)
    {
        if (Player.I.isDead) return;
        GetItemData d = ed as GetItemData;
        if (d.Name == "Heal")
        {
            currHP += 25f;
            currHP = Mathf.Clamp(currHP, 0f, 100f);

        }
    }
    void OnAttack(EventData _ed)
    {
        if (Player.I.isDead) return;
        AttackData ed = _ed as AttackData;
        if (ed == null) return;
        if (ed.target != transform) return;
        currHP -= ed.damage;
        currHP = Mathf.Clamp(currHP, 0f, 100f);
        //Debug.Log(ed.damage);
        //Debug.Log(ed.attacker);
        // 플레이어가 공격에 맞았을때 아랫줄에 작성
        if (currHP <= 0f && !Player.I.isDead)
        {
            //플레이어가 죽었을때 아랫줄에 작성
            Player.I.isDead = true;
            EventHub.I.Invoke<EventDie>(new DieData(transform));
            Player.I.DisableAbility<AbilityJump>("Die");
            Player.I.DisableAbility<AbilityMove>("Die");
            anim.CrossFade("Die", 0.2f);
            Player.I.state = "Die";
            EventHub.I.Invoke<EventScrollPause>();
            DieAnimation(cts.Token).Forget();
#if UNITY_ANDROID
            Handheld.Vibrate();
#elif UNITY_IOS
            Handheld.Vibrate();
#endif
        }
        Player.I.tempInvincible = true;
        TemporaryInvincible(cts.Token).Forget();
    }
    async UniTask TemporaryInvincible(CancellationToken token)
    {
        await UniTask.Delay(Random.Range(300, 600), cancellationToken: token);
        Player.I.tempInvincible = false;
    }
    async UniTask DieAnimation(CancellationToken token)
    {
        var anim = Player.I.transform.GetComponentInChildren<Animator>();
        await UniTask.Delay(500, cancellationToken: token);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            anim.SetTrigger("Die");
            anim.Play("Die");
            Player.I.state = "Die";
        }
    }




}
