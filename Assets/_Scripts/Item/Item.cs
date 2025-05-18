using UnityEngine;
using UnityEngine.Events;
public abstract class Item : PoolBehaviour
{
    protected abstract bool isUsed {get;}
    public abstract int probablity {get;}
}
