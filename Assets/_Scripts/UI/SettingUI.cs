using UnityEngine;
using DG.Tweening;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingsPanel;
    private RectTransform panelTransform;

    [Header("애니메이션 설정")]
    public float animationDuration = 0.4f;
    [Range(0f, 1f)] public float startScale = 0.7f;

    public Ease showEase = Ease.OutBack;
    public Ease hideEase = Ease.InBack;

    private bool isVisible = false;

    void Start()
    {
        panelTransform = settingsPanel.GetComponent<RectTransform>();
        panelTransform.localScale = Vector3.one * startScale;
        settingsPanel.SetActive(false);
    }

    public void ToggleSettings()
    {
        if (!isVisible)
        {
            settingsPanel.SetActive(true);
            panelTransform.localScale = Vector3.one * startScale;

            panelTransform.DOScale(Vector3.one, animationDuration)
                .SetEase(showEase);
        }
        else
        {
            panelTransform.DOScale(Vector3.one * startScale, animationDuration)
                .SetEase(hideEase)
                .OnComplete(() => settingsPanel.SetActive(false));
        }

        isVisible = !isVisible;
    }
}

