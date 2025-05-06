using System.Collections;
using UnityEngine;
public class FirstScene : MonoBehaviour
{
    IEnumerator Start()
    {
        
        yield return YieldInstructionCache.WaitForSeconds(0.8f);

        GameManager.Instance.LoadSceneAsync(1);


    }
}
