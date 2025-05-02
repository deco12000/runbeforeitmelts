using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IngameSetting : MonoBehaviour
{
    [Header("설정 패널")]
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

    public void ToggleSettings()
    {
        if (!isSettingsVisible)
        {
            settingsPanel.SetActive(true);
            settingsTransform.localScale = Vector3.one * startScale;

            settingsTransform.DOScale(Vector3.one, animationDuration)
                .SetEase(showEase)
                .SetUpdate(true);

            Time.timeScale = 0f;
        }
        else
        {
            settingsTransform.DOScale(Vector3.one * startScale, animationDuration)
                .SetEase(hideEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    settingsPanel.SetActive(false);
                    Time.timeScale = 1f;
                });
        }

        isSettingsVisible = !isSettingsVisible;
    }

    public void ToggleRanking()
    {
        if (!isRankingVisible)
        {
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

    public void RestartGame()
    {
        Time.timeScale = 1f; // 퍼즈 해제
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 다시 로드
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 퍼즈 해제
        SceneManager.LoadScene("1.TitleScene"); // "MainMenu" 씬 이름에 맞게 변경 가능
    }
}
