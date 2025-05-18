using System.Collections;
using UnityEngine;
public class LogoScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        SceneChanger.I?.LoadSceneAsync(1);
    }
}
