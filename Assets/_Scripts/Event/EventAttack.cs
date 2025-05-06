using UnityEngine;
using UnityEngine.Events;
public class EventAttack : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class AttackData : EventData
{
    public Transform attacker;
    public Transform target;
    public float damage;
    public AttackData(Transform attacker, Transform target, float damage)
    {
        this.attacker = attacker;
        this.target = target;
        this.damage = damage;
    }
}
