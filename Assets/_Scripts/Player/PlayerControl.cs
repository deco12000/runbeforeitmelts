using UnityEngine;
public class PlayerControl : PlayerGroup
{
    protected override void Awake()
    {
        pctrl = this;
    }
    void Start()
    {
        EventHub.Instance.Register<EventDisablePlayer>(OnDisablePlayer);
        EventHub.Instance.Register<EventEnablePlayer>(OnDisablePlayer);
    }
    void OnDisablePlayer(EventData data)
    {
        gameObject.SetActive(false);
        Debug.Log("2" + pui);
        pui.gameObject.SetActive(false);
    }
    void OnEnablePlayer(EventData data)
    {
        gameObject.SetActive(true);
        pui.gameObject.SetActive(true);

    }







}
