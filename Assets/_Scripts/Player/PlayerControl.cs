using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : Player
{
    List<IAblity> ablities;
    [SerializeField] List<AblityDisable> ablityDisables = new List<AblityDisable>();
    [SerializeField] List<AblityMultiply> ablityMultiplies = new List<AblityMultiply>();
    protected override void Awake()
    {
        base.Awake();
        pctrl = this;
        InitAblity();
    }

    void InitAblity()
    {
        ablities = GetComponents<IAblity>().ToList();
        ablityDisables.Clear();
        ablityMultiplies.Clear();
    }
    
    void Start()
    {
        EventHub.Instance.Register<EventDisablePlayer>(OnDisablePlayer);
        EventHub.Instance.Register<EventEnablePlayer>(OnEnablePlayer);
    }

    public void DisableAblity<T>(string reason) where T : IAblity
    {
        string TName = typeof(T).Name;
        int find = -1;
        for(int i=0; i<ablityDisables.Count; i++)
            if(ablityDisables[i].targetAbilityName == TName && ablityDisables[i].reason == reason)
            {
                find = i;
                break;
            }
        if(find != -1) return;

        AblityDisable disable = new AblityDisable(TName, reason);
        ablityDisables.Add(disable);
        for(int i=0; i<ablities.Count; i++)
            if(ablities[i].GetType().Name == TName)
            {
                ablities[i].enabled = false;
                break;
            }
    }

    void OnDisablePlayer(EventData data)
    {
        gameObject.SetActive(false);
        pui.gameObject.SetActive(false);
    }
    
    void OnEnablePlayer(EventData data)
    {
        gameObject.SetActive(true);
        pui.gameObject.SetActive(true);
    }



}


