using UnityEngine;
public class Player : SingletonBehaviour<Player>
{
    protected override bool IsDontDestroy() => true;
    [ReadOnly] public PlayerControl pctrl;
    [ReadOnly] public PlayerInput pinput;
    [ReadOnly] public PlayerCamera pcam;
    [ReadOnly] public PlayerUI pui;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(transform.parent);
    }
}
