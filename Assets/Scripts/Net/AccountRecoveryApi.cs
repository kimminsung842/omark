using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AccountRecoveryApi : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:8080";

    [Serializable]
    public class FindEmailRequest
    {
        public string phoneNumber;
        public int askId;
        public string askAnswer;
    }

    [Serializable]
    public class FindEmailResponseData
    {
        public string email;
    }

    [Serializable]
    public class FindEmailResponse
    {
        public string status;
        public string message;
        public FindEmailResponseData data;
        public string error;
    }

    public void FindEmail(string phoneNumber, int askId, string askAnswer, Action<FindEmailResponse> onCompleted, Action<string> onError)
    {
        FindEmailRequest req = new FindEmailRequest
        {
            phoneNumber = phoneNumber,
            askId = askId,
            askAnswer = askAnswer
        };

        StartCoroutine(PostJson("/api/v1/auth/find-email", req, onCompleted, onError));
    }

    IEnumerator PostJson(string path, FindEmailRequest body, Action<FindEmailResponse> onCompleted, Action<string> onError)
    {
        string url = baseUrl + path;
        string json = JsonUtility.ToJson(body);

        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        string text = request.downloadHandler.text;
        FindEmailResponse res = null;

        try
        {
            res = JsonUtility.FromJson<FindEmailResponse>(text);
        }
        catch (Exception e)
        {
            onError?.Invoke("Parse error: " + e.Message);
            yield break;
        }

        if (res == null)
        {
            onError?.Invoke("Empty response");
            yield break;
        }

        onCompleted?.Invoke(res);
    }
}
