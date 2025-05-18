using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class EventHub : SingletonBehaviour<EventHub>
{
    protected override bool IsDontDestroy() => false;
    void OnEnable()
    {
        Init();
    }
    [SerializeField] List<Event> viewOnly = new List<Event>();
    Dictionary<string,Event> dictionary = new Dictionary<string, Event>();
    void Init()
    {
        viewOnly = GetComponents<Event>().ToList();
        dictionary.Clear();
        for(int i=0; i<viewOnly.Count; i++)
            dictionary.Add(viewOnly[i].GetType().ToString(), viewOnly[i]);
    }
    public void Register<T>(UnityAction<EventData> method) where T : Event
    {
        if(dictionary.ContainsKey(typeof(T).Name))
            dictionary[typeof(T).Name].action += method;
    }
    public void UnRegister<T>(UnityAction<EventData> method) where T : Event
    {
        if(dictionary.ContainsKey(typeof(T).Name))
            dictionary[typeof(T).Name].action -= method;
    }
    public void Invoke<T>(EventData eventData = null) where T : Event
    {
        if(dictionary.ContainsKey(typeof(T).Name))
            dictionary[typeof(T).Name].action?.Invoke(eventData);
    }
}
