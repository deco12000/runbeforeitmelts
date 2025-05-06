using System.Collections;
using UnityEngine;
public class TitleScene : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] FirebaseDB firebaseDB;
    IEnumerator Start()
    {
        GameManager.Instance.FadeIn();
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        SoundManager.Instance.PlayBGM("Track1",2.4f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
        StartCoroutine(firebaseDB.LoadHighScoreUsers());
        //
    }



    IEnumerator NextScene()
    {
        SoundManager.Instance.PlaySFX("UIClickBubble1");
        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        GameManager.Instance.LoadSceneAsync(2);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        canvas.gameObject.SetActive(false);
        

    }





}
