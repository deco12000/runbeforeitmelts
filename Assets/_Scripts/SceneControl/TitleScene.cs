using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TitleScene : MonoBehaviour
{
    IEnumerator Start()
    {
        // 화면 페이드 인
        SceneChanger.I?.FadeIn();
        SoundManager.I?.PlayBGM("Track1",2.4f);
        // 배경 살짝식 흔들리기
        background.DOLocalMoveX(background.transform.localPosition.x + 15f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        titleText.DOLocalMoveY(titleText.transform.localPosition.y + 15f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        yield return YieldInstructionCache.WaitForSeconds(1f);
    }
    [SerializeField] Transform background;
    [SerializeField] Canvas canvas;
    [SerializeField] Transform titleText;
    IEnumerator NextScene()
    {
        SoundManager.I?.PlaySFX("UIClickBubble1");
        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        canvas.gameObject.SetActive(false);
        SceneChanger.I?.LoadSceneAsync(2);
    }


}
