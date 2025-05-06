using UnityEngine;
public abstract class Track : PoolBehaviour
{
    public abstract Transform[] endPivot {get;}
    public abstract Animation[] camPivot {get;}
    public abstract void SpawnObstacles();
    public abstract void SpawnItems();


}
