using UnityEngine;
using UnityEngine.Events;
public class EventScrollStart : Event
{
    public override UnityAction<EventData> action {get; set;}
}
