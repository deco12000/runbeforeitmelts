using UnityEngine;
using UnityEngine.Events;

public abstract class Event : MonoBehaviour
{
    public abstract UnityAction<EventData> action { get; protected set; }
}

public abstract class EventData
{
    public abstract Transform owner {get; protected set;}
}
