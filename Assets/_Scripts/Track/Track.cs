using UnityEngine;
public abstract class Track : PoolBehaviour
{
    public abstract Transform[] endPoint { get; }
    public abstract Animation[] checkPoint { get; }
    public abstract void SpawnItem();
    public abstract void SpawnObstacle();
    public abstract SphericalBGControl.GroundType groundType { get; }
    public abstract int startFloor {get;}
    public abstract bool isInside {get;}
}
