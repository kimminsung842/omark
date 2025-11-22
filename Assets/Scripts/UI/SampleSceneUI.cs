using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class SampleSceneUI : MonoBehaviour
{
    public SpaceApi spaceApi;

    [Header("UI")]
    public TMP_InputField inputName;
    public Button btnSave;
    public Button btnBack;

    long envId;

    private void Start()
    {
        envId = SpaceSession.currentEnvironmentId;

        btnSave.onClick.AddListener(() => StartCoroutine(SaveEnvironment()));
        btnBack.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("sc_main");
        });

        StartCoroutine(LoadEnvironment());
    }

    IEnumerator LoadEnvironment()
    {
        yield return spaceApi.GetEnvironmentDetail(
            envId,
            env =>
            {
                inputName.text = env.name;
            },
            err => Debug.LogError(err)
        );
    }


    IEnumerator SaveEnvironment()
    {
        // 1) 씬 데이터를 byte[] 로 만들기 (TODO)
        byte[] sceneData = BuildSceneBytes();

        // 2) 서버에서 Presigned URL 요청
        S3PresignedUrlResponseDto uploadInfo = null;

        yield return spaceApi.RequestUploadUrl(
            envId,
            "scene.dat",
            res => uploadInfo = res,
            err => Debug.LogError(err)
        );

        if (uploadInfo == null)
        {
            Debug.LogError("Presigned URL 요청 실패");
            yield break;
        }

        // 3) S3 PUT 업로드
        UnityWebRequest req = UnityWebRequest.Put(uploadInfo.presignedUploadUrl, sceneData);
        req.method = "PUT";
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("S3 업로드 실패: " + req.error);
            yield break;
        }

        Debug.Log("S3 업로드 성공!");

        // 4) 이름 업데이트도 수행
        yield return spaceApi.UpdateEnvironment(envId, inputName.text,
            () => { },
            err => Debug.LogError(err)
        );

        // 5) 메인으로 복귀
        UnityEngine.SceneManagement.SceneManager.LoadScene("sc_main");
    }


    private byte[] BuildSceneBytes()
    {
        // 아직 구현 전이므로 빈 데이터
        return System.Text.Encoding.UTF8.GetBytes("TEMP DATA");
    }
}
