using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FirstScene : MonoBehaviour
{
    IEnumerator Start()
    {
        //
        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        //
        yield return YieldInstructionCache.WaitForSeconds(0.8f);
        SceneManager.LoadSceneAsync(1,LoadSceneMode.Single);
    }
}
