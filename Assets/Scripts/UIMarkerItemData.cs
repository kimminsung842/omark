using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 드래그 인터페이스 사용
using TMPro; // TextMeshPro를 사용한다면 필요합니다. 

public class UIMarkerItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 이 UI 항목이 가지는 MarkerData 객체 전체를 참조합니다.
    private MarkerData _data;
    public MarkerData Data { get { return _data; } }

    // 더블 클릭 로직 변수
    private float lastClickTime = 0f;
    private const float DOUBLE_CLICK_TIME = 0.3f;

    // **[핵심]** 2D-3D 동기화를 위한 변수
    public static MarkerData markerDataToPlace;
    public GameObject linked3DMarker; // **3D 오브젝트 참조 (삭제 동기화용)**

    // 드래그 로직 변수
    private GameObject draggedImageObject; // 드래그 중인 시각적 피드백 오브젝트 (임시)
    private ScrollRect parentScrollRect;   // 상위 Scroll Rect 컴포넌트 (스크롤 차단용)

    [Header("UI 요소 연결")]
    public TextMeshProUGUI nameText;
    public Image colorIndicator;
    public GameObject favoriteIcon;

    // [추가] 드래그 시각화용 프리팹
    [Header("드래그 시각화 프리팹")]
    public GameObject dragVisualPrefab;

    public void Setup(MarkerData data)
    {
        _data = data;

        if (nameText != null)
        {
            nameText.text = data.Name;
        }

        // 즐겨찾기 (Check) 표시
        if (favoriteIcon != null)
        {
            // IsFavorite 값에 따라 아이콘을 활성화/비활성화
            favoriteIcon.SetActive(data.IsFavorite);
        }

        // 3. **핵심 수정: 색상 코드를 기반으로 이미지 교체**
        if (colorIndicator != null)
        {
            // MarkerColorImageManager의 Instance를 찾아 이미지 요청 (싱글톤 사용 가정)
            MarkerColorImageManager manager = MarkerColorImageManager.Instance;

            if (manager != null)
            {
                Sprite newSprite = manager.GetSpriteByColorCode(data.ColorCode);
                if (newSprite != null)
                {
                    colorIndicator.sprite = newSprite; // 이미지 교체
                    // 기존 색상 오버레이를 제거하고 이미지의 원래 색상을 표시합니다.
                    colorIndicator.color = Color.white;
                }
            }
            else
            {
                Debug.LogError("MarkerColorImageManager 인스턴스를 찾을 수 없습니다. 기본 색칠 로직으로 대체합니다.");

                // 관리자를 찾을 수 없을 때 임시로 Hex 색상을 적용하는 대체 로직
                if (ColorUtility.TryParseHtmlString(data.ColorCode, out Color newColor))
                {
                    colorIndicator.color = newColor;
                    colorIndicator.sprite = null;
                }
            }
        }

        Debug.Log($"[UI Data] 마커 UI에 데이터 저장 완료. ID: {data.Id}");
    }

    public void OnMarkerIconClicked()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
        {
            if (Data == null)
            {
                Debug.LogError("클릭된 마커 아이콘에 데이터가 없습니다.");
                return;
            }

            Debug.Log($"[Click Event] 마커 아이콘 클릭. ID: {Data.Id}, 이름: {Data.Name}");

            // 팝업 관리자에게 이 마커의 데이터를 전달하여 상세 팝업을 띄우도록 요청
            UIPopupManager popupManager = FindFirstObjectByType<UIPopupManager>();

            if (popupManager != null)
            {
                popupManager.ShowMarkerDetailPopup(Data);
            }
            else
            {
                Debug.LogError("UIPopupManager를 찾을 수 없습니다.");
            }

            lastClickTime = 0f;
        }
        else
        {
            // 싱글 클릭: 다음 더블 클릭을 위해 시간만 기록
            lastClickTime = Time.time;
            Debug.Log("[Click Event] 싱글 클릭: 다음 더블 클릭 대기 중...");
        }
    }

    // 1. 드래그 시작 시 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. 드래그 중인 데이터 저장
        if (parentScrollRect != null) parentScrollRect.enabled = false;
        markerDataToPlace = Data;

        // 2. **[수정]** 임시 드래그 이미지 생성
        if (dragVisualPrefab != null)
        {
            // 드래그 시각화용 2D 이미지를 Canvas의 루트에 생성
            draggedImageObject = Instantiate(dragVisualPrefab, transform.root);

            // 3. CanvasGroup 검사 및 레이캐스트 차단 (드롭 타겟에 방해 방지)
            CanvasGroup cg = draggedImageObject.GetComponent<CanvasGroup>();
            if (cg == null) cg = draggedImageObject.AddComponent<CanvasGroup>(); // 없으면 추가
            cg.blocksRaycasts = false;

            // 4. 위치 초기 설정
            draggedImageObject.transform.position = eventData.position;
        }
        else
        {
            Debug.LogError("Drag Visual Prefab이 UIMarkerItemData에 연결되지 않았습니다. 시각적 피드백이 없습니다.");
        }

        Debug.Log($"[Drag] 드래그 시작: ID {markerDataToPlace.Id}");
    }

    // 2. 드래그 중 호출
    public void OnDrag(PointerEventData eventData)
    {
        // 임시 이미지를 마우스 위치로 이동
        if (draggedImageObject != null)
        {
            draggedImageObject.transform.position = eventData.position;
        }
    }

    // 3. 드래그 종료 시 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        // 스크롤 재활성화
        if (parentScrollRect != null) parentScrollRect.enabled = true;

        // 임시 이미지 파괴
        if (draggedImageObject != null)
        {
            Destroy(draggedImageObject);
        }

        // **중요:** markerDataToPlace는 MarkerPlacer가 드롭 로직을 처리한 후 초기화해야 합니다.
    }
}