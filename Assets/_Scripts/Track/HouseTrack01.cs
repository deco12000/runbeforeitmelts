using UnityEngine;
public class HouseTrack01 : Track
{
    #region Implement Setting
    [SerializeField] Transform[] _endPoint;
    public override Transform[] endPoint => _endPoint;
    [SerializeField] Animation[] _checkPoint;
    public override Animation[] checkPoint => _checkPoint;
    public override void SpawnObstacle() { }
    public override SphericalBGControl.GroundType groundType => SphericalBGControl.GroundType.Concrete;
    public override int startFloor => 2;
    public override bool isInside => true;
    public Item[] _items;
    public override Item[] items => _items;
    public override void SpawnItem()
    {
        base.SpawnItem();
    }
    #endregion



}
