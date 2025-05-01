using UnityEngine;
using UnityEngine.Events;
public class EventScrollStart : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventScrollStartData : EventData
{
    public override Transform owner {get; set;}
}
