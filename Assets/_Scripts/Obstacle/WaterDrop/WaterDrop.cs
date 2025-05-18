using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class WaterDrop : Obstacle
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
        //크기 랜덤?
        float size = Random.Range(minSize, maxSize);
        transform.localScale = size * Vector3.one;
        startTime = Time.time;
        isGround = false;
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
    [SerializeField] float minSize = 0.3f;
    [SerializeField] float maxSize = 1f;
    [SerializeField] float _damage = 10f;
    [SerializeField] float startTime;
    protected override float damage => _damage;
    public bool isGround = false;
    bool isCooltime;
    public void OnTriggetStayFromModel(Collider other)
    {
        if (Player.I == null) return;
        if (EventHub.I == null) return;
        if (!other.CompareTag("Player")) return;
        if (Player.I.tempInvincible) return;
        if (Player.I.isDead) return;
        if (isCooltime) return;
        AttackData data = new AttackData(transform, Player.I?.transform, _damage);
        EventHub.I?.Invoke<EventAttack>(data);
        ParticleManager.I.PlayParticle("WaterHit", 0.5f * (transform.position + Player.I.transform.position), Quaternion.identity);
        Player.I.transform.DOScale(Player.I.transform.localScale.x - (_damage * 0.00625f), 0.295f);
        isCooltime = true;
        CoolTime(cts.Token).Forget();
    }
    async UniTask CoolTime(CancellationToken token)
    {
        await UniTask.Delay(Random.Range(900, 1400), cancellationToken: token);
        isCooltime = false;
    }
    public void DespawnTime()
    {
        DespawnTime_ut(cts.Token).Forget();
    }
    async UniTask DespawnTime_ut(CancellationToken token)
    {
        await UniTask.Delay(25000, cancellationToken: token);
        base.DeSpawn();
    }





}
