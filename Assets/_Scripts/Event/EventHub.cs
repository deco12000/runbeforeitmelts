using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EventHub : SingletonBehaviour<EventHub>
{
    protected override bool IsDontDestroy() => true;
    void OnEnable()
    {
        Init();
    }
    [SerializeField] List<Event> viewOnly = new List<Event>();
    void Init()
    {
        viewOnly = GetComponents<Event>().ToList();
    }

    




}
