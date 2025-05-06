using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    void Start()
    {
        EventHub.Instance.Register<EventAttack>(OnAttack);
    }

    void OnAttack(EventData ed)
    {
        AttackData hd = ed as AttackData;
        if(hd == null) return;
        if(hd.target != Player.Instance.transform) return;
        hpSlider.value -= (hd.damage/100f);
    }





}
