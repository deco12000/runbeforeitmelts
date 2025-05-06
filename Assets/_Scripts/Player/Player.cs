using UnityEngine;
public class Player : SingletonBehaviour<Player>
{
    protected override bool IsDontDestroy() => true;
    [ReadOnlyInspector] public PlayerControl ctrl;
    [ReadOnlyInspector] public PlayerInput input;
    [ReadOnlyInspector] public PlayerCamera cam;
    [ReadOnlyInspector] public PlayerUI ui;
    [ReadOnlyInspector] public string state = "Idle";
    [ReadOnlyInspector] public string prevState = "Idle";
    protected virtual void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(transform.parent);
    }
}
