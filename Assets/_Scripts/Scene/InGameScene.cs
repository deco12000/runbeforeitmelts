using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class InGameScene : MonoBehaviour
{
    PlayerControl ctrl;
    Camera cam;
    [SerializeField] Image[] buttons;
    IEnumerator Start()
    {
        // 화면 페이드인 들어갈 부분
        yield return null;
        foreach(Image img in buttons) img.raycastTarget = false;
        ctrl =  Player.Instance.ctrl;
        cam = Player.Instance.cam.cam;
        ctrl.modelChanger.Change(GameManager.Instance.select);
        ctrl.DisableAblity<AbilityMove>("Ready");
        ctrl.DisableAblity<AbilityJump>("Ready");
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        SoundManager.Instance.PlayBGM("CodeRedAlert",2.5f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);





        Debug.Log("3");
        yield return YieldInstructionCache.WaitForSeconds(1f);
        Debug.Log("2");
        yield return YieldInstructionCache.WaitForSeconds(1f);
        Debug.Log("1");
        yield return YieldInstructionCache.WaitForSeconds(1f);
        Debug.Log("출발");
        ctrl.EnableAblity<AbilityMove>("Ready");
        ctrl.EnableAblity<AbilityJump>("Ready");
        EventHub.Instance.Invoke<EventScrollStart>();
        foreach(Image img in buttons) img.raycastTarget = true;



        //
    }
    


}
