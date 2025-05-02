using UnityEngine;


public class TestGameScene : MonoBehaviour
{
    [Header("테스트 게임 플레이 0~2")]
    [SerializeField] int characterSelect = 0;


    void Start()
    {
        EventHub.Instance.Invoke<EventEnablePlayer>();
        Player.Instance.pcam.target = Player.Instance.transform;
        Player.Instance.pcam.StartFollow();
        EventHub.Instance.Invoke<EventScrollReady>();
        //아랫줄은 추후에 변경
        Player.Instance.pctrl.modelChanger.Change(characterSelect);
    }

    

}
