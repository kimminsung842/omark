// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using UnityEngine.Networking;
// using System.IO;

// public class Creator : MonoBehaviour
// {
//     [Header("UI References")]
//     public RawImage displayImage;
//     public Text feedbackText;
//     private Texture2D loadedTexture;
//     public string serverUrl = ""; 
//     public void PickImage()
//     {
//         // 갤러리 열기
//         NativeGallery.GetImageFromGallery((path) =>
//         {
//             if (path == null)
//             {
//                 ShowFeedback("이미지 선택이 취소되었습니다.", Color.gray);
//                 return;
//             }

//             // 이미지 불러오기
//             Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024);
//             if (texture == null)
//             {
//                 ShowFeedback("이미지를 불러올 수 없습니다.", Color.red);
//                 return;
//             }

//             loadedTexture = texture;
//             displayImage.texture = loadedTexture;
//             displayImage.color = Color.white;

//             ShowFeedback("이미지가 성공적으로 로드되었습니다!", new Color(0.2f, 0.8f, 0.2f));
//         },
//         "이미지를 선택하세요",
//         "image/*");
//     }

//     IEnumerator SendToTripoSR(Texture2D tex)
//     {
//         if (tex == null)
//         {
//             ShowFeedback("업로드할 이미지가 없습니다.", Color.red);
//             yield break;
//         }

//         ShowFeedback("이미지 업로드 중...", Color.yellow);

//         Texture2D readableTexture = MakeTextureReadable(loadedTexture);
//         // Texture2D → PNG (또는 JPG)
//         byte[] imageBytes = readableTexture.EncodeToPNG();  

//         WWWForm form = new WWWForm();
//         form.AddBinaryData("file", imageBytes, "input.png", "image/png");

//         using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
//         {
//             Debug.Log("이미지 업로드 중...");
//             yield return www.SendWebRequest();

//             if (www.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.LogError("TripoSR 요청 실패: " + www.error);
//                 Debug.LogError("서버 응답: " + www.downloadHandler.text);
//                 yield break;
//             }

//             string json = www.downloadHandler.text;
//             Debug.Log("서버 응답: " + json);

//             // 서버 응답을 파싱해서 obj_url 가져오기
//             string objUrl = ExtractOBJUrl(json);

//             if (string.IsNullOrEmpty(objUrl))
//             {
//                 Debug.LogError("obj_url을 응답에서 찾지 못함");
//                 yield break;
//             }

//             // OBJ 다운로드 진행
//             StartCoroutine(DownloadOBJ(objUrl));
//         }
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


//     private void ShowFeedback(string message, Color color)
//     {
//         if (feedbackText != null)
//         {
//             feedbackText.text = message;
//             feedbackText.color = color;
//             StartCoroutine(ClearFeedbackAfterDelay(3f));
//         }
//     }

//     private IEnumerator ClearFeedbackAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         if (feedbackText != null)
//             feedbackText.text = "";
//     }
// }
