using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class SceneChanger : MonoBehaviour
{
    public static SceneChanger I;
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
    }
    void OnDisable() { UniTaskCancel(); }
    void OnDestroy() { UniTaskCancel(); }
    void UniTaskCancel()
    {
        try
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
        cts = null;
    }
    #endregion

    //씬 전환 트랜지션 효과 관련
    Image imgTrans;
    Image imgWait;
    Image imgBlack;
    Slider slider;
    void Awake()
    {
        I = this;
        transform.Find("Transition/ImgTrans").TryGetComponent(out imgTrans);
        transform.Find("Transition/ImgWait").TryGetComponent(out imgWait);
        transform.Find("Transition/Slider").TryGetComponent(out slider);
    }
    void Start()
    {
        imgTrans.material.color = new Color(0f, 0f, 0f, 0f);
        imgTrans.material.SetFloat("_Value", 1f);
        imgTrans.gameObject.SetActive(false);
        imgWait.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
    }
    public void LoadSceneAsync(int index)
    {
        LoadSceneAsync_ut(index, cts.Token).Forget();
    }
    async UniTask LoadSceneAsync_ut(int index, CancellationToken token)
    {
        FadeOut();
        await UniTask.Delay(500, ignoreTimeScale:true, cancellationToken: token);
        imgTrans.material.color = new Color(0f, 0f, 0f, 1f);
        imgTrans.material.SetFloat("_Value", 0f);
        // 로딩바
        imgWait.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        slider.value = 0f;
        imgWait.transform.DOLocalRotate(new Vector3(0f, 0f, -360f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        // 0.5초 정도 기다리는 시간 (시각적 효과)
        slider.DOValue(0.5f, 0.5f);
        await UniTask.Delay(500, ignoreTimeScale:true, cancellationToken: token);
        // 비동기 씬 로드
        AsyncOperation ao = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!ao.isDone)
        {
            slider.value = 0.5f + 0.5f * ao.progress;
            await UniTask.Delay(10, ignoreTimeScale:true, cancellationToken: token);
        }
    }
    public void FadeOut()
    {
        // 화면 페이드 아웃
        float rnd1 = (Random.Range(0, 2) - 0.5f) * 4f;
        float rnd2 = (Random.Range(0, 2) - 0.5f) * 2f;
        imgTrans.gameObject.SetActive(true);
        imgTrans.transform.localScale = new Vector3(rnd1, rnd2, 0);
        imgTrans.material.SetFloat("_Value", 1f);
        imgTrans.material.DOFloat(0f, "_Value", 0.14f);
        imgTrans.material.color = new Color(0f, 0f, 0f, 0.3f);
        imgTrans.material.DOFade(1f, 0.3f);
    }
    public void FadeIn()
    {
        // 화면 페이드 인
        StopCoroutine("LoadSceneAsync_co");
        imgWait.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        imgTrans.material.color = new Color(0f, 0f, 0f, 1f);
        imgTrans.material.SetFloat("_Value", 0f);
        imgTrans.material.DOFade(0f, 2.5f).SetEase(ease:Ease.OutQuad);
    }

}

























//     void Update()
//     {
//         TouchEffect();
//     }
//     void TouchEffect()
//     {
//         if (cam == null || !cam.gameObject.activeInHierarchy || !cam.enabled) cam = Camera.main;
//         int touchCount = Mathf.Min(Input.touchCount, 2);
//         for (int i = 0; i < touchCount; i++)
//         {
//             Touch touch = Input.GetTouch(i);
//             if (touch.phase == TouchPhase.Began)
//             {
//                 ParticleManager.I?.PlayEffectSprite("TouchTick", touch.position, 0.77f, 0.23f);
//                 SFX sfx = SoundManager.I?.PlaySFX("TouchTick");
//             }
//         }
// #if UNITY_EDITOR
//         if (Input.GetMouseButtonDown(0))
//         {
//             ParticleManager.I?.PlayEffectSprite("TouchTick", Input.mousePosition, 0.77f, 0.23f);
//             SFX sfx = SoundManager.I?.PlaySFX("TouchTick");
//         }
// #endif
//     }




