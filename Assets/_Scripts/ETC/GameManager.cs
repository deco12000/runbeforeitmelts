using UnityEngine;
public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true; 
    // 캐릭터 뭘 골랐는지 정보
    public int select;
    // 이전 트랙까지 총 이동 거리 정보
    public float prevDistance;
    // 현재 트랙에서 이동거리 정보
    public float currDistance;
    // 돈을 얼마나 먹었는지 정보
    public int money;
    

    

}
