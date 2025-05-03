using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IObstacle
{
    void OnOnHit(EventData ed);
    void OnAttackHit(EventData ed);
}
