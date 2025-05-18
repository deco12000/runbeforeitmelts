using UnityEngine;
public abstract class Track : PoolBehaviour
{
    public abstract Transform[] endPoint { get; }
    public abstract Animation[] checkPoint { get; }
    public abstract Item[] items { get; }
    public virtual void SpawnItem()
    {
        Transform spawn = transform.Find("Spawn");
        if (spawn == null) return;
        foreach (Item item in items)
        {
            



        }
    }
    public virtual void SpawnObstacle()
    {

    }
    public abstract SphericalBGControl.GroundType groundType { get; }
    public abstract int startFloor {get;}
    public abstract bool isInside {get;}
}
