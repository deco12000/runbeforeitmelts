using UnityEngine;
using UnityEngine.Events;
public class EventHit : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class HitData : EventData
{
    public Transform attacker;
    public Transform target;
    public float damage;
}
