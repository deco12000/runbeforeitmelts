using UnityEngine;
using UnityEngine.Events;
public class EventOnHit : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventOnHitData : EventData
{
    public override Transform owner {get; set;}
}
