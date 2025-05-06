using UnityEngine;
using System.Collections;
public class ObstacleRainDrop : PoolBehaviour
{
    bool already = false;
    IEnumerator CoolTime()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        already = false;
    }
    void OnTriggerStay(Collider other)
    {
        if(!already)
        {
            already = true;
            AttackData attackData = new AttackData(transform,Player.Instance.transform,10f);
            EventHub.Instance.Invoke<EventAttack>(attackData);
            StartCoroutine("CoolTime");
        }
    }



}
