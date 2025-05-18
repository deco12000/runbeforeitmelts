using UnityEngine;
public class StreetTrack01 : Track
{
    #region Implement Setting
    [SerializeField] Transform[] _endPoint;
    public override Transform[] endPoint => _endPoint;
    [SerializeField] Animation[] _checkPoint;
    public override Animation[] checkPoint => _checkPoint;
    public override SphericalBGControl.GroundType groundType => SphericalBGControl.GroundType.Concrete;
    public override int startFloor => 0;
    public override bool isInside => false;
    public override void SpawnItem()
    {

    }
    public override void SpawnObstacle()
    {

    }
    #endregion

    
}
