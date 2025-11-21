// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using UnityEngine.Networking;
// using System.IO;

// public class ImageHandler : MonoBehaviour
// {
//     [Header("UI References")]
//     public RawImage displayImage;
//     public Text feedbackText;
//     public Text imagePath;

//     [Header("Server Info")]
//     public string uploadUrl = "https://kemini-aws.duckdns.org/api/v1/files/upload"; // 🔹 서버 주소 입력

//     private Texture2D loadedTexture;
    

//     // -------------------------------
//     // 0. 갤러리에서 사진 선택
//     // -------------------------------
//     public void PickImage()
//     {
//         if(imagePath.text == null)
//         {
//             ShowFeedback("업로드할 이미지가 없습니다.", Color.red);
//             return;
//         }
//         Texture2D texture = Resources.Load<Texture2D>(imagePath.text);
//         loadedTexture = texture;
//         displayImage.texture = loadedTexture;

//         StartCoroutine(UploadImageToServer());
//     }


//     // -------------------------------
//     // 1. Texture2D → 서버 전송
//     // -------------------------------
//     private IEnumerator UploadImageToServer()
//     {
//         if (loadedTexture == null)
//         {
//             ShowFeedback("업로드할 이미지가 없습니다.", Color.red);
//             yield break;
//         }

//         ShowFeedback("이미지 업로드 중...", Color.yellow);

//         // 🔹 읽기 가능한 텍스처로 변환
//         Texture2D readableTexture = MakeTextureReadable(loadedTexture);

//         // 🔹 PNG 변환
//         byte[] imageBytes = readableTexture.EncodeToPNG();
        
//         // 🔹 multipart/form-data 전송을 위한 폼 생성
//         WWWForm form = new WWWForm();
//         form.AddBinaryData("file", imageBytes, Path.GetFileName(imagePath.text), "image/png");

//         string accessToken =  "eyJraWQiOiJFQk1jMXlEaXVOQTlsNTIwd00wK2VqZTk2RmxtN2JJS0lzUm1VOXhheGJBPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI3NGE4ZWQwYy1jMDMxLTcwYmEtMDNlZi1iNDM2NjU5ODk2ODgiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtbm9ydGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtbm9ydGhlYXN0LTJfWXBTMHpwMDlLIiwiY2xpZW50X2lkIjoiM2xyMW1zcGJtYzZwcmU4amtyaWZjMGFqajYiLCJvcmlnaW5fanRpIjoiZDg4YWIyMDctMzM0Ny00ZWM4LWIyMGMtYjM3YjQ1OTk0MWI3IiwiZXZlbnRfaWQiOiI1Y2YyYmYxOS03Y2I2LTQ5NWQtYmIzMi04MmY3ZDUzZDA4ZWMiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIiwiYXV0aF90aW1lIjoxNzYzMDIwNDc4LCJleHAiOjE3NjMwMjQwNzgsImlhdCI6MTc2MzAyMDQ3OCwianRpIjoiMzkyYjI0ZDQtYzM0Yy00NmMyLWJkODctNGYyYjgxNzEwNmQwIiwidXNlcm5hbWUiOiJ0YWVAdGFlLmNvbSJ9.pm_h0mqZ9rO4S45dybp4EglBzpUualsnSjd4SiMhL52V-Ytea2NTE9CMHBvxoT9yaphPZEaKpHI8vomQfX37a9U0_5r6g9l4pbp1ZLamZPcIAvU14TO-YUnav_L4AmLFuy9aYlfiT50eLW13IVNnQy4XZ66FXG9BDGZiNJUMAM09BLgXDska_S1waDQ3z0vW352chUSwynH499dNeJR7JsENXM87rBoJo3EXJwS0Cxx1CfqqhoPTdUC-Da-6PxqcEOtLas8ydUOLYgc06FwIRS50BEXnyZcLrqVE59f7Kxwrj5UvnuKfl76eNNNheupCVGDGJyF8_0txchWMvkMpQA";  // ... (로그인 시 저장한 Access Token) ...;
        

//         using (UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form))
//         {
//             // www.SetRequestHeader(CognitoHeaderAuthenticationFilter.AUTH_HEADER_KEY, "Bearer " + accessToken);
//             www.SetRequestHeader("Authorization", "Bearer " + accessToken);
//             yield return www.SendWebRequest();

