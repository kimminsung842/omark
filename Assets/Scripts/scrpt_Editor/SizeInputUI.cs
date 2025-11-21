using UnityEngine;
using TMPro;

public class SizeInputUI : MonoBehaviour
{
    public static SizeInputUI Instance;

    public GameObject panel;

    public TMP_InputField widthField;   // Room width (cm)
    public TMP_InputField heightField;  // Room height (cm)
    public TMP_InputField lengthField;  // Wall length (cm)

    private EditableObject currentObj;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // -------------------------------
    // UI 표시
    // -------------------------------
    public void Show(EditableObject obj)
    {
        currentObj = obj;
        panel.SetActive(true);

        if (obj.type == ObjectType.Room)
        {
            widthField.gameObject.SetActive(true);
            heightField.gameObject.SetActive(true);
            lengthField.gameObject.SetActive(false);

            widthField.text  = (obj.width  * 100f).ToString("F0"); // m → cm
            heightField.text = (obj.height * 100f).ToString("F0");
        }
        else if (obj.type == ObjectType.Wall)
        {
            widthField.gameObject.SetActive(false);
            heightField.gameObject.SetActive(false);
            lengthField.gameObject.SetActive(true);

            lengthField.text = (obj.length * 100f).ToString("F0");
        }
    }

    // -------------------------------
    // 적용하기 버튼
    // -------------------------------
    public void Apply()
    {
        if (currentObj == null) return;

        if (currentObj.type == ObjectType.Room)
        {
            float widthCm  = float.Parse(widthField.text);
            float heightCm = float.Parse(heightField.text);

            float widthM  = widthCm  / 100f;   // cm → m
            float heightM = heightCm / 100f;

            ResizeRoom(currentObj, widthM, heightM);
        }
        else if (currentObj.type == ObjectType.Wall)
        {
            float lengthCm = float.Parse(lengthField.text);
            float lengthM  = lengthCm / 100f;  // cm → m

            ResizeWall(currentObj, lengthM);
        }

        panel.SetActive(false);
        currentObj = null;
    }

    // -------------------------------
    // 취소 버튼 → 생성된 객체 삭제
    // -------------------------------
    public void Cancel()
    {
        if (currentObj != null)
        {
            Destroy(currentObj.gameObject);
            currentObj = null;
        }

        panel.SetActive(false);
    }

    // -------------------------------
    // 실제 크기 변경 함수
    // -------------------------------
    void ResizeRoom(EditableObject obj, float w, float h)
    {
        obj.width = w;
        obj.height = h;

        obj.transform.localScale = new Vector3(w, 0.1f, h);
    }

    void ResizeWall(EditableObject obj, float len)
    {
        obj.length = len;

        obj.transform.localScale = new Vector3(0.1f, 0.1f, len);
    }
}
