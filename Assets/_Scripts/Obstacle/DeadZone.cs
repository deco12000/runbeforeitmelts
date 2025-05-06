using UnityEngine;
public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AttackData attackData = new AttackData(transform, Player.Instance.ctrl.transform, 100f);
            EventHub.Instance.Invoke<EventAttack>(attackData);
        }
    }
}
