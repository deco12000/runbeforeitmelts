using UnityEngine;
using DG.Tweening;
public class SettingsUI : MonoBehaviour
{
    public GameObject settingsPanel;

    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            settingsPanel.transform.localScale = 0.7f * Vector3.one;
            settingsPanel.transform.DOScale(1.0f, 0.3f).SetEase(ease: Ease.OutBounce);
        }
        
    }
}