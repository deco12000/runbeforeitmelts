using UnityEngine;
public abstract class Track : PoolBehaviour
{
    public abstract Transform[] endPoint { get; }
    public abstract Animation[] checkPoint { get; }
    public abstract Item[] items { get; }
    public virtual void SpawnItem()
    {
        Transform spawn = transform.Find("Spawn");
        //Transform[] child = spawn.GetChild();
        if (spawn == null) return;
        for (int i = 0; i < spawn.childCount; i++)
        {
            Transform child = spawn.GetChild(i);
            foreach (Item item in items)
            {
                if (child.name == item.name)
                {
                    if (Random.Range(0, 100) <= item.probablity)
                    {
                        PoolManager.I.Spawn(item, child.position, Quaternion.identity, transform, 30);
                        Debug.Log("아이템 스폰 성공");

                    }
                }
            }
        }
    }
    public virtual void SpawnObstacle()
    {

    }
    public abstract SphericalBGControl.GroundType groundType { get; }
    public abstract int startFloor { get; }
    public abstract bool isInside { get; }
    
}
