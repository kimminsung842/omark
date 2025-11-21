using UnityEngine;
using TMPro; // 3D TextMeshPro 사용 가정

public class MarkerVisualSync : MonoBehaviour
{
    // [Inspector Connection]
    [Header("3D 비주얼 요소")]
    public Renderer markerRenderer; // 색상을 변경할 3D 모델의 Renderer
    public TextMeshPro nameTagText; // 3D 이름 태그 (선택 사항)

    private ARMarkerData arData;

    void Start()
    {
        arData = GetComponent<ARMarkerData>();
        if (arData == null)
        {
            Debug.LogError("MarkerVisualSync는 ARMarkerData와 같은 오브젝트에 있어야 합니다.");
            enabled = false;
            return;
        }
        // 초기 배치 시 비주얼 업데이트
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (arData == null || arData.fullMarkerData == null) return;

        MarkerData data = arData.fullMarkerData;

        // 1. **이름 출력 (Name Tag)**
        if (nameTagText != null)
        {
            nameTagText.text = data.Name;
        }

        // 2. **색상 변경 (Color)**
        if (markerRenderer != null)
        {
            Color newColor;
            // Hex 코드(string)를 Color 객체로 변환합니다.
            if (ColorUtility.TryParseHtmlString(data.ColorCode, out newColor))
            {
                // Material의 색상을 변경합니다.
                markerRenderer.material.color = newColor;
            }
        }

        // TODO: data.IsFavorite 상태에 따라 3D 이펙트 활성화/비활성화 로직 추가
        Debug.Log($"[VisualSync] 3D 마커 비주얼 업데이트 완료: {data.Name}");
    }
}