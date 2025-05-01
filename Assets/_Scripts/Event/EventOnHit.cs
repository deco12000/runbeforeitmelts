using UnityEngine;
using UnityEngine.Events;
public class EventOnHit : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class OnHitData : EventData
{
    public Transform attacker;
    public Transform target;
    public float damage;
    
}
