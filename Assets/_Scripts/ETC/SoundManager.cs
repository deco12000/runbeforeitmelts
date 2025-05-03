using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    // public static SoundManager Instance;
    // [SerializeField]
    // public SFX prefab;
    // public List<AudioClip> sfxList = new List<AudioClip>();
    // public List<AudioClip> bgmList = new List<AudioClip>();
    // AudioSource audioSourceBGM;
    // void Awake()
    // {
    //     if(Instance == null){Instance = this;}
    //     else{Destroy(this.gameObject);}
    //     DontDestroyOnLoad(this);
    //     transform.Find("BGM").TryGetComponent(out audioSourceBGM);
    // }
    // public void PlayBGM(string name)
    // {
    //     int find = bgmList.FindIndex(x => x.name == name);
    //     if (find == -1) return;
    //     audioSourceBGM.Stop();
    //     audioSourceBGM.clip = bgmList[find];
    //     audioSourceBGM.Play();
    // }
    // public void StopBGM()
    // {
    //     audioSourceBGM.Stop();
    // }
    // // BGM 크로스 페이드 효과
    // //     public Particle Play(string name, Vector3 pos, Vector3 scale, Vector3 dir, Transform parent)
    // //     {
    // //         int find = particleList.FindIndex(x => x.name == name);
    // //         if (find == -1)
    // //             return null;
    // //         if (parent == null)
    // //         {
    // //             parent = transform;
    // //         }
    // //         PoolBehaviour p = PoolManager.Instance.Spawn(particleList[find], pos, Quaternion.identity, parent);
    // //         Particle rp = p as Particle;
    // //         rp.transform.localScale = scale;
    // //         if (dir != Vector3.zero)
    // //             rp.transform.forward = dir;
    // //         return rp;
    // //     }

}