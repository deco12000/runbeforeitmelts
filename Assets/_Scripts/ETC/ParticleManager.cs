using System.Collections.Generic;
using UnityEngine;
public class ParticleManager : SingletonBehaviour<ParticleManager>
{
    [SerializeField] List<Particle> ptcList = new List<Particle>();
    [SerializeField] List<EffectImage> eiList = new List<EffectImage>();
    protected override bool IsDontDestroy() => true;
    public GameObject PlayParticle(string Name, Vector3 pos, Quaternion rot, Transform parent)
    {
        return null;
    }
    public GameObject PlayEffectImage(string Name, Vector3 pos, Quaternion rot, Canvas parent)
    {
        return null;
    }

}
