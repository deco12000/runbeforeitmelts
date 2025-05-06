using UnityEngine;
public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true;
    public float scrollSpeed;
    public float distance;
    public int select;
    public int money;
    




}
