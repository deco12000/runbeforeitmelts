using UnityEngine;
using UnityEngine.Events;
public class PlayerInput : MonoBehaviour
{
    public bool isMove;
    public Vector2 moveDirection;
    public bool isJump;
    public Vector2 jumpDirection;
    public float jumpTime;
    public UnityAction OnJumpDown;
    public UnityAction<Vector2,float> OnJumpUp;
    public bool isSlide;
    public Vector2 slideDirection;
    public float slideTime;
    public UnityAction OnSlideDown;
    public UnityAction<float> OnSlideUp;
    public bool isDoubleJump;
    public Vector2 doubleJumpDirection;
    public UnityAction OnDoubleJumpDown;
    public bool isRoll;
    public bool rollDirection;
    public UnityAction OnRollDown;
    public bool isDash;
    public bool dashDirection;
    public UnityAction OnDashDown;

    
    



}
