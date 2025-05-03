using UnityEngine;
using UnityEngine.Events;
public class EventAttack : Event
{
    public override UnityAction<EventData> action {get; set;}
}

