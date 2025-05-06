using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerControl : Player
{
    [ReadOnlyInspector] public bool isGround = true;
    [ReadOnlyInspector] public float lastMoveSpeed;
    [HideInInspector] public Animator anim;
    List<IAblity> ablities;
    [SerializeField] List<DisableAblity> ablityDisables = new List<DisableAblity>();
    [SerializeField] List<MultiplyAblity> ablityMultiplies = new List<MultiplyAblity>();
    protected override void Awake()
    {
        base.Awake();
        ctrl = this;
        InitAblity();
        modelChanger = GetComponentInChildren<PlayerModelChange>();
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
    [HideInInspector] public PlayerModelChange modelChanger;

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

        DisableAblity disable = new DisableAblity(TName, reason);
        ablityDisables.Add(disable);
        for(int i=0; i<ablities.Count; i++)
            if(ablities[i].GetType().Name == TName)
            {
                ablities[i].enabled = false;
                break;
            }
    }

    public void EnableAblity<T>(string reason) where T : IAblity
    {
        string TName = typeof(T).Name;
        int find = -1;
        for(int i=0; i<ablityDisables.Count; i++)
            if(ablityDisables[i].targetAbilityName == TName && ablityDisables[i].reason == reason)
            {
                find = i;
                break;
            }
        if(find == -1) return;
        ablityDisables.RemoveAt(find);
        if(!ablityDisables.Any(x => x.targetAbilityName == TName))
        {
            for(int i=0; i<ablities.Count; i++)
            if(ablities[i].GetType().Name == TName)
            {
                ablities[i].enabled = true;
                break;
            }
        }

    }


    void OnDisablePlayer(EventData data)
    {
        gameObject.SetActive(false);
        ui.gameObject.SetActive(false);
    }
    
    void OnEnablePlayer(EventData data)
    {
        gameObject.SetActive(true);
        ui.gameObject.SetActive(true);
    }



}


