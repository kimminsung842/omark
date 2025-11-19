using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainListUI : MonoBehaviour
{
    [Header("Create Popup")]
    public GameObject popupCreateRoom;
    public TMP_InputField inputRoomName;
    public Button btnCancel;
    public Button btnOK;

    [Header("List (Content)")]
    public Transform listContent;
    public GameObject roomItemPrefab;

    [Header("Panels")]
    public GameObject panelListNormal;
    public GameObject panelListEdit;

    [Header("Buttons")]
    public Button btnOpenCreatePopup;
    public Button btnOpenEdit;
    public Button btnCloseEdit;
    public Button btnDeleteChecked;

    [Header("Empty Text")]
    public GameObject txtEmpty;

    [Header("Delete Confirm Popup")]
    public GameObject popupConfirm;
    public Button popupConfirmOk;
    public Button popupConfirmCancel;
    public TMP_Text popupConfirmTitle;

    enum DeleteMode { None, Single, Multiple }
    DeleteMode pendingDeleteMode = DeleteMode.None;

    readonly List<RoomItem> pendingDeleteItems = new List<RoomItem>();


    void Awake()
    {
        // 초기 팝업 비활성화
        if (popupCreateRoom) popupCreateRoom.SetActive(false);
        if (popupConfirm) popupConfirm.SetActive(false);

        // 새 공간 생성 UI
        if (btnOpenCreatePopup) btnOpenCreatePopup.onClick.AddListener(OpenCreatePopup);
        if (btnCancel) btnCancel.onClick.AddListener(CloseCreatePopup);
        if (btnOK) btnOK.onClick.AddListener(CreateRoom);

        // 편집 모드
        if (btnOpenEdit) btnOpenEdit.onClick.AddListener(OpenEditMode);
        if (btnCloseEdit) btnCloseEdit.onClick.AddListener(CloseEditMode);

        // 일괄 삭제 버튼
        if (btnDeleteChecked) btnDeleteChecked.onClick.AddListener(OnClickDeleteChecked);

        // 삭제 확인 팝업
        if (popupConfirmOk) popupConfirmOk.onClick.AddListener(OnConfirmDelete);
        if (popupConfirmCancel) popupConfirmCancel.onClick.AddListener(CloseConfirmPopup);
    }

    void Start()
    {
        RefreshEmptyText();
    }

    // ------------------------------------------------------------
    // 1) 새 공간 생성
    // ------------------------------------------------------------
    void OpenCreatePopup()
    {
        popupCreateRoom.SetActive(true);
        inputRoomName.text = "";
        inputRoomName.ActivateInputField();
    }

    void CloseCreatePopup()
    {
        popupCreateRoom.SetActive(false);
    }

    void CreateRoom()
    {
        string nameToUse = "새 공간";
        if (!string.IsNullOrWhiteSpace(inputRoomName.text))
            nameToUse = inputRoomName.text.Trim();

        GameObject go = Instantiate(roomItemPrefab, listContent);
        go.name = "RoomItem_" + nameToUse;

        var item = go.GetComponent<RoomItem>();
        if (item != null)
        {
            item.SetTexts(nameToUse, GetNowDateString());
            item.SetEditMode(false);

            // 단일 삭제 버튼 콜백 연결
            item.SetDeleteAction(() =>
            {
                pendingDeleteItems.Clear();
                pendingDeleteItems.Add(item);
                OpenConfirmPopup(DeleteMode.Single);
            });
        }

        CloseCreatePopup();
        RefreshEmptyText();
        StartCoroutine(RebuildNextFrame());
    }

    // ------------------------------------------------------------
    // 2) 편집 모드 전환
    // ------------------------------------------------------------
    void OpenEditMode()
    {
        panelListNormal.SetActive(false);
        panelListEdit.SetActive(true);

        foreach (Transform child in listContent)
        {
            var item = child.GetComponent<RoomItem>();
            if (item != null)
            {
                item.SetEditMode(true);
                item.SetToggle(false); // 편집모드 진입 시 선택 초기화
            }
        }

        StartCoroutine(RebuildNextFrame());
    }

    void CloseEditMode()
    {
        // 편집모드에서 변경한 이름을 모두 저장
        foreach (Transform child in listContent)
        {
            var item = child.GetComponent<RoomItem>();
            if (item != null)
            {
                item.ApplyEditedName();  // 이름 저장
                item.SetEditMode(false); // 기본모드로 복귀
            }
        }

        panelListEdit.SetActive(false);
        panelListNormal.SetActive(true);

        RefreshEmptyText();
        StartCoroutine(RebuildNextFrame());
    }


    // ------------------------------------------------------------
    // 3) 다중삭제 버튼
    // ------------------------------------------------------------
    void OnClickDeleteChecked()
    {
        pendingDeleteItems.Clear();

        foreach (Transform child in listContent)
        {
            var item = child.GetComponent<RoomItem>();
            if (item != null && item.IsSelected())
                pendingDeleteItems.Add(item);
        }

        if (pendingDeleteItems.Count == 0)
            return;

        OpenConfirmPopup(DeleteMode.Multiple);
    }

    // ------------------------------------------------------------
    // 4) 삭제 팝업
    // ------------------------------------------------------------
    void OpenConfirmPopup(DeleteMode mode)
    {
        pendingDeleteMode = mode;

        popupConfirmTitle.text =
            (mode == DeleteMode.Single)
            ? "정말 삭제하시겠습니까?"
            : "선택한 공간들을 삭제하시겠습니까?";

        popupConfirm.SetActive(true);
    }

    void CloseConfirmPopup()
    {
        popupConfirm.SetActive(false);
        pendingDeleteMode = DeleteMode.None;
        pendingDeleteItems.Clear();
    }

    // ------------------------------------------------------------
    // 5) 삭제 확정
    // ------------------------------------------------------------
    void OnConfirmDelete()
    {
        foreach (var item in pendingDeleteItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        pendingDeleteItems.Clear();
        pendingDeleteMode = DeleteMode.None;

        CloseConfirmPopup();
        RefreshEmptyText();
        StartCoroutine(RebuildNextFrame());
    }

    // ------------------------------------------------------------
    // 6) Helper
    // ------------------------------------------------------------
    string GetNowDateString()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    bool HasAnyRoom()
    {
        return listContent.childCount > 0;
    }

    void RefreshEmptyText()
    {
        txtEmpty.SetActive(!HasAnyRoom());
    }

    IEnumerator RebuildNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent as RectTransform);
        Canvas.ForceUpdateCanvases();
    }
}
