using UnityEngine;
using UnityEngine.Events;
public class EventAttack : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventAttackData : EventData
{
    public override Transform owner {get; set;}
}

