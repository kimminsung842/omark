// SceneLoader.cs (수정된 코드)

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("불러올 3D 공간 Scene 이름")]
    public string sceneToLoad = "Scene_3DSample";

    // 이 스크립트가 붙어있는 오브젝트가 켜지고(OnEnable) 꺼질 때(OnDisable) 로드/언로드를 실행합니다.

    // private bool isSceneLoaded = false; // GetSceneByName().isLoaded로 대체 가능

    // **OnEnable:** 오브젝트가 활성화될 때 호출됨
    void OnEnable()
    {
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            // Scene을 'Additive' 모드로 비동기 로드
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            Debug.Log($"[SceneLoader] '{sceneToLoad}' 씬을 추가 로드했습니다.");
        }
    }

    // **OnDisable:** 오브젝트가 비활성화될 때 호출됨
    void OnDisable()
    {
        if (SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            // Scene을 언로드
            SceneManager.UnloadSceneAsync(sceneToLoad);
            Debug.Log($"[SceneLoader] '{sceneToLoad}' 씬을 해제했습니다.");
        }
    }
}