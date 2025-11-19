using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RoomItem : MonoBehaviour
{
    [Header("Groups")]
    public GameObject groupNormal;
    public GameObject groupEdit;

    [Header("Normal Mode Texts")]
    public TMP_Text txtNameNormal;
    public TMP_Text txtDateNormal;

    [Header("Edit Mode Elements")]
    public TMP_InputField inputNameEdit;
    public TMP_Text txtDateEdit;
    public Button btnDeleteEdit;
    public Toggle toggleSelect;

    // 삭제 콜백 (MainListUI가 등록)
    private Action onDeleteRequest;

    // -----------------------------------------------------
    // 초기 텍스트 설정
    // -----------------------------------------------------
    public void SetTexts(string name, string date)
    {
        if (txtNameNormal) txtNameNormal.text = name;
        if (txtDateNormal) txtDateNormal.text = date;

        if (inputNameEdit) inputNameEdit.text = name;
        if (txtDateEdit) txtDateEdit.text = date;
    }

    // -----------------------------------------------------
    // 이름 변경 저장 (편집모드 종료 시 호출)
    // -----------------------------------------------------
    public void ApplyEditedName()
    {
        if (inputNameEdit != null && txtNameNormal != null)
        {
            txtNameNormal.text = inputNameEdit.text;
        }
    }

    // -----------------------------------------------------
    // 모드 전환
    // -----------------------------------------------------
    public void SetEditMode(bool isEdit)
    {
        if (groupNormal) groupNormal.SetActive(!isEdit);
        if (groupEdit) groupEdit.SetActive(isEdit);
    }

    // -----------------------------------------------------
    // 편집모드 토글 관련
    // -----------------------------------------------------
    public void SetToggle(bool on)
    {
        if (toggleSelect != null)
            toggleSelect.isOn = on;
    }

    public bool IsSelected()
    {
        return toggleSelect != null && toggleSelect.isOn;
    }

    // -----------------------------------------------------
    // 단일 삭제 버튼 이벤트 연결
    // -----------------------------------------------------
    public void SetDeleteAction(Action deleteCallback)
    {
        onDeleteRequest = deleteCallback;

        if (btnDeleteEdit != null)
        {
            btnDeleteEdit.onClick.RemoveAllListeners();
            btnDeleteEdit.onClick.AddListener(() =>
            {
                onDeleteRequest?.Invoke();
            });
        }
    }

    // -----------------------------------------------------
    // 현재 이름 가져오기
    // -----------------------------------------------------
    public string GetEditedName()
    {
        return inputNameEdit != null ? inputNameEdit.text : "";
    }
}
