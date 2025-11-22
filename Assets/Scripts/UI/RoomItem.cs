using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomItem : MonoBehaviour
{
    [Header("Normal Mode")]
    public GameObject groupNormal;
    public TMP_Text txtName;
    public TMP_Text txtDate;

    [Header("Edit Mode")]
    public GameObject groupEdit;
    public TMP_InputField inputName;
    public Toggle toggleSelect;

    [Header("Buttons")]
    public Button btnEdit;
    public Button btnDelete;

    public long environmentId;
    public string s3FileUrl;

    private System.Action<RoomItem> onDelete;

    private void Start()
    {
        groupNormal.SetActive(true);
        groupEdit.SetActive(false);

        btnEdit.onClick.AddListener(OnClickEdit);
        btnDelete.onClick.AddListener(() => onDelete?.Invoke(this));
    }

    private void OnClickEdit()
    {
        SpaceSession.currentEnvironmentId = environmentId;
        SpaceSession.currentS3Url = s3FileUrl;
        SceneManager.LoadScene("sample");
    }

    public void SetTexts(string name, string date)
    {
        txtName.text = name;
        txtDate.text = date;
    }

    public void SetDeleteAction(System.Action<RoomItem> callback)
    {
        onDelete = callback;
    }

    public void SetEditMode(bool active)
    {
        groupNormal.SetActive(!active);
        groupEdit.SetActive(active);

        if (active)
            inputName.text = txtName.text;
    }

    public void SetToggle(bool value)
    {
        toggleSelect.isOn = value;
    }

    public bool IsSelected()
    {
        return toggleSelect.isOn;
    }

    public string GetEditedName()
    {
        return inputName.text;
    }

    public void ApplyEditedNameToNormal()
    {
        txtName.text = inputName.text;
    }
}
