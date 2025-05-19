using System.Collections;
using UnityEngine;
public class WaterSpawner : MonoBehaviour
{
    [SerializeField] WaterDrop waterDrop;
    [SerializeField] Particle rainParticle1;
    [SerializeField] Particle rainParticle2;
    [SerializeField] Particle rainParticle3;
    ParticleSystem rain1Ps;
    ParticleSystem rain2Ps;
    ParticleSystem rain3Ps;
    float originalValue1;
    float originalValue2;
    float originalValue3;
    Transform player;
    public bool isInside = true;
    IEnumerator Start()
    {
        player = Player.I.transform;
        rain1Ps = rainParticle1.transform.GetChild(0).GetComponent<ParticleSystem>();
        rain2Ps = rainParticle2.transform.GetChild(0).GetComponent<ParticleSystem>();
        rain3Ps = rainParticle3.transform.GetChild(0).GetComponent<ParticleSystem>();
        originalValue1 = rain1Ps.emission.rateOverTime.constant;
        originalValue2 = rain2Ps.emission.rateOverTime.constant;
        originalValue3 = rain3Ps.emission.rateOverTime.constant;
        //Debug.Log($"{originalValue1}, {originalValue2}");
        yield return YieldInstructionCache.WaitForSeconds(4f);
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
            Vector3 pos = player.position + 30f * player.forward + Random.Range(0f, 100f) * Random.insideUnitSphere;
            pos.y = player.position.y + Random.Range(15f, 35f);
            if (Random.Range(0, 100) < 5)
            {
                pos = player.position;
                pos.y = player.position.y + Random.Range(15f, 35f);
            }
            else if (Random.Range(0, 100) < 60)
            {
                pos = player.position + 10f * player.forward + Random.Range(0f, 15f) * Random.insideUnitSphere;
                pos.y = player.position.y + Random.Range(15f, 35f);
            }
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 100f, 1 << 0))
            {
                PoolBehaviour pb;
                pb = PoolManager.I.Spawn(waterDrop, pos, Quaternion.identity, transform, 35);
            }
            else
            {
                continue;
            }
            if (isInside)
            {
                if (rain1Ps.emission.rateOverTime.constant != originalValue1 * 0.04f)
                {
                    var emission = rain1Ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.mode = ParticleSystemCurveMode.Constant;
                    rateOverTime.constant = originalValue1 * 0.04f;
                    emission.rateOverTime = rateOverTime;
                }
                if (rain2Ps.emission.rateOverTime.constant != originalValue2 * 0.04f)
                {
                    var emission = rain2Ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.mode = ParticleSystemCurveMode.Constant;
                    rateOverTime.constant = originalValue1 * 0.04f;
                    emission.rateOverTime = rateOverTime;
                }
                if (rain3Ps.emission.rateOverTime.constant != originalValue3 * 0.2f)
                {
                    var emission = rain3Ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.mode = ParticleSystemCurveMode.Constant;
                    rateOverTime.constant = originalValue1 * 0.2f;
                    emission.rateOverTime = rateOverTime;
                }
                yield return YieldInstructionCache.WaitForSeconds(1f);
            }
            else
            {
                if (rain1Ps.emission.rateOverTime.constant != originalValue1)
                {
                    var emission = rain1Ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.mode = ParticleSystemCurveMode.Constant;
                    rateOverTime.constant = originalValue1;
                    emission.rateOverTime = rateOverTime;
                }
                if (rain2Ps.emission.rateOverTime.constant != originalValue2)
                {
                    var emission = rain2Ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.mode = ParticleSystemCurveMode.Constant;
                    rateOverTime.constant = originalValue2;
                    emission.rateOverTime = rateOverTime;
                }
                if (rain3Ps.emission.rateOverTime.constant != originalValue3)
                {
                    var emission = rain3Ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.mode = ParticleSystemCurveMode.Constant;
                    rateOverTime.constant = originalValue3;
                    emission.rateOverTime = rateOverTime;
                }
                yield return YieldInstructionCache.WaitForSeconds(0.25f);
            }
            if (isInside) yield return YieldInstructionCache.WaitForSeconds(1f);
            if (isInside) yield return YieldInstructionCache.WaitForSeconds(1f);
            if (isInside) yield return YieldInstructionCache.WaitForSeconds(1f);
            if (isInside) yield return YieldInstructionCache.WaitForSeconds(1f);
            if (Player.I.isDead) yield return YieldInstructionCache.WaitForSeconds(6f);
        }
    }

}
