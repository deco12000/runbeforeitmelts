using UnityEngine;
public interface IObstacle
{
    string Name { get; }
    void OnHit(EventData ed);
    void OnAttackHit(EventData ed);
}
