using UnityEngine;
public class Heal1 : Item
{
    public bool _isUsed;
    protected override bool isUsed => _isUsed;
    public int _probablity = 7;
    public override int probablity => _probablity;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.I.isDead) return;
            if (_isUsed) return;
            _isUsed = true;
            EventHub.I.Invoke<EventGetItem>(new GetItemData("Heal1"));
            Particle p = ParticleManager.I.PlayParticle("Heal", Player.I.transform.position, Quaternion.identity);
            p.transform.localScale = 0.5f * Vector3.one;
            SoundManager.I.PlaySFX("Heal");
            base.DeSpawn();
        }
    }
}
