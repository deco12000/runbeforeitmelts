using System.Collections;
using UnityEngine;
public class InGameScene : MonoBehaviour
{
    PlayerCamera pcam;
    PlayerControl pctrl;
    Camera cam;
    IEnumerator Start()
    {
        // 화면 페이드인 들어갈 부분

        pcam = Player.Instance.pcam;
        pctrl =  Player.Instance.pctrl;
        cam = pcam.cam;
        pctrl.modelChanger.Change(GameManager.Instance.select);
        pctrl.transform.position = new Vector3(0f,24.17f,0f);
        pcam.transform.position = new Vector3(0f,24.17f,0f);
        cam.transform.localPosition = new Vector3(0f,11f,-18.5f);
        cam.transform.localRotation = Quaternion.Euler(22,0,0);
        cam.fieldOfView = 40f;
        EventHub.Instance.Invoke<EventEnablePlayer>();
        pcam.target = Player.Instance.transform;
        pctrl.DisableAblity<AbilityMove>("Ready");
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        EventHub.Instance.Invoke<EventScrollReady>();
        pcam.StartFollow();

        Debug.Log("3");

        yield return YieldInstructionCache.WaitForSeconds(1f);


        Debug.Log("2");



        yield return YieldInstructionCache.WaitForSeconds(1f);


        Debug.Log("1");


        yield return YieldInstructionCache.WaitForSeconds(1f);

        
        Debug.Log("출발");


        pctrl.EnableAblity<AbilityMove>("Ready");


        

    }

}
