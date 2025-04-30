using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScene : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene(2);
    }
}
