using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SphericalBGControl : MonoBehaviour
{
    public enum GroundType
    {
        Dirt,
        Grass,
        Asphalt,
        Concrete,
    }
    [SerializeField] List<Texture> SpehericalBG_Dirt = new List<Texture>();
    [SerializeField] List<Texture> SpehericalBG_Grass = new List<Texture>();
    [SerializeField] List<Texture> SpehericalBG_Asphalt = new List<Texture>();
    [SerializeField] List<Texture> SpehericalBG_Concrete = new List<Texture>();
    Transform playerTr;
    Transform SphericalBG;
    Transform SphericalBG1;
    Renderer SphericalBGrndr;
    Renderer SphericalBG1rndr;
    [ReadOnlyInspector][SerializeField] Texture currTex;
    [ReadOnlyInspector][SerializeField] Texture prevTex;
    void Awake()
    {
        SphericalBG = transform.Find("SphericalBG");
        SphericalBG1 = transform.Find("SphericalBG(1)");
        SphericalBGrndr = SphericalBG.GetComponent<Renderer>();
        SphericalBG1rndr = SphericalBG1.GetComponent<Renderer>();
        playerTr = Player.I.transform;
    }
    public void ChangeBG(GroundType groundType)
    {
        if (isChanging) prevTex = SphericalBG1rndr.material.mainTexture;
        else prevTex = SphericalBGrndr.material.mainTexture;
        List<Texture> list = SpehericalBG_Dirt;
        switch (groundType)
        {
            case GroundType.Dirt:
                list = SpehericalBG_Dirt;
                break;
            case GroundType.Grass:
                list = SpehericalBG_Grass;
                break;
            case GroundType.Asphalt:
                list = SpehericalBG_Asphalt;
                break;
            case GroundType.Concrete:
                list = SpehericalBG_Concrete;
                break;
        }
        int rnd = Random.Range(0, list.Count);
        for (int j = 0; j < 40; j++)
        {
            if (list[rnd].name == prevTex.name)
                rnd = Random.Range(0, list.Count);
            else break;
        }
        currTex = list[rnd];
        Debug.Log(currTex);
        tween1.Kill();
        tween2.Kill();
        tween1 = SphericalBGrndr.material.DOFloat(0f,"_Alpha",15f);
        SphericalBG1.gameObject.SetActive(true);
        Vector3 pos = playerTr.position;
        pos.y = 35f;
        SphericalBG1.position = pos;
        SphericalBG1rndr.material.mainTexture = currTex;
        tween2 = SphericalBG1rndr.material.DOFloat(1f,"_Alpha",15f);
        isChanging = true;
        tween1.OnComplete(() =>
        {
            isChanging = false;
            SphericalBG.position = SphericalBG1.position;
            SphericalBGrndr.material.mainTexture = SphericalBG1rndr.material.mainTexture;
            Debug.Log(SphericalBGrndr.material.mainTexture);
            SphericalBGrndr.material.SetFloat("_Alpha", 1f);
            SphericalBG1rndr.material.SetFloat("_Alpha", 0f);
            SphericalBG1.gameObject.SetActive(false);
        });
    }
    bool isChanging = false;
    Tween tween1;
    Tween tween2;
}
