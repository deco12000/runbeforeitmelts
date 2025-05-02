using UnityEditor.Animations;
using UnityEngine;
public class PlayerModelChanger : MonoBehaviour
{
    [SerializeField] GameObject[] models;
    [SerializeField] AnimatorController[] animatorContollers;


    public void Change(int index)
    {
        switch(index)
        {
            case 0:
            break;
        }
    }




}
