using UnityEngine;
public class TrackGarden1 : Track
{
    public Transform[] _endPivot;
    public Animation[] _camPivot;
    public override Transform[] endPivot => _endPivot;
    public override Animation[] camPivot => _camPivot;
    [SerializeField] HealItem heal;
    Transform spawnPoint;

    void Start()
    {
        spawnPoint = transform.Find("SpawnPoints");   
    }

    public override void SpawnItems()
    {
        
    }


}
