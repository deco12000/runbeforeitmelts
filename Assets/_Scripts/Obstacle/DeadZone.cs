using UnityEngine;
public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.I.isDead) return;
            AttackData attackData = new AttackData(transform, Player.I.transform, 100f);
            EventHub.I.Invoke<EventAttack>(attackData);
            ParticleManager.I.PlayParticle("Hit", Player.I.transform.position + 0.5f * Vector3.up, Quaternion.identity);
            SoundManager.I.PlaySFX("Hit");
        }
    }
}
