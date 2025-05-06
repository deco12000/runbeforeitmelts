using UnityEngine;
public class TackKichen : Track
{
    public Transform[] _endPivot;
    public Animation[] _camPivot;
    public override Transform[] endPivot => _endPivot;
    public override Animation[] camPivot => _camPivot;
    [SerializeField] GameObject[] obstacles;
    [SerializeField] GameObject[] items;
    Transform spawnPoint;
    void OnEnable()
    {
        spawnPoint = transform.Find("SpawnPoints");
    }

    public override void SpawnItems()
    {
        
    }
    
}