//             if (www.result != UnityWebRequest.Result.Success)
//             {
//                 ShowFeedback($"업로드 실패: {www.error}", Color.red);
//             }
//             else
//             {
//                 ShowFeedback("이미지가 성공적으로 업로드되었습니다!", new Color(0.2f, 0.8f, 0.2f));
//                 Debug.Log($"서버 응답: {www.downloadHandler.text}");
//             }
//         }
//     }

//     // -------------------------------
//     // 2. JSON에서 obj_url 추출
//     // -------------------------------
//     string ExtractOBJUrl(string json)
//     {
//         // 매우 단순한 파싱 (JsonUtility로는 딕셔너리가 안돼서 수동 파싱)
//         string key = "\"obj_url\":";
//         int idx = json.IndexOf(key);
//         if (idx < 0) return null;

//         int start = json.IndexOf("\"", idx + key.Length) + 1;
//         int end = json.IndexOf("\"", start);

//         return json.Substring(start, end - start);
//     }

//     // -------------------------------
//     // 3. OBJ 파일 다운로드
//     // -------------------------------
//     IEnumerator DownloadOBJ(string url)
//     {
//         Debug.Log("OBJ 다운로드 시작: " + url);

//         using (UnityWebRequest www = UnityWebRequest.Get(url))
//         {
//             yield return www.SendWebRequest();

//             if (www.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.LogError("OBJ 다운로드 실패: " + www.error);
//                 Debug.LogError("응답: " + www.downloadHandler.text);
//                 yield break;
//             }

//             byte[] objBytes = www.downloadHandler.data;

//             string savePath = Application.persistentDataPath + "/mesh.obj";
//             File.WriteAllBytes(savePath, objBytes);

//             Debug.Log("OBJ 저장 완료: " + savePath);

//             // OBJ 파싱 후 Unity Mesh로 변환
//             LoadOBJToScene(savePath);
//         }
//     }
//         // -------------------------------
//     // 4. OBJ 파일을 Unity Mesh로 로드
//     // -------------------------------
//     void LoadOBJToScene(string path)
//     {
//         Debug.Log("OBJ 로드 시작: " + path);

//         // Unity는 OBJ 파일을 기본적으로 파싱할 수 없으므로
//         // SimpleOBJImporter 같은 파서 필요
//         // 기본적인 OBJ 로더 구현

//         Mesh mesh = SimpleOBJImporter.Import(path);
//         if (mesh == null)
//         {
//             Debug.LogError("OBJ 파싱 실패");
//             return;
//         }

//         GameObject obj = new GameObject("GeneratedMesh");
//         MeshFilter mf = obj.AddComponent<MeshFilter>();
//         MeshRenderer mr = obj.AddComponent<MeshRenderer>();

//         mf.mesh = mesh;
//         mr.material = meshMaterial;

//         obj.transform.position = Vector3.zero;
//         obj.transform.localScale = Vector3.one * 0.1f;

//         Debug.Log("OBJ 로드 완료!");
//     }
//     private void ShowFeedback(string message, Color color)
//     {
//         if (feedbackText != null)
//         {
//             feedbackText.text = message;
//             feedbackText.color = color;
//             StopAllCoroutines();
//             StartCoroutine(ClearFeedbackAfterDelay(3f));
//         }
//     }

//     private IEnumerator ClearFeedbackAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         if (feedbackText != null)
//             feedbackText.text = "";
//     }

//     private Texture2D MakeTextureReadable(Texture2D source)
//     {
//         RenderTexture rt = RenderTexture.GetTemporary(
//             source.width,
//             source.height,
//             0,
//             RenderTextureFormat.Default,
//             RenderTextureReadWrite.Linear);

//         Graphics.Blit(source, rt);
//         RenderTexture previous = RenderTexture.active;
//         RenderTexture.active = rt;

//         Texture2D readableTex = new Texture2D(source.width, source.height);
//         readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
//         readableTex.Apply();

//         RenderTexture.active = previous;
//         RenderTexture.ReleaseTemporary(rt);

//         return readableTex;
//     }
// }
