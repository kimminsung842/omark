using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using GLTFast;
using System.Threading.Tasks;

public class ModelUploader : MonoBehaviour
{
    [Header("UI")]
    public Button uploadButton;
    public RawImage previewImage;
    public Transform modelParent;

    [Header("Server")]
    // public string serverUrl = "http://3.236.220.19:8000/upload-image";
    public string serverUrl = "https://kemini-aws.duckdns.org/api/v1/ai/generate-model";
    // public string serverUrl = "http://3.236.220.19:8000/api/v1/ai/generate-model";


    private string imagePath;

    void Start()
    {
        uploadButton.onClick.AddListener(OnUploadButtonClicked);
    }

    void OnUploadButtonClicked()
    {
        PickImageFromGallery();
    }

    void PickImageFromGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path == null)
            {
                Debug.Log("이미지 선택 취소됨");
                return;
            }

            imagePath = path;

            // 미리보기 로드
            Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize: 2048);
            previewImage.texture = tex;

            // 서버 업로드
            StartCoroutine(UploadImageToServer(path));

        }, "이미지를 선택하세요");
    }

    IEnumerator UploadImageToServer(string path)
    {
        Debug.Log("이미지 업로드 중...");

        byte[] imageBytes = File.ReadAllBytes(path);
        string fileName = Path.GetFileName(path);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, fileName, "image/png");

        string accessToken = "eyJraWQiOiJFQk1jMXlEaXVOQTlsNTIwd00wK2VqZTk2RmxtN2JJS0lzUm1VOXhheGJBPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI1NGM4YmQ2Yy02MDMxLTcwY2UtYWRjMS03ZGM3ZDcwOTRjNjMiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtbm9ydGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtbm9ydGhlYXN0LTJfWXBTMHpwMDlLIiwiY2xpZW50X2lkIjoiM2xyMW1zcGJtYzZwcmU4amtyaWZjMGFqajYiLCJvcmlnaW5fanRpIjoiNmJhZWY3ODAtZTZhZC00NDM1LWI1MWMtZjBmMjg0OTQ1NDM1IiwiZXZlbnRfaWQiOiJkZmRhN2JmMi0wYWQzLTQyMjYtYTkxYy1kOGVkMGU3YmU1Y2MiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIiwiYXV0aF90aW1lIjoxNzYzNDE0MjgyLCJleHAiOjE3NjM0MTc4ODIsImlhdCI6MTc2MzQxNDI4MiwianRpIjoiMTI2NDIxNTktZTBjNS00MjNhLTgwYzktNmQ5MjFiNmMwOGVjIiwidXNlcm5hbWUiOiJkZXZ0NmVAZ21haWwuY29tIn0.m7BDNjWCY7YEPOOspZ0arGwyPVGNab-zGmUs0L0c0kLur1x5wW0FGaOQvF42pDvgVBbYhhFndS3qpITZxxggpc3KudV6cZUs6fwC-3hLJ4m-P9iFidvEUfj7wK2FT0Qb1uDh8it3ILz80_ufbjV2LADBu2LDJKPxrWiUY2j371MiLBqYrtXuJ2-3adDs3RSiWbSjhIrVGblAK2b8IXPkw16pWcmJJakeKajCFEkjBb5a4ErV4x0cPnv9flhFPrkuDHxbKiFAPeYJLNK073ECXXRmqGEtt524YosotVAiU3yrQS-RpgB5HkXZbXyuiaaefYpG7ge8FoUo7YZOa8ZlLA";

        UnityWebRequest request = UnityWebRequest.Post(serverUrl, form);
        request.timeout = 300;   // 300초

        // request.SetRequestHeader("X-Authenticated-User-Email", "Bearer " + accessToken);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        // 요청 보내기
        yield return request.SendWebRequest();

        // 결과 판정
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("업로드 실패: " + request.error);
            yield break; // 이후 로직 실행 막기
        }

        // 서버 응답 로그
        Debug.Log("서버 응답: " + request.downloadHandler.text);

        // GLB 저장
        string localGlbPath = Path.Combine(Application.persistentDataPath, "result.glb");
        File.WriteAllBytes(localGlbPath, request.downloadHandler.data);

        Debug.Log("GLB 저장 완료 → " + localGlbPath);

        // GLB 불러오기
        yield return LoadGLBModel(localGlbPath);

        // request는 마지막에 Dispose
        request.Dispose();
    }

    public async Task LoadGLBModel(string path)
    {
        // 기존 오브젝트 제거
        foreach (Transform child in modelParent)
            Destroy(child.gameObject);

        var gltf = new GltfImport();

        bool success = await gltf.Load(path);

        if (!success)
        {
            Debug.LogError("GLB 로드 실패!");
            return;
        }

        // 새로운 권장 방식
        bool instantiated = await gltf.InstantiateMainSceneAsync(modelParent);

        modelParent.localScale = new Vector3(300f, 300f, 300f);


        if (!instantiated)
        {
            Debug.LogError("GLB 인스턴스 생성 실패!");
            return;
        }

        Debug.Log("모델 표시 완료!");
    }

    public async void OnClickLoadModel()
    {
        string path = Application.persistentDataPath + "/model.glb";
        await LoadGLBModel(path);
    }
}
