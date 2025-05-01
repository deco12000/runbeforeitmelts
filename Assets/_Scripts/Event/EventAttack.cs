using UnityEngine;
using UnityEngine.Events;
public class EventAttack : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class HitData : EventData
{
    public Transform owner;
    public Transform target;
}

