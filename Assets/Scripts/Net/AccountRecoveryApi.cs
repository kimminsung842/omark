using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AccountRecoveryApi : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:8080";

    // -----------------------------------------------------------
    // 공통 응답 구조
    // -----------------------------------------------------------
    [Serializable]
    public class ApiResponse
    {
        public string status;
        public string message;
        public ResponseData data;
    }

    [Serializable]
    public class ResponseData
    {
        public int askId;
        public string email;
    }

    // -----------------------------------------------------------
    // 요청 DTO
    // -----------------------------------------------------------
    [Serializable]
    public class FindAskIdRequest
    {
        public string phoneNumber;
    }

    [Serializable]
    public class FindEmailRequest
    {
        public string phoneNumber;
        public int askId;
        public string askAnswer;
    }

    [Serializable]
    public class FindPasswordStep1Request
    {
        public string email;
    }

    [Serializable]
    public class FindPasswordStep2Request
    {
        public string email;
        public int askId;
        public string askAnswer;
    }

    [Serializable]
    public class ResetPasswordByQuestionRequest
    {
        public string email;
        public int askId;
        public string askAnswer;
        public string newPassword;
    }

    // -----------------------------------------------------------
    // 아이디 찾기 API
    // -----------------------------------------------------------
    public void FindAskId(string phoneNumber,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        var req = new FindAskIdRequest { phoneNumber = phoneNumber };
        StartCoroutine(PostJson("/api/v1/auth/find-ask-id", req, onCompleted, onError));
    }

    public void FindEmail(string phoneNumber, int askId, string askAnswer,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        var req = new FindEmailRequest
        {
            phoneNumber = phoneNumber,
            askId = askId,
            askAnswer = askAnswer
        };
        StartCoroutine(PostJson("/api/v1/auth/find-email", req, onCompleted, onError));
    }

    // -----------------------------------------------------------
    // 비밀번호 찾기 Step1
    // -----------------------------------------------------------
    public void FindPasswordStep1(string email,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        var req = new FindPasswordStep1Request { email = email };
        StartCoroutine(PostJson("/api/v1/auth/find-password/step1", req, onCompleted, onError));
    }

    // -----------------------------------------------------------
    // 비밀번호 찾기 Step2
    // -----------------------------------------------------------
    public void FindPasswordStep2(string email, int askId, string answer,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        var req = new FindPasswordStep2Request
        {
            email = email,
            askId = askId,
            askAnswer = answer
        };
        StartCoroutine(PostJson("/api/v1/auth/find-password/step2", req, onCompleted, onError));
    }

    // -----------------------------------------------------------
    // 비밀번호 재설정
    // -----------------------------------------------------------
    public void ResetPasswordByQuestion(string email, int askId, string askAnswer, string newPw,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        var req = new ResetPasswordByQuestionRequest
        {
            email = email,
            askId = askId,
            askAnswer = askAnswer,
            newPassword = newPw
        };

        StartCoroutine(PostJson("/api/v1/auth/reset-password-by-question",
            req, onCompleted, onError));
    }

    // -----------------------------------------------------------
    // 공통 POST JSON 메서드
    // -----------------------------------------------------------
    IEnumerator PostJson(string path, object body,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        string url = baseUrl + path;
        string json = JsonUtility.ToJson(body);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        var text = request.downloadHandler.text;
        ApiResponse res;

        try
        {
            res = JsonUtility.FromJson<ApiResponse>(text);
        }
        catch (Exception e)
        {
            onError?.Invoke("Json parse error: " + e.Message);
            yield break;
        }

        onCompleted?.Invoke(res);
    }
}
