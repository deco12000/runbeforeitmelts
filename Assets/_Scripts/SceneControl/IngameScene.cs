using UnityEngine;
public class IngameScene : MonoBehaviour
{
    void Start()
    {
        EventHub.Instance.Invoke<EventEnablePlayer>();
        Player.Instance.pcam.target = Player.Instance.transform;
        Player.Instance.pcam.StartFollow();
        EventHub.Instance.Invoke<EventScrollReady>();
    }

}
