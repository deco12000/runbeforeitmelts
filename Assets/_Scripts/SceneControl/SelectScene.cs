using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SelectScene : MonoBehaviour
{

    IEnumerator Start()
    {
        yield return null;
        Player.Instance.pinput.OnMouseDown0 += OnMouseDown0;
        Player.Instance.pcam.SetCamera(new Vector3(-23, 52, 14.8f), Quaternion.Euler(28, 178, 0));
        Player.Instance.pcam.cam.fieldOfView = 60f;
        blink = notice.DOFade(0f,1f).SetEase(ease:Ease.OutSine).SetLoops(-1,LoopType.Yoyo);
        //화면 페이드인 들어갈 부분
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        Debug.Log("캐릭터를 선택해 주세요");
        setup = true;
    }

    [SerializeField] GameObject select = null;
    bool isStartButton = false;
    public GameObject[] spotlights = new GameObject[3];
    public Text notice;
    bool setup = false;
    Tween blink;
    public Button button;
    Tween bounce1;
    Tween bounce2;

    void OnMouseDown0(GameObject go)
    {
        if(!setup) return;
        if(go == null) return;
        if(go.name != "0" && go.name != "1" && go.name != "2") return;
        select = go;
        Debug.Log($"{go} 선택");
        blink.Kill();
        bounce1.Kill();
        bounce2.Kill();
        notice.color = new Color(1f,1f,1f);
        Player.Instance.pcam.SetCamera(go.transform, true, true, 4f);
        spotlights[0].gameObject.SetActive(false);
        spotlights[1].gameObject.SetActive(false);
        spotlights[2].gameObject.SetActive(false);
        switch (go.name)
        {
            case "0":
                spotlights[0].gameObject.SetActive(true);
                notice.text = "각설탕맨을 선택했습니다.";
                GameManager.Instance.select = 0;
                break;
            case "1":
                spotlights[1].gameObject.SetActive(true);
                notice.text = "티슈맨을 선택했습니다.";
                GameManager.Instance.select = 1;
                break;
            case "2":
                spotlights[2].gameObject.SetActive(true);
                notice.text = "비누를 선택했습니다.";
                GameManager.Instance.select = 2;
                break;
        }
        button.gameObject.SetActive(true);
        notice.transform.localScale = 0.7f * Vector3.one;
        button.transform.localScale = 0.7f * Vector3.one;
        bounce1 = notice.transform.DOScale(1f,0.7f).SetEase(ease: Ease.OutBounce);
        bounce2 = button.transform.DOScale(1f,0.7f).SetEase(ease: Ease.OutBounce);
    }

    public IEnumerator NextScene()
    {
        //화면 페이드 아웃 들어갈 부분
        
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        Player.Instance.pinput.OnMouseDown0 -= OnMouseDown0; // 이벤트 구독해제 꼭 해주세요
        SceneManager.LoadScene(3);
    }

    public IEnumerator PrevScene()
    {
        //화면 페이드 아웃 들어갈 부분

        prevClick = prevRt.DOScale(0.7f,0.1f).SetEase(ease: Ease.OutQuart);
        prevClick.OnComplete(() => prevRt.DOScale(1f,0.2f).SetEase(ease: Ease.OutBounce));

        yield return YieldInstructionCache.WaitForSeconds(0.7f);
        Player.Instance.pinput.OnMouseDown0 -= OnMouseDown0; // 이벤트 구독해제 꼭 해주세요
        SceneManager.LoadScene(1);
    }
    Tween prevClick;
    public RectTransform prevRt;























}
