using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class InGameScene : MonoBehaviour
{
    [SerializeField] Canvas playerCanvas;
    [SerializeField] Canvas setting;
    Text count;
    IEnumerator Start()
    {
        count = setting.transform.Find("Count").GetComponent<Text>();
        // 화면 페이드인 들어갈 부분
        SceneChanger.I?.FadeIn();
        SoundManager.I.ausAmbience.gameObject.SetActive(true);
        SoundManager.I.ausAmbience.volume = 0f;
        SoundManager.I.ausAmbience.Play();
        SoundManager.I.ausAmbience.DOFade(0.56f, 3f);
        // 3,2,1 카운트 보기 전까진 조이스틱 UI 조작 못하게
        playerCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        Player.I?.ChangeModel(GameManager.I.select);
        // 3,2,1 카운트 보기 전까진 이동,점프 금지처리
        Player.I?.DisableAbility<AbilityMove>("321count");
        Player.I?.DisableAbility<AbilityJump>("321count");
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        SoundManager.I?.PlayBGM("JeremyBlack-Daydreamer", 2.4f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);

        count.gameObject.SetActive(true);
        Debug.Log("3");
        count.text = "3";
        SoundManager.I.PlaySFX("Count");
        count.transform.localScale = 0.6f * Vector3.one;
        count.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
        yield return YieldInstructionCache.WaitForSeconds(1f);

        Debug.Log("2");
        count.text = "2";
        SoundManager.I.PlaySFX("Count");
        count.transform.localScale = 0.6f * Vector3.one;
        count.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
        yield return YieldInstructionCache.WaitForSeconds(1f);

        Debug.Log("1");
        count.text = "1";
        SoundManager.I.PlaySFX("Count");
        count.transform.localScale = 0.6f * Vector3.one;
        count.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
        playerCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        yield return YieldInstructionCache.WaitForSeconds(1f);

        Debug.Log("출발");
        count.text = "Start";
        SoundManager.I.PlaySFX("Start");
        count.transform.localScale = 0.6f * Vector3.one;
        count.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
        Player.I?.EnableAbility<AbilityMove>("321count");
        Player.I?.EnableAbility<AbilityJump>("321count");
        EventHub.I?.Invoke<EventScrollStart>();

        yield return YieldInstructionCache.WaitForSeconds(1f);
        count.transform.DOScale(0f, 1.8f).OnComplete(() =>
        { 
            count.transform.localScale = 1f * Vector3.one;
            count.gameObject.SetActive(false);
        });

        
    }

    public void ReStart()
    {
        SoundManager.I.ausAmbience.gameObject.SetActive(false);
        playerCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        setting.GetComponent<GraphicRaycaster>().enabled = false;
        SceneChanger.I.LoadSceneAsync(2);

    }

    


}
