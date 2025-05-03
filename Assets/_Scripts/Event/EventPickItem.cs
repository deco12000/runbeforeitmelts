using UnityEngine;
using UnityEngine.Events;
public class EventGetItem : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class GetItemData : EventData
{
    public Transform owner;
}
