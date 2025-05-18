using System.Collections.Generic;
using UnityEngine;
public class ParticleManager : SingletonBehaviour<ParticleManager>
{
    [SerializeField] List<Particle> particleList = new List<Particle>();
    [SerializeField] List<EffectSprite> EffectSpriteList = new List<EffectSprite>();
    Transform canvas;
    protected override void Awake()
    {
        base.Awake();
        canvas = transform.GetChild(0);
    }
    protected override bool IsDontDestroy() => true;
    public Particle PlayParticle(string Name, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        int find = -1;
        for (int i = 0; i < particleList.Count; i++)
        {
            if (Name == particleList[i].name)
            {
                find = i;
                break;
            }
        }
        if (find == -1) return null;
        if (parent == null) parent = transform;
        PoolBehaviour pb = particleList[find];
        PoolBehaviour clone = PoolManager.I?.Spawn(pb, pos, Quaternion.identity, canvas);
        Particle _clone = clone as Particle;
        _clone.transform.position = pos;
        _clone.transform.rotation = rot;
        _clone.transform.SetParent(parent);
        _clone.Play();
        return _clone;
    }
    public EffectSprite PlayEffectSprite(string Name, Vector2 screenPos, float scale, float time)
    {
        int find = -1;
        for (int i = 0; i < EffectSpriteList.Count; i++)
        {
            if (Name == EffectSpriteList[i].name)
            {
                find = i;
                break;
            }
        }
        if (find == -1) return null;
        RectTransform rect = canvas as RectTransform;
        Vector2 localPos;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, null, out localPos);
        if (!success) return null;
        // localPos → worldPos 변환 필수!
        Vector3 worldPos = rect.TransformPoint(localPos);
        PoolBehaviour pb = EffectSpriteList[find];
        PoolBehaviour clone = PoolManager.I?.Spawn(pb, worldPos, Quaternion.identity, canvas);
        EffectSprite _clone = clone as EffectSprite;
        _clone.transform.localScale = scale * Vector3.one;
        _clone.Play(time);
        return _clone;
    }
    public Transform PlayWorldText(string content, Vector3 pos, Vector3 scale, Color color, float time = 1f, Transform parent = null)
    {
        

        return null;
    }
}