using UnityEngine;
public class TrackGarden1 : Track
{
    public Transform[] _endPivot;
    public Animation[] _camPivot;
    public override Transform[] endPivot => _endPivot;
    public override Animation[] camPivot => _camPivot;

    public override void SpawnItems()
    {
        
    }

    public override void SpawnObstacles()
    {
        
    }
}
