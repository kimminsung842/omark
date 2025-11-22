using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainListUI : MonoBehaviour
{
    [Header("API")]
    public SpaceApi spaceApi;

    [Header("Popup Create")]
    public GameObject popupCreateRoom;
    public TMP_InputField inputRoomName;
    public Button popupCreateCancel;
    public Button popupCreateOK;

    [Header("Scroll List")]
    public Transform listContent;
    public GameObject roomItemPrefab;

    [Header("Panels")]
    public GameObject panelListNormal;
    public GameObject panelListEdit;

    [Header("Buttons")]
    public Button btnOpenCreate;
    public Button btnOpenEdit;
    public Button btnCloseEdit;
    public Button btnDeleteChecked;

    [Header("Empty Text")]
    public GameObject txtEmpty;

    [Header("Popup Confirm Delete")]
    public GameObject popupConfirm;
    public Button popupConfirmOK;
    public Button popupConfirmCancel;
    public TMP_Text popupConfirmTitle;

    enum DeleteMode { None, Single, Multiple }
    DeleteMode deleteMode = DeleteMode.None;

    private readonly List<RoomItem> pendingDeleteList = new();


    // ===============================================================
    // Start — 앱 시작 시 공간 목록 조회
    // ===============================================================
    private void Start()
    {
        popupCreateRoom.SetActive(false);
        popupConfirm.SetActive(false);

        btnOpenCreate.onClick.AddListener(OpenCreatePopup);
        popupCreateCancel.onClick.AddListener(CloseCreatePopup);
        popupCreateOK.onClick.AddListener(OnCreateConfirm);

        btnOpenEdit.onClick.AddListener(OpenEditMode);
        btnCloseEdit.onClick.AddListener(CloseEditMode);

        btnDeleteChecked.onClick.AddListener(OnClickDeleteChecked);

        popupConfirmOK.onClick.AddListener(OnConfirmDelete);
        popupConfirmCancel.onClick.AddListener(CloseConfirmPopup);

        StartCoroutine(LoadEnvironmentList());
    }


    // ===============================================================
    // LIST 불러오기
    // ===============================================================
    private IEnumerator LoadEnvironmentList()
    {
        // 기존 리스트 제거
        foreach (Transform c in listContent)
            Destroy(c.gameObject);

        yield return spaceApi.GetAllEnvironments(
            onSuccess: (list) =>
            {
                foreach (var env in list)
                {
                    CreateRoomItemFromServer(env);
                }

                RefreshEmptyText();
                StartCoroutine(RebuildNextFrame());
            },
            onError: (msg) =>
            {
                Debug.LogError(msg);
            });
    }


    // 서버 응답 DTO → RoomItem 생성
    private void CreateRoomItemFromServer(VirtualEnvironmentResponseDto env)
    {
        GameObject go = Instantiate(roomItemPrefab, listContent);
        var item = go.GetComponent<RoomItem>();

        // 날짜 대신 CreatedAt 미제공이므로 현재 시간으로 표시
        string nowDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        item.SetTexts(env.name, nowDate);
        item.environmentId = env.id;
        item.s3FileUrl = env.s3FileUrl;

        item.SetDeleteAction((roomItem) =>
        {
            pendingDeleteList.Clear();
            pendingDeleteList.Add(roomItem);
            OpenConfirmPopup(DeleteMode.Single);
        });
    }



    // ===============================================================
    // CREATE (새 공간 생성)
    // ===============================================================
    private void OpenCreatePopup()
    {
        popupCreateRoom.SetActive(true);
        inputRoomName.text = "";
        inputRoomName.ActivateInputField();
    }

    private void CloseCreatePopup()
    {
        popupCreateRoom.SetActive(false);
    }

    private void OnCreateConfirm()
    {
        string name = string.IsNullOrEmpty(inputRoomName.text)
            ? "새로운 공간"
            : inputRoomName.text;

        StartCoroutine(spaceApi.CreateEnvironment(
            name,
            onSuccess: (env) =>
            {
                CreateRoomItemFromServer(env);

                popupCreateRoom.SetActive(false);
                RefreshEmptyText();
                StartCoroutine(RebuildNextFrame());
            },
            onError: (msg) =>
            {
                Debug.LogError(msg);
            }));
    }



    // ===============================================================
    // EDIT MODE
    // ===============================================================
    private void OpenEditMode()
    {
        panelListNormal.SetActive(false);
        panelListEdit.SetActive(true);

        foreach (Transform t in listContent)
        {
            var item = t.GetComponent<RoomItem>();
            if (item != null)
            {
                item.SetEditMode(true);
                item.SetToggle(false);
            }
        }
        StartCoroutine(RebuildNextFrame());
    }

    private void CloseEditMode()
    {
        panelListEdit.SetActive(false);
        panelListNormal.SetActive(true);

        StartCoroutine(SaveEditedNames());

        StartCoroutine(RebuildNextFrame());
        RefreshEmptyText();
    }

    private IEnumerator SaveEditedNames()
    {
        foreach (Transform t in listContent)
        {
            var item = t.GetComponent<RoomItem>();
            if (item != null)
            {
                string newName = item.GetEditedName();
                long envId = item.environmentId;

                item.ApplyEditedNameToNormal();
                item.SetEditMode(false);

                yield return spaceApi.UpdateEnvironment(
                    envId,
                    newName,
                    onSuccess: () => { },
                    onError: (msg) => { Debug.LogError(msg); });
            }
        }
    }



    // ===============================================================
    // DELETE — 체크된 항목 삭제
    // ===============================================================
    private void OnClickDeleteChecked()
    {
        pendingDeleteList.Clear();

        foreach (Transform t in listContent)
        {
            var item = t.GetComponent<RoomItem>();

            if (item != null && item.IsSelected())
            {
                pendingDeleteList.Add(item);
            }
        }

        if (pendingDeleteList.Count == 0)
            return;

        OpenConfirmPopup(DeleteMode.Multiple);
    }



    // ===============================================================
    // CONFIRM POPUP
    // ===============================================================
    private void OpenConfirmPopup(DeleteMode mode)
    {
        deleteMode = mode;

        popupConfirmTitle.text =
            (mode == DeleteMode.Single)
            ? "정말 삭제하시겠습니까?"
            : "선택한 공간들을 삭제하시겠습니까?";

        popupConfirm.SetActive(true);
    }

    private void CloseConfirmPopup()
    {
        popupConfirm.SetActive(false);
        deleteMode = DeleteMode.None;
        pendingDeleteList.Clear();
    }



    // ===============================================================
    // DELETE — 서버로 삭제 요청 수행
    // ===============================================================
    private void OnConfirmDelete()
    {
        StartCoroutine(DeleteSelectedRooms());
    }

    private IEnumerator DeleteSelectedRooms()
    {
        foreach (var item in pendingDeleteList)
        {
            long envId = item.environmentId;

            yield return spaceApi.DeleteEnvironment(
                envId,
                onSuccess: () =>
                {
                    Destroy(item.gameObject);
                },
                onError: (msg) =>
                {
                    Debug.LogError(msg);
                });
        }

        pendingDeleteList.Clear();
        popupConfirm.SetActive(false);

        RefreshEmptyText();
        StartCoroutine(RebuildNextFrame());
    }



    // ===============================================================
    // Utility
    // ===============================================================
    private void RefreshEmptyText()
    {
        txtEmpty.SetActive(listContent.childCount == 0);
    }

    private IEnumerator RebuildNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent as RectTransform);
        Canvas.ForceUpdateCanvases();
    }
}
