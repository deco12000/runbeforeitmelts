using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    Transform camTr;
    void Awake()
    {
        PlayerGroup.Instance.pcam = this;
        camTr = transform.GetChild(0);
    }
    public void SetCamera(Vector3 start, Vector3 forward)
    {
        camTr.position = start;
        camTr.forward = forward;
    }
    public void SetCamera(Vector3 start, Quaternion euler)
    {
        camTr.position = start;
        camTr.rotation = euler;
    }
    


}
