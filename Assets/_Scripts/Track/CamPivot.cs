using UnityEngine;
public class CamPivot : MonoBehaviour
{
    [SerializeField] Transform player;
    public Quaternion targetQuat = Quaternion.identity;
    void Start()
    {
        targetQuat = player.rotation;
    }
    void Update()
    {
        transform.position = player.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, 0.5f * Time.deltaTime);
    }
}
