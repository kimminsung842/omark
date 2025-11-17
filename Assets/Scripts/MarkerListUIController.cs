using UnityEngine;
using UnityEngine.UI; // GridLayoutGroup 사용을 위해 추가
using System.Collections.Generic;

public class MarkerListUIController : MonoBehaviour
{
    // [UI 연결]
    [Header("UI 연결")]
    public Transform markerListContainer;
    public GameObject markerIconPrefab;

    [Header("뷰 제어")]
    public GameObject markerListPanel;

    [Header("패딩 조정 설정")]
    // Unity Inspector에서 Marker_Content에 붙어있는 GridLayoutGroup을 연결해야 합니다.
    public GridLayoutGroup markerContentGrid;

    public int threshold4Markers = 4; // 첫 번째 경계 (4개 미만)
    public int threshold8Markers = 8; // 두 번째 경계 (8개 미만)

    public int paddingLessThan4 = -300;      // 마커 수 < 4 일 때 적용
    public int padding4To7 = -150;           // 4 <= 마커 수 < 8 일 때 적용
    public int paddingGreaterEqual8 = 0;     // 마커 수 >= 8 일 때 적용

    private GameObject currentPlusButton;
    private List<GameObject> createdMarkerIcons = new List<GameObject>();

    void Start()
    {
        Transform plusTransform = transform.Find("Plus");
        if (plusTransform != null)
        {
            currentPlusButton = plusTransform.gameObject;
        }
        else
        {
            Debug.LogError("Hierarchy에서 이름이 'Plus'인 객체를 찾을 수 없습니다. 수동 배치된 Plus 버튼을 확인하고 이름을 'Plus'로 지정하세요.");
        }

        // 초기 시작 시 기본 패딩 값 적용
        AdjustGridPadding();
    }

    // ======================================================================
    // 마커 생성 시 호출되는 함수
    // ======================================================================
    public void UpdateInventoryDisplay(MarkerData newMarkerData)
    {
        if (currentPlusButton == null)
        {
            Debug.LogError("Plus 버튼 객체를 찾을 수 없어 갱신 로직을 실행할 수 없습니다.");
            return;
        }

        GameObject newMarkerIcon = Instantiate(
            markerIconPrefab,
            currentPlusButton.transform.position, // 위치를 복사
            Quaternion.identity,
            currentPlusButton.transform.parent // 부모를 복사
        );

        newMarkerIcon.transform.SetSiblingIndex(currentPlusButton.transform.GetSiblingIndex());

        // UIMarkerItemData 설정 로직 (이전 논의에서 구현됨)
        UIMarkerItemData uiItemData = newMarkerIcon.GetComponent<UIMarkerItemData>();
        if (uiItemData != null)
        {
            uiItemData.Setup(newMarkerData);
        }
        else
        {
            Debug.LogError("MarkerIconPrefab에 UIMarkerItemData 스크립트가 없습니다. 데이터를 저장할 수 없습니다.");
        }

        newMarkerIcon.name = newMarkerData.Name;
        createdMarkerIcons.Add(newMarkerIcon);

        currentPlusButton.transform.SetAsLastSibling();

        // 마커가 추가되었으므로 패딩 조정
        AdjustGridPadding();
    }

    // ======================================================================
    // 마커 삭제 시 호출되는 함수 (UIPopupManager에서 호출)
    // ======================================================================
    public void RemoveMarkerIcon(string markerId)
    {
        GameObject markerToRemove = null;

        // 1. createdMarkerIcons 리스트를 순회하며 해당 ID를 가진 UI 오브젝트를 찾습니다.
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            // UI 오브젝트에 붙어있는 UIMarkerItemData 스크립트에서 ID를 가져옵니다.
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data.Id == markerId)
            {
                markerToRemove = markerIcon;
                break;
            }
        }

        if (markerToRemove != null)
        {
            // 2. 리스트에서 제거
            createdMarkerIcons.Remove(markerToRemove);

            // 3. 씬에서 오브젝트 파괴
            Destroy(markerToRemove);

            // 마커가 제거되었으므로 패딩 조정
            AdjustGridPadding();

            Debug.Log($"[UI List] 마커 ID {markerId}의 UI 항목이 리스트에서 제거되었습니다.");
        }
    }

    // ======================================================================
    // 마커 개수에 따라 Grid Layout Group의 패딩을 조정하는 핵심 함수
    // ======================================================================
    private void AdjustGridPadding()
    {
        if (markerContentGrid == null) return;

        // 현재 마커의 총 개수 (Plus 버튼 제외)
        int markerCount = createdMarkerIcons.Count;

        // Grid Layout Group의 Padding 구조체 복사
        GridLayoutGroup grid = markerContentGrid;
        RectOffset padding = grid.padding;
        int newPaddingTop;

        if (markerCount < threshold4Markers) // 마커 수 < 4
        {
            newPaddingTop = paddingLessThan4; // -300
        }
        else if (markerCount < threshold8Markers) // 4 <= 마커 수 < 8
        {
            newPaddingTop = padding4To7; // -150
        }
        else // 마커 수 >= 8
        {
            newPaddingTop = paddingGreaterEqual8; // 0
        }

        if (padding.top != newPaddingTop)
        {
            padding.top = newPaddingTop;

            // 수정된 Padding 구조체를 다시 컴포넌트에 적용
            grid.padding = padding;
            Debug.Log($"마커 수 {markerCount}개. Top Padding을 {newPaddingTop}으로 설정.");
        }
    }

    // ======================================================================
    // 마커 편집 시 UI 갱신을 위해 호출되는 함수 (UIPopupManager에서 사용)
    // ======================================================================
    public void UpdateMarkerIconStatus(MarkerData updatedData)
    {
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data.Id == updatedData.Id)
            {
                // UIMarkerItemData의 Setup 함수를 호출하여 이름, 색상, 즐겨찾기 상태 갱신
                uiItemData.Setup(updatedData);
                markerIcon.name = updatedData.Name; // GameObject 이름도 갱신

                Debug.Log($"[UI List] 마커 ID {updatedData.Id}의 UI 상태가 갱신되었습니다.");
                return;
            }
        }
    }

    // 마커 리스트 패널의 활성화/비활성화를 제어하는 함수 (슬라이더 로직에서 사용)
    public void SetPanelVisibility(bool isVisible)
    {
        if (markerListPanel != null)
        {
            markerListPanel.SetActive(isVisible);
        }
    }
}