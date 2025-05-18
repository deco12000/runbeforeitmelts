using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        EventHub.I.Register<EventAttack>(OnAttack);
        EventHub.I.Register<EventGetItem>(OnGetItem);
    }
    void OnDisable()
    {
        EventHub.I.UnRegister<EventAttack>(OnAttack);
        EventHub.I.UnRegister<EventGetItem>(OnGetItem);
    }
    void OnAttack(EventData ed)
    {
        if (Player.I.isDead) return;
        AttackData _ed = ed as AttackData;
        if (_ed == null) return;
        if (_ed.target != Player.I.transform) return;
        slider.value -= (_ed.damage / 100f);
        slider.value = Mathf.Clamp01(slider.value);
    }
    void OnDie(EventData ed)
    {
        slider.value = 0f;
    }
    void OnGetItem(EventData ed)
    {
        if (Player.I.isDead) return;
        GetItemData d = ed as GetItemData;
        Debug.Log($"아이템 획득 : {d.Name}");
        if (d.Name == "Heal1")
        {
            slider.value += 0.25f;
            slider.value = Mathf.Clamp01(slider.value);
        }
        else if (d.Name == "Heal2")
        {
            slider.value += 1f;
            slider.value = Mathf.Clamp01(slider.value);
        }
    }




}
