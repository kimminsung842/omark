using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDragHandler : MonoBehaviour
{
    public GameObject roomPreviewPrefab;
    public GameObject wallPreviewPrefab;

    public GameObject roomFinalPrefab;
    public GameObject wallFinalPrefab;

    private Vector3 dragStartPos;
    private GameObject previewObj;
    private bool isDragging = false;

    private const float GRID_SIZE = 0.1f;  // 10cm 스냅
    private const float SNAP_ENDPOINT_THRESHOLD = 0.15f; // 15cm

    void Update()
    {
        // 1) 터치가 UI 위면 무시
        if (IsTouchOverUI()) return;

        // 2) Move 모드면 그리기 동작 안함
        if (ModeManager.Instance.CurrentMode == EditMode.Move)
            return;

        // ----------------------------
        // 터치 시작
        // ----------------------------
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = SnapToExistingWallEndpoint(
                SnapToGrid(GetWorldPos(Input.mousePosition))
            );

            StartPreview();
        }

        // ----------------------------
        // 드래그 중
        // ----------------------------
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentPos = SnapToGrid(GetWorldPos(Input.mousePosition));
            currentPos = SnapToExistingWallEndpoint(currentPos);

            UpdatePreview(currentPos);
        }

        // ----------------------------
        // 터치 종료 → 최종 생성
        // ----------------------------
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 endPos = SnapToExistingWallEndpoint(
                SnapToGrid(GetWorldPos(Input.mousePosition))
            );

            EndPreview(endPos);

            // 자동 Move 모드 복귀
            ModeManager.Instance.SetMode(EditMode.Move);
        }
    }

    // =========================================================
    // 📌 UI 위 터치 체크
    // =========================================================
    bool IsTouchOverUI()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(t.fingerId))
                return true;
        }
        return EventSystem.current.IsPointerOverGameObject();
    }

    // =========================================================
    // 📌 스크린 → XZ 평면 좌표 변환
    // =========================================================
    Vector3 GetWorldPos(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float dist))
            return ray.GetPoint(dist);

        return Vector3.zero;
    }

    // =========================================================
    // 📌 10cm 단위 GRID SNAP
    // =========================================================
    Vector3 SnapToGrid(Vector3 pos)
    {
        float x = Mathf.Round(pos.x / GRID_SIZE) * GRID_SIZE;
        float z = Mathf.Round(pos.z / GRID_SIZE) * GRID_SIZE;
        return new Vector3(x, pos.y, z);
    }

    // =========================================================
    // 📌 기존 벽 Endpoint SNAP (15cm)
    // =========================================================
    Vector3 SnapToExistingWallEndpoint(Vector3 pos)
    {
        foreach (var wall in WallManager.Instance.walls)
        {
            if (Vector3.Distance(pos, wall.start) < SNAP_ENDPOINT_THRESHOLD)
                return wall.start;

            if (Vector3.Distance(pos, wall.end) < SNAP_ENDPOINT_THRESHOLD)
                return wall.end;
        }

        return pos;
    }

    // =========================================================
    // 📌 드래그 시작
    // =========================================================
    void StartPreview()
    {
        isDragging = true;

        if (ModeManager.Instance.CurrentMode == EditMode.Room)
            previewObj = Instantiate(roomPreviewPrefab);

        if (ModeManager.Instance.CurrentMode == EditMode.Wall)
            previewObj = Instantiate(wallPreviewPrefab);
    }

    // =========================================================
    // 📌 드래그 중 (미리보기 업데이트)
    // =========================================================
    void UpdatePreview(Vector3 currentPos)
    {
        if (!previewObj) return;

        Vector3 center = (dragStartPos + currentPos) / 2f;
        previewObj.transform.position = center;

        // ------------------ 방 (사각형) ------------------
        if (ModeManager.Instance.CurrentMode == EditMode.Room)
        {
            Vector3 size = new Vector3(
                Mathf.Abs(currentPos.x - dragStartPos.x),
                0.1f,
                Mathf.Abs(currentPos.z - dragStartPos.z)
            );

            previewObj.transform.localScale = size;
        }

        // ------------------ 벽 (선분) ------------------
        else if (ModeManager.Instance.CurrentMode == EditMode.Wall)
        {
            float length = Vector3.Distance(
                new Vector3(dragStartPos.x, 0, dragStartPos.z),
                new Vector3(currentPos.x, 0, currentPos.z)
            );

            Vector3 dir = (currentPos - dragStartPos).normalized;

            // 각도 계산
            float rawAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            // 5도 단위 스냅
            float snappedAngle = Mathf.Round(rawAngle / 5f) * 5f;

            previewObj.transform.rotation = Quaternion.Euler(0, snappedAngle, 0);

            previewObj.transform.localScale = new Vector3(
                0.1f,
                0.1f,
                length
            );
        }
    }

    // =========================================================
    // 📌 드래그 종료 → 최종 생성 + 벽 데이터 등록
    // =========================================================
    void EndPreview(Vector3 endPos)
    {
        if (previewObj) Destroy(previewObj);
        isDragging = false;

        Vector3 center = (dragStartPos + endPos) / 2f;

        // ------------------ 방 생성 ------------------
        if (ModeManager.Instance.CurrentMode == EditMode.Room)
        {
            Vector3 size = new Vector3(
                Mathf.Abs(endPos.x - dragStartPos.x),
                0.1f,
                Mathf.Abs(endPos.z - dragStartPos.z)
            );

            GameObject obj = Instantiate(roomFinalPrefab, center, Quaternion.identity);
            obj.transform.localScale = size;
        }

        // ------------------ 벽 생성 ------------------
        else if (ModeManager.Instance.CurrentMode == EditMode.Wall)
        {
            float length = Vector3.Distance(
                new Vector3(dragStartPos.x, 0, dragStartPos.z),
                new Vector3(endPos.x, 0, endPos.z)
            );

            Vector3 dir = (endPos - dragStartPos).normalized;

            float rawAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(rawAngle / 5f) * 5f;

            GameObject obj = Instantiate(wallFinalPrefab);

            obj.transform.position = center;
            obj.transform.rotation = Quaternion.Euler(0, snappedAngle, 0);
            obj.transform.localScale = new Vector3(0.1f, 0.1f, length);

            // 벽 정보 저장
            WallManager.Instance.AddWall(dragStartPos, endPos);
        }
    }
}
