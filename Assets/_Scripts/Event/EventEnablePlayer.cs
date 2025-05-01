using UnityEngine;
using UnityEngine.Events;
public class EventEnablePlayer : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventEnablePlayerData : EventData
{
    public override Transform owner {get; set;}
}

