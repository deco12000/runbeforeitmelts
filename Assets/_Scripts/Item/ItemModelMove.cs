using UnityEngine;
using DG.Tweening;
public class ItemModelMove : MonoBehaviour
{
    void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(Random.Range(-7f, 7f), Random.Range(0f, 360f), Random.Range(-7f, 7f));
        transform.DORotate(30f * Vector3.up, 2f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        transform.DOLocalMoveY(0.3f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    void OnDisable()
    {
        transform.DOKill();
    }

}
