using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void OnTrigerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("체력 회복");
        }
    }
}
