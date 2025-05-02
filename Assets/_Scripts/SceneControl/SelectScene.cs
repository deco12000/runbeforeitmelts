using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SelectScene : MonoBehaviour
{

    IEnumerator Start()
    {
        Player.Instance.pcam.SetCamera(new Vector3(-23, 52, 14.8f), Quaternion.Euler(28, 178, 0));
        Player.Instance.pcam.cam.fieldOfView = 60f;
        blink = notice.DOFade(0f,1f).SetEase(ease:Ease.OutSine).SetLoops(-1,LoopType.Yoyo);

        //화면 페이드인 들어갈 부분

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        Debug.Log("캐릭터를 선택해 주세요");
        Player.Instance.pinput.OnMouseDown0 += OnMouseDown0;
        
    }

    [SerializeField] GameObject select = null;
    bool isStartButton = false;
    [SerializeField] GameObject[] spotlights;
    [SerializeField] Text notice;
    Tween blink;
    [SerializeField] Button button;
    Tween bounce1;
    Tween bounce2;

    void OnMouseDown0(GameObject go)
    {
        if(go == null) return;
        if(go.name != "0" && go.name != "1" && go.name != "2") return;
        select = go;
        Debug.Log($"{go} 선택");
        blink.Kill();
        bounce1.Kill();
        bounce2.Kill();
        notice.color = new Color(1f,1f,1f);
        Player.Instance.pcam.SetCamera(go.transform, true, true, 4f);
        spotlights[0].SetActive(false);
        spotlights[1].SetActive(false);
        spotlights[2].SetActive(false);
        switch (go.name)
        {
            case "0":
                spotlights[0].SetActive(true);
                notice.text = "각설탕맨을 선택했습니다.";
                GameManager.Instance.select = 0;
                break;
            case "1":
                spotlights[1].SetActive(true);
                notice.text = "티슈맨을 선택했습니다.";
                GameManager.Instance.select = 1;
                break;
            case "2":
                spotlights[2].SetActive(true);
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
        SceneManager.LoadScene(2);
    }






















}
