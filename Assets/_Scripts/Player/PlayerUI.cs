using UnityEngine;
public class PlayerUI : MonoBehaviour
{
    void Awake()
    {
       PlayerGroup.Instance.pui = this;
    }
    
}
