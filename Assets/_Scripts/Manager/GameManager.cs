using UnityEngine;
public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true;
    public Vector3 scrollForward;
    public float scrollSpeed;
    public float distance;
    public int select;
    public int money;
    




}
