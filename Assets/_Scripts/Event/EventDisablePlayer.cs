using UnityEngine;
using UnityEngine.Events;
public class EventDisablePlayer : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventDisablePlayerData : EventData
{
    public override Transform owner {get; set;}
}
