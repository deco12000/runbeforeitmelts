using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    
    void Start()
    {
        EventHub.Instance.Register<EventDisablePlayer>(Disappear);
    }

    void Disappear(EventData data)
    {
        gameObject.SetActive(false);
    }

    
}
