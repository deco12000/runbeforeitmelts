using UnityEngine;
public class ObstacleRainDrop : MonoBehaviour, IObstacle
{
    string IObstacle.Name {get=>"ObstacleRainDrop";}
    public void OnAttackHit(EventData ed){}
    public void OnHit(EventData ed)
    {
        
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        
    }



}
