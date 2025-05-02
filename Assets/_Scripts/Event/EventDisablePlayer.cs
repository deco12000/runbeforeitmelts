using UnityEngine;
using UnityEngine.Events;
public class EventDisablePlayer : Event
{
    public override UnityAction<EventData> action {get; set;}
}