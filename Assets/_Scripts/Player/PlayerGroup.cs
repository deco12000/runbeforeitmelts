public class PlayerGroup : SingletonBehaviour<PlayerGroup>
{
    protected override bool IsDontDestroy() => true;
    public PlayerInput pinput { get; protected set; }
    public PlayerControl pctrl;
    public PlayerCamera pcam;
    public PlayerUI pui;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(transform.parent);
    }
}
