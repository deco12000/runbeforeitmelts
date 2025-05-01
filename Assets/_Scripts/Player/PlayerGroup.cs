public class PlayerGroup : SingletonBehaviour<PlayerGroup>
{
    protected override bool IsDontDestroy() => false;
    public PlayerInput pinput { get; protected set; }
    public PlayerControl pctrl { get; protected set; }
    public PlayerCamera pcam { get; protected set; }
    public PlayerUI pui { get; protected set; }
    protected override void Awake()
    {
        DontDestroyOnLoad(transform.parent);
    }
}
