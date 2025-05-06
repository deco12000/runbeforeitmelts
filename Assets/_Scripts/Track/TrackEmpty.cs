using UnityEngine;
public class TrackEmpty : Track
{
    public override Transform[] endPivot => throw new System.NotImplementedException();

    public override Animation[] camPivot => throw new System.NotImplementedException();

    public override void SpawnItems()
    {
        
    }

    public override void SpawnObstacles()
    {
        
    }
}
