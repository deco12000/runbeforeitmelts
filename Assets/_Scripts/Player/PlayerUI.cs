using UnityEngine;
public class PlayerUI : PlayerGroup
{
    protected override void Awake()
    {
        pui = this;
        Debug.Log("1" + pui);
    }

    
}
