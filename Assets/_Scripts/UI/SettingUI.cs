using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("패널")]
    public GameObject settingsPanel;
    public GameObject rankingPanel;

    private RectTransform settingsTransform;
    private RectTransform rankingTransform;

    [Header("애니메이션 설정")]
    public float animationDuration = 0.4f;
    [Range(0f, 1f)] public float startScale = 0.7f;

    public Ease showEase = Ease.OutBack;
    public Ease hideEase = Ease.InBack;

    private bool isSettingsVisible = false;
    private bool isRankingVisible = false;

    void Start()
    {
        settingsTransform = settingsPanel.GetComponent<RectTransform>();
        rankingTransform = rankingPanel.GetComponent<RectTransform>();

        settingsTransform.localScale = Vector3.one * startScale;
        rankingTransform.localScale = Vector3.one * startScale;

        settingsPanel.SetActive(false);
        rankingPanel.SetActive(false);

        sliderVolumeBGM.value = PlayerPrefs.GetFloat("volumeBGM", 1f);
        sliderVolumeSFX.value = PlayerPrefs.GetFloat("volumeSFX", 1f);
    }

    public void ToggleSettings(bool forceClose = false)
    {
        if (forceClose)
        {
            settingsTransform.DOKill();
            settingsPanel.SetActive(false);
            settingsTransform.localScale = Vector3.one * startScale;
            isSettingsVisible = false;
            return;
        }

        if (!isSettingsVisible)
        {
            if (isRankingVisible)
            {
                ToggleRanking(forceClose: true);
            }

            settingsPanel.SetActive(true);
            settingsTransform.localScale = Vector3.one * startScale;

            settingsTransform.DOScale(Vector3.one, animationDuration)
                .SetEase(showEase);

            SoundManager.Instance.PlaySFX("UIClickSharp1");
        }
        else
        {
            settingsTransform.DOScale(Vector3.one * startScale, animationDuration * 0.2f)
                .SetEase(hideEase)
                .OnComplete(() => settingsPanel.SetActive(false));

            SoundManager.Instance.PlaySFX("UIClickCrispy1");
        }
        PlayerPrefs.SetFloat("volumeBGM", sliderVolumeBGM.value);
        PlayerPrefs.SetFloat("volumeSFX", sliderVolumeSFX.value);

        isSettingsVisible = !isSettingsVisible;
    }

    public void ToggleRanking(bool forceClose = false)
    {
        if (forceClose)
        {
            rankingTransform.DOKill();
            rankingPanel.SetActive(false);
            rankingTransform.localScale = Vector3.one * startScale;
            isRankingVisible = false;
            return;
        }

        if (!isRankingVisible)
        {
            if (isSettingsVisible)
            {
                ToggleSettings(forceClose: true);
            }

            rankingPanel.SetActive(true);
            rankingTransform.localScale = Vector3.one * startScale;

            rankingTransform.DOScale(Vector3.one, animationDuration)
                .SetEase(showEase);

            SoundManager.Instance.PlaySFX("UIClickSharp1");
        }
        else
        {
            rankingTransform.DOScale(Vector3.one * startScale, animationDuration * 0.2f)
                .SetEase(hideEase)
                .OnComplete(() => rankingPanel.SetActive(false));
            
            SoundManager.Instance.PlaySFX("UIClickCrispy1");
        }

        isRankingVisible = !isRankingVisible;
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlaySFX("UIClickCrispy1");

        Debug.Log("게임 종료 버튼 클릭됨");
#if UNITY_EDITOR
        // 에디터에서 실행 중일 경우
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 실행 중일 경우
        Application.Quit();
#endif
    }


    [SerializeField] Slider sliderVolumeBGM;
    [SerializeField] Slider sliderVolumeSFX;
    public void SetVolumeBGM()
    {
        SoundManager.Instance.SetVolumeBGM(sliderVolumeBGM.value);
        
    }
    public void SetVolumeSFX()
    {
        SoundManager.Instance.SetVolumeSFX(sliderVolumeSFX.value);
    }

    
}
