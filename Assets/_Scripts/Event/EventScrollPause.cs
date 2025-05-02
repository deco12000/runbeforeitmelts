using UnityEngine;
using UnityEngine.Events;
public class EventScrollPause : Event
{
    public override UnityAction<EventData> action {get; set;}
}
