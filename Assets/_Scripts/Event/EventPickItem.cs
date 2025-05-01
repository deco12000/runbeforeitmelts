using UnityEngine;
using UnityEngine.Events;
public class EventPickItem : Event
{
    public override UnityAction<EventData> action {get; set;}
}
public class EventPickItemData : EventData
{
    public override Transform owner {get; set;}
}
