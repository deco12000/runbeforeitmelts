using UnityEngine;
public class PlayerUI : MonoBehaviour
{
    void Awake()
    {
       Player.Instance.ui = this;
    }
    
}
