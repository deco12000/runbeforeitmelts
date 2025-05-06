using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    public bool isDead = false;
    public float maxHP = 100;
    public float currHP = 100;
    public float maxSP = 100;
    public float currSP = 100;

    void Start()
    {
        EventHub.Instance.Register<EventAttack>(OnAttack);
    }




    void OnAttack(EventData ed)
    {
        AttackData hd = ed as AttackData;
        if (hd == null) return;
        if (hd.target != transform) return;
        currHP -= hd.damage;
        currHP = Mathf.Clamp(currHP, 0f, 100f);
        // 플레이어가 공격에 맞았을때 아랫줄에 작성

#if UNITY_ANDROID
        Handheld.Vibrate();
#elif UNITY_IOS
        Handheld.Vibrate();
#endif

        if (currHP <= 0f && !isDead)
        {
            //플레이어가 죽었을때 아랫줄에 작성
            isDead = true;
            EventHub.Instance.Invoke<EventDie>(new DieData(transform));
            Player.Instance.ctrl.DisableAblity<AbilityJump>("Die");
            Player.Instance.ctrl.DisableAblity<AbilityMove>("Die");
            Player.Instance.ctrl.anim.SetTrigger("Die");
            EventHub.Instance.Invoke<EventScrollPause>();
#if UNITY_ANDROID
            Handheld.Vibrate();
#elif UNITY_IOS
    Handheld.Vibrate();
#endif



        }
    }







}
