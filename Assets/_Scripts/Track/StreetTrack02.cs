using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetTrack02 : Track
{
    #region Implement Setting
    [SerializeField] Transform[] _endPoint;
    public override Transform[] endPoint => _endPoint;
    [SerializeField] Animation[] _checkPoint;
    public override Animation[] checkPoint => _checkPoint;
    public override void SpawnItem(){}
    public override void SpawnObstacle(){}
    public override SphericalBGControl.GroundType groundType => SphericalBGControl.GroundType.Dirt;
    public override int startFloor => 0;
    public override bool isInside => false;
    #endregion
    
}
