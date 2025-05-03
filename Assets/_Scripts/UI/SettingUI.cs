using UnityEngine;
using DG.Tweening;

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
        }
        else
        {
            settingsTransform.DOScale(Vector3.one * startScale, animationDuration)
                .SetEase(hideEase)
                .OnComplete(() => settingsPanel.SetActive(false));
        }

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
        }
        else
        {
            rankingTransform.DOScale(Vector3.one * startScale, animationDuration)
                .SetEase(hideEase)
                .OnComplete(() => rankingPanel.SetActive(false));
        }

        isRankingVisible = !isRankingVisible;
    }
}
