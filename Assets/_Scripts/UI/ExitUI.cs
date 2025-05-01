using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("게임 종료 버튼 클릭됨");

#if UNITY_EDITOR
        // 에디터에서 실행 중일 경우
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 실행 중일 경우
        Application.Quit();
#endif
    }
}