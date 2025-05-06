using UnityEngine;
using System.Collections;
using Deform;
using DG.Tweening;
public class RainDrop : PoolBehaviour
{
    [SerializeField] SquashAndStretchDeformer squash;
    [SerializeField] SimplexNoiseDeformer noise;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider collider;
    bool already = false;
    IEnumerator CoolTime()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        already = false;
    }

    void OnEnable()
    {
        squash.Factor = 0f;
        transform.localScale = 0.4f * Vector3.one;
        rb.useGravity = true;
        already = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Player"))
        {
            float scaleY = transform.localScale.y;
            transform.DOScaleY(scaleY * 0.5f, 0.8f);
            DOTween.To(() => squash.Factor, x => squash.Factor = x, -0.5f, 0.8f);
            rb.useGravity = false;
            collider.isTrigger = true;
        }
        else
        {
            if (!already)
            {
                already = true;
                AttackData attackData = new AttackData(transform, Player.Instance.ctrl.transform, 18f);
                EventHub.Instance.Invoke<EventAttack>(attackData);
                StartCoroutine("CoolTime");
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!already)
            {
                already = true;
                AttackData attackData = new AttackData(transform, Player.Instance.ctrl.transform, 18f);
                EventHub.Instance.Invoke<EventAttack>(attackData);
                StartCoroutine("CoolTime");
                
            }
        }
    }




}
