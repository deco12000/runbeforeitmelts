using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true;
    public float scrollSpeed;
    public float trackDistance = 0f;
    public float distance = 0f;
    public int select;
    public int money;
    public Image transition;
    public RawImage waiting;
    public Slider waitingBar;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
        transition = GetComponentInChildren<Image>();
        waiting = GetComponentInChildren<RawImage>();
        waitingBar = GetComponentInChildren<Slider>();
        transition.material.DOFade(0f, 0.01f);
        waiting.gameObject.SetActive(false);
        waitingBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (cam == null || !cam.gameObject.activeInHierarchy || !cam.enabled)
        {
            cam = Camera.main;
        }
        int touchCount = Mathf.Min(Input.touchCount, 2);
        for (int i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPosition = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 1f));

                // 이펙트 생성
                //Instantiate(effectPrefab, touchPosition, Quaternion.identity);

                // 사운드 재생
                SFX sfx = SoundManager.Instance.PlaySFX("TouchTick");
                sfx.aus.volume = 0.2f;
            }
        }
    }


    public void LoadSceneAsync(int index)
    {
        StartCoroutine("LoadSceneAsync_co", index);
    }
    IEnumerator LoadSceneAsync_co(int index)
    {
        // 화면 페이드 아웃
        transition.transform.localScale = 2.5f * Vector3.one;
        transition.material.SetFloat("_Threshold", 1f);
        transition.material.DOFloat(0f, "_Threshold", 0.25f);
        transition.material.SetFloat("_Smoothness", 0.08f);
        transition.material.DOFloat(0f, "_Smoothness", 0.25f);
        transition.material.DOFade(1f, 0.8f).SetEase(ease: Ease.OutExpo);
        yield return YieldInstructionCache.WaitForSeconds(0.4f);
        waiting.gameObject.SetActive(true);
        waitingBar.gameObject.SetActive(true);
        waitingBar.value = 0f;
        waiting.transform
        .DOLocalRotate(new Vector3(0f, 0f, -360f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        waitingBar.DOValue(0.5f, 1.2f);
        yield return YieldInstructionCache.WaitForSeconds(1.2f);
        AsyncOperation ao = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!ao.isDone)
        {
            waitingBar.value = 0.5f + 0.5f * (ao.progress + 0.2f);
            yield return null;
        }
    }
    public void FadeIn()
    {
        StopCoroutine("LoadSceneAsync_co");
        waiting.gameObject.SetActive(false);
        waitingBar.gameObject.SetActive(false);
        transition.material.SetFloat("_Threshold", 0f);
        transition.material.SetFloat("_Smoothness", 0.001f);
        transition.material.color = new Color(0f,0f,0f,1f);
        transition.material.DOFade(0f, 3f);
    }







}
