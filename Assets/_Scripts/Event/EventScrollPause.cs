using UnityEngine;
using UnityEngine.Events;
public class EventScrollPause : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventScrollPauseData : EventData
{
    public override Transform owner {get; set;}
}
