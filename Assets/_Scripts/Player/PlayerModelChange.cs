using UnityEngine;
public class PlayerModelChange : MonoBehaviour
{
    public GameObject[] models;
    void OnEnable()
    {
        if (GameManager.I == null)
        {
            GameObject go = Instantiate(models[0]);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            Player.I.anim = go.GetComponent<Animator>();
            return;
        }
        if (GameManager.I.select == 0)
        {
            GameObject go = Instantiate(models[0]);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            Player.I.anim = go.GetComponent<Animator>();

        }
        else
        {
            GameObject go = Instantiate(models[1]);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            Player.I.anim = go.GetComponent<Animator>();



        }
    }
}
