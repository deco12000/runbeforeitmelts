using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FirstScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        EventHub.Instance.Invoke<EventDisablePlayer>();
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        SceneManager.LoadScene(1);

    }

    



}
