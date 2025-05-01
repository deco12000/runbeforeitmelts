using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class JoysticMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    RectTransform rt;
    Rect rect;
    Vector2 center;
    Image img;
    PlayerInput input;
    Camera cam;
    void Awake()
    {
        TryGetComponent(out img);
        TryGetComponent(out rt);
#if UNITY_EDITOR
        img.color = new Color(1f, 1f, 1f, 0.02f);
#else
        img.color = new Color(1f,1f,1f,0f);
#endif
    }
    void OnEnable()
    {
        input = PlayerGroup.Instance.pinput;
        cam = Camera.main;
        rect = rt.GetRect();
        center = new Vector2(rect.x + 0.5f * rect.width , rect.y + 0.5f * rect.height);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(eventData.position - center);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(eventData.position - center);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.position - center);
    }





}
