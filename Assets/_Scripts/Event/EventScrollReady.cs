using UnityEngine;
using UnityEngine.Events;
public class EventScrollReady : Event
{
    public override UnityAction<EventData> action {get; set;}
}
