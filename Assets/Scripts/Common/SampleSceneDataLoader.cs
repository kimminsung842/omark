using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SampleSceneDataLoader : MonoBehaviour
{
    private void Start()
    {
        long envId = SpaceSession.currentEnvironmentId;
        string s3Url = SpaceSession.currentS3Url;

        if (!string.IsNullOrEmpty(s3Url))
        {
            StartCoroutine(DownloadData(s3Url));
        }
    }

    IEnumerator DownloadData(string url)
    {
        var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("S3 다운로드 실패: " + req.error);
            yield break;
        }

        var data = req.downloadHandler.data;
        Debug.Log("다운로드 완료: " + data.Length + " bytes");

        Apply(data);
    }

    void Apply(byte[] bytes)
    {
        Debug.Log("환경 데이터 적용됨 (TODO)");
    }
}
