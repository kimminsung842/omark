using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MarkerListUIController : MonoBehaviour
{
    // [UI 연결]
    [Header("UI 연결")]
    public Transform markerListContainer;
    public GameObject markerIconPrefab;

    [Header("뷰 제어")]
    public GameObject markerListPanel;

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
    }

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

        newMarkerIcon.name = newMarkerData.Name;
        createdMarkerIcons.Add(newMarkerIcon);

        currentPlusButton.transform.SetAsLastSibling();

    }

    public void SetPanelVisibility(bool isVisible)
    {
        if (markerListPanel != null)
        {
            markerListPanel.SetActive(isVisible);
        }
    }
}