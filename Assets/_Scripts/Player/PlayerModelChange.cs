using UnityEngine;
public class PlayerModelChange : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    Animator anim;
    public void Change(int index)
    {
        GameObject go = null;
        go = Instantiate(prefabs[index]);
        go.transform.parent = transform;
        go.TryGetComponent(out anim);
        Player.Instance.ctrl.anim = anim;
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
    }




}
