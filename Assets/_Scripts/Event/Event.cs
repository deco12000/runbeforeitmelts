using UnityEngine;
using UnityEngine.Events;
public abstract class Event : MonoBehaviour
{
    private UnityAction<EventData> _action = (e) => { };
    public virtual UnityAction<EventData> action
    {
        get => _action;
        set => _action = value ?? ((e) => { });
    }
}
public abstract class EventData
{
    
}
