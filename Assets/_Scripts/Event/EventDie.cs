using UnityEngine;
using UnityEngine.Events;
public class EventDie : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class DieData : EventData
{
    public Transform owner;
}
