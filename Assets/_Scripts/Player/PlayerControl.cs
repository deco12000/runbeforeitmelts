using UnityEngine;
public class PlayerControl : MonoBehaviour
{
    void Awake()
    {
        PlayerGroup.Instance.pctrl = this;
    }
    void Start()
    {
        EventHub.Instance.Register<EventDisablePlayer>(OnDisablePlayer);
        EventHub.Instance.Register<EventEnablePlayer>(OnEnablePlayer);
    }
    void OnDisablePlayer(EventData data)
    {
        gameObject.SetActive(false);
        PlayerGroup.Instance.pui.gameObject.SetActive(false);
    }
    void OnEnablePlayer(EventData data)
    {
        gameObject.SetActive(true);
        PlayerGroup.Instance.pui.gameObject.SetActive(true);
    }

    







}
