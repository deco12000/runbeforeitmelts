using UnityEngine;

public abstract class PoolBehaviour : MonoBehaviour
{
    [HideInInspector] public PoolManager poolManager;
    public void Despawn() => poolManager.Despawn(this);
}
