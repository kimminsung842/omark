using UnityEngine;
using UnityEngine.UI;

public class MarkerCreator : MonoBehaviour
{
    [Header("UI 연결")]
    public Button AddMarkerToolbarButton;

    private static int inventoryItemCount = 0;

    void Start()
    {
        if (AddMarkerToolbarButton != null)
        {
            AddMarkerToolbarButton.onClick.AddListener(HandleAddMarkerClick);
        }
    }

    private void HandleAddMarkerClick()
    {
        inventoryItemCount++;

        MarkerData newTempMarker = new MarkerData($"마커 {inventoryItemCount}", "Red");

        MarkerListUIController uiController = FindObjectOfType<MarkerListUIController>();

        if (uiController != null)
        {
            uiController.UpdateInventoryDisplay(newTempMarker);
        }
        else
        {
            Debug.LogError("MarkerListUIController를 찾을 수 없습니다. UI 관리 객체에 스크립트를 붙여주세요.");
        }

        Debug.Log($"[Creator] 툴바 버튼 클릭됨. 현재 총 마커 수: {inventoryItemCount}");
    }
}