using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceApi : MonoBehaviour
{
    [SerializeField] ApiClient api;

    // ============ Auth Header 설정 ============
    private void ApplyAuth()
    {
        string token = PlayerPrefs.GetString("ACCESS_TOKEN", null);
        if (!string.IsNullOrEmpty(token))
            api.AccessToken = token;
    }

    // ============ GET 전체 환경 ============
    public IEnumerator GetAllEnvironments(
        System.Action<List<VirtualEnvironmentResponseDto>> onSuccess,
        System.Action<string> onError)
    {
        ApplyAuth();

        bool ok = false;
        string body = null;

        yield return api.Get("/api/v1/environments", (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("목록을 불러오지 못했습니다.");
            yield break;
        }

        var res = JsonUtility.FromJson<ApiResponse<List<VirtualEnvironmentResponseDto>>>(body);

        if (res != null && res.status == "success")
            onSuccess?.Invoke(res.data ?? new List<VirtualEnvironmentResponseDto>());
        else
            onError?.Invoke(res?.message ?? "응답 오류");
    }


    // ============ GET 상세 조회 ============
    public IEnumerator GetEnvironmentDetail(
        long envId,
        System.Action<VirtualEnvironmentResponseDto> onSuccess,
        System.Action<string> onError)
    {
        ApplyAuth();

        bool ok = false;
        string body = null;

        yield return api.Get($"/api/v1/environments/{envId}", (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("환경 상세 조회 실패");
            yield break;
        }

        var res = JsonUtility.FromJson<ApiResponse<VirtualEnvironmentResponseDto>>(body);

        if (res != null && res.status == "success")
            onSuccess?.Invoke(res.data);
        else
            onError?.Invoke(res?.message ?? "상세 조회 오류");
    }


    // ============ POST 생성 ============
    public IEnumerator CreateEnvironment(
        string name,
        System.Action<VirtualEnvironmentResponseDto> onSuccess,
        System.Action<string> onError)
    {
        ApplyAuth();

        var dto = new VirtualEnvironmentRequestDto { name = name };
        string json = JsonUtility.ToJson(dto);

        bool ok = false;
        string body = null;

        yield return api.Post("/api/v1/environments", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("공간 생성 실패");
            yield break;
        }

        var res = JsonUtility.FromJson<ApiResponse<VirtualEnvironmentResponseDto>>(body);

        if (res != null && res.status == "success")
            onSuccess?.Invoke(res.data);
        else
            onError?.Invoke(res?.message ?? "생성 오류");
    }


    // ============ PUT 이름 수정 ============
    public IEnumerator UpdateEnvironment(
        long envId, string newName,
        System.Action onSuccess,
        System.Action<string> onError)
    {
        ApplyAuth();

        var dto = new VirtualEnvironmentRequestDto { name = newName };
        string json = JsonUtility.ToJson(dto);

        bool ok = false;
        string body = null;

        yield return api.Put($"/api/v1/environments/{envId}", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("이름 수정 실패");
            yield break;
        }

        var res = JsonUtility.FromJson<ApiResponse<VirtualEnvironmentResponseDto>>(body);

        if (res != null && res.status == "success")
            onSuccess?.Invoke();
        else
            onError?.Invoke(res?.message ?? "수정 오류");
    }


    // ============ DELETE 삭제 ============
    public IEnumerator DeleteEnvironment(
        long envId,
        System.Action onSuccess,
        System.Action<string> onError)
    {
        ApplyAuth();

        bool ok = false;
        string body = null;

        yield return api.Delete($"/api/v1/environments/{envId}", (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("삭제 실패");
            yield break;
        }

        var res = JsonUtility.FromJson<ApiResponse<object>>(body);

        if (res != null && res.status == "success")
            onSuccess?.Invoke();
        else
            onError?.Invoke(res?.message ?? "삭제 오류");
    }


    // ============ ★ Presigned URL 요청 ============
    public IEnumerator RequestUploadUrl(
        long envId,
        string fileName,
        System.Action<S3PresignedUrlResponseDto> onSuccess,
        System.Action<string> onError)
    {
        ApplyAuth();

        var dto = new S3PresignedUrlRequestDto { fileName = fileName };
        string json = JsonUtility.ToJson(dto);

        bool ok = false;
        string body = null;

        yield return api.Post($"/api/v1/environments/{envId}/request-upload", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("S3 업로드 URL 요청 실패");
            yield break;
        }

        var res = JsonUtility.FromJson<ApiResponse<S3PresignedUrlResponseDto>>(body);

        if (res != null && res.status == "success")
            onSuccess?.Invoke(res.data);
        else
            onError?.Invoke(res?.message ?? "Presigned URL 오류");
    }
}
