using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    public bool isDead = false;
    public float maxHP = 100;
    public float currHP = 100;

    void Start()
    {
        EventHub.Instance.Register<EventHit>(OnHit);
    }




    void OnHit(EventData ed)
    {
        HitData hd = ed as HitData;
        if(hd == null) return;
        if(hd.target != transform) return;
        currHP -= hd.damage;
        currHP = Mathf.Clamp(currHP,0f,100f);
        // 플레이어가 공격에 맞았을때 아랫줄에 작성


        
        if(currHP <= 0f && !isDead)
        {
            //플레이어가 죽었을때 아랫줄에 작성
            isDead = true;
            EventHub.Instance.Invoke<EventDie>(new DieData(transform));



        }
    }







}
