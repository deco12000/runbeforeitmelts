using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScene : MonoBehaviour
{
    IEnumerator Start()
    {
        //화면 페이드 인 들어갈 부분
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        SoundManager.Instance.PlayBGM("Track1",2.4f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
        //
    }



    IEnumerator NextScene()
    {
        SoundManager.Instance.PlaySFX("UIClickBubble1");
        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        SceneManager.LoadSceneAsync(2,LoadSceneMode.Single);
    }





}
