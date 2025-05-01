using UnityEngine;
using UnityEngine.Events;
public class EventAttack : Event
{
    public override UnityAction<EventData> action {get; protected set;}
}
public class EventAttackData : EventData
{
    public override Transform owner {get; protected set;}
}