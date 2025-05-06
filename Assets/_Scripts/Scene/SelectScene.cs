using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class SelectScene : MonoBehaviour
{
    [SerializeField] GameObject select = null;
    public GameObject[] spotlights = new GameObject[3];
    public Text notice;
    bool setup = false;
    Tween blink;
    public Button button;
    Tween bounce1;
    Tween bounce2;
    Camera cam;
    IEnumerator Start()
    {
        GameManager.Instance.FadeIn();
        cam = Camera.main;
        notice.gameObject.SetActive(false);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        SoundManager.Instance.PlayBGM("Track5",2.4f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
        notice.gameObject.SetActive(true);
        notice.color = new Color(notice.color.r, notice.color.g, notice.color.b, 0f);
        blink = notice.DOFade(1f, 1f).SetEase(ease: Ease.OutSine).SetLoops(-1, LoopType.Yoyo);
        Debug.Log("캐릭터를 선택해 주세요");
        setup = true;
    }
    void Update()
    {
        if (!setup) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider);
                OnMouseDown0(hit.collider.gameObject);
            }
        }
    }
    Tween camTween1;
    void OnMouseDown0(GameObject go)
    {
        if (!setup) return;
        if (go == null) return;
        if (go.name != "0" && go.name != "1" && go.name != "2") return;
        select = go;
        Debug.Log($"{go} 선택");
        blink.Kill();
        bounce1.Kill();
        bounce2.Kill();
        notice.color = new Color(1f, 1f, 1f);
        camTween1?.Kill();
        camTween1 = cam.transform.DOLookAt(go.transform.position, 1.7f).SetEase(ease: Ease.OutExpo);
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
        SoundManager.Instance.PlaySFX("UIClickSharp1");
        button.gameObject.SetActive(true);
        notice.transform.localScale = 0.7f * Vector3.one;
        button.transform.localScale = 0.7f * Vector3.one;
        bounce1 = notice.transform.DOScale(1f, 0.7f).SetEase(ease: Ease.OutBounce);
        bounce2 = button.transform.DOScale(1f, 0.7f).SetEase(ease: Ease.OutBounce);
    }
    public IEnumerator PrevScene()
    {
        SoundManager.Instance.PlaySFX("UIClickCrispy1");
        prevClick = prevRt.DOScale(0.7f, 0.1f).SetEase(ease: Ease.OutQuart);
        prevClick.OnComplete(() => prevRt.DOScale(1f, 0.2f).SetEase(ease: Ease.OutBounce));
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        GameManager.Instance.LoadSceneAsync(1);
    }
    Tween prevClick;
    public RectTransform prevRt;
    IEnumerator NextScene()
    {
        SoundManager.Instance.PlaySFX("UIClickBubble1");


        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        GameManager.Instance.LoadSceneAsync(3);
    }

}
