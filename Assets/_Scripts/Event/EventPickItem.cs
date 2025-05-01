using UnityEngine;
using UnityEngine.Events;
public class EventPickItem : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class PickItemData : EventData
{
    public Transform owner;
}
