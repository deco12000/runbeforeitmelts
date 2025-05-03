using UnityEngine;
public class Player : SingletonBehaviour<Player>
{
    protected override bool IsDontDestroy() => true;
    [ReadOnlyInspector] public PlayerControl pctrl;
    [ReadOnlyInspector] public PlayerInput pinput;
    [ReadOnlyInspector] public PlayerCamera pcam;
    [ReadOnlyInspector] public PlayerUI pui;
    [ReadOnlyInspector] public State state = State.Idle;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(transform.parent);
    }
    public enum State {Idle,Move,Jump,Hit,Die}
}
