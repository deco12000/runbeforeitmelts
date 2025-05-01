using UnityEngine;
using UnityEngine.Events;
public class EventEnablePlayer : Event
{
    public override UnityAction<EventData> action {get; set;}
}
