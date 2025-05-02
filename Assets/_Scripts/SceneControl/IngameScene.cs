using UnityEngine;
public class InGameScene : MonoBehaviour
{
    void Start()
    {
        EventHub.Instance.Invoke<EventEnablePlayer>();
        Player.Instance.pcam.target = Player.Instance.transform;
        Player.Instance.pcam.StartFollow();
        EventHub.Instance.Invoke<EventScrollReady>();
        //아랫줄은 추후에 변경
        Player.Instance.pctrl.modelChanger.Change(0);
    }

}
