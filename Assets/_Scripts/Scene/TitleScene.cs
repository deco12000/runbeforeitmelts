using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScene : MonoBehaviour
{
    IEnumerator Start()
    {
        GameManager.Instance.FadeIn();
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        SoundManager.Instance.PlayBGM("Track1",2.4f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
        //
    }



    IEnumerator NextScene()
    {
        SoundManager.Instance.PlaySFX("UIClickBubble1");
        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        GameManager.Instance.LoadSceneAsync(2);
    }





}
