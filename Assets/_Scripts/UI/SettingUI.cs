using UnityEngine;
using DG.Tweening;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingsPanel;
    private RectTransform panelTransform;
    public float animationDuration = 0.3f;

    public float startScale = 0.5f; // 시작 크기 비율 (70%)
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
                .SetEase(Ease.OutBack);
        }
        else
        {
            panelTransform.DOScale(Vector3.one * startScale, animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => settingsPanel.SetActive(false));
        }

        isVisible = !isVisible;
    }
}
