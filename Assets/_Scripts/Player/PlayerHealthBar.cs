using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        EventHub.I.Register<EventAttack>(OnAttack);
    }
    void OnDisable()
    {
        EventHub.I.UnRegister<EventAttack>(OnAttack);
    }
    void OnAttack(EventData ed)
    {
        if (Player.I.isDead) return;
        AttackData _ed = ed as AttackData;
        if (_ed == null) return;
        if (_ed.target != Player.I.transform) return;
        slider.value -= (_ed.damage / 100f);
    }
    void OnDie(EventData ed)
    {
        slider.value = 0f;
    }




}
