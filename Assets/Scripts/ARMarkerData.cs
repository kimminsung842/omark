using UnityEngine;
using System;

public class ARMarkerData : MonoBehaviour
{
    public MarkerData fullMarkerData;

    [Header("연결된 UI 마커 데이터의 ID")]
    // 이 3D 마커가 UI 리스트에 있는 2D 마커와 연결되는 고유 ID입니다.
    public string markerId;

    [Header("3D 공간 정보")]
    public Vector3 worldPosition;
    public Quaternion worldRotation;

    // 초기화 함수: MarkerPlacer.cs에서 3D 마커를 생성할 때 호출됩니다.
    public void Initialize(MarkerData data, Vector3 position, Quaternion rotation)
    {
        this.fullMarkerData = data; // MarkerData 객체 전체 저장
        this.markerId = data.Id;    // MarkerData의 ID를 추출하여 저장
        this.worldPosition = position;
        this.worldRotation = rotation;

        // 실제 GameObject의 위치와 회전도 설정합니다.
        transform.position = position;
        transform.rotation = rotation;

        Debug.Log($"[AR Data] 3D 마커 초기화 완료. ID: {this.markerId}"); // string인 ID 사용

        // TODO: 이제 이 정보를 데이터베이스에 저장하는 로직을 호출해야 합니다.
    }
}