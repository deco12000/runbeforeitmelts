using UnityEditor.Animations;
using UnityEngine;
public class PlayerModelChanger : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    Animator anim;
    public void Change(int index)
    {
        GameObject go = null;
        go = Instantiate(prefabs[index]);
        go.transform.parent = transform;
        go.TryGetComponent(out anim);
        Player.Instance.pctrl.ainm = anim;
    }




}
