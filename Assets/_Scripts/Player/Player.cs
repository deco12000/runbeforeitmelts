using UnityEngine;
public class Player : SingletonBehaviour<Player>
{
    protected override bool IsDontDestroy() => true;
    [ReadOnly] public PlayerControl pctrl;
    [ReadOnly] public PlayerInput pinput;
    [ReadOnly] public PlayerCamera pcam;
    [ReadOnly] public PlayerUI pui;
    [ReadOnly] public State state = State.Idle;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(transform.parent);
    }
    public enum State {Idle,Move,Jump,Hit,Die}
}
