using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    void Start()
    {
        EventHub.Instance.Register<EventAttack>(OnAttack);
        EventHub.Instance.Register<EventGetItem>(OnGetItem);
    }

    void OnAttack(EventData ed)
    {
        AttackData hd = ed as AttackData;
        if(hd == null) return;
        if(hd.target != Player.Instance.transform) return;
        hpSlider.value -= (hd.damage/100f);
    }

     void OnGetItem(EventData ed)
    {
        GetItemData d = ed as GetItemData;
        if(d.Name == "Heal")
        {
            hpSlider.value += (25f/100f);
        }
    }





}
