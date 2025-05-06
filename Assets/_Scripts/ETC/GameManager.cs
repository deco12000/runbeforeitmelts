using UnityEngine;
public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true;
    public float scrollSpeed;
    public float trackDistance = 0f;
    public float distance = 0f;
    public int select;
    public int money;
    




}
