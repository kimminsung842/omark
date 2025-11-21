using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hiddenPanel;

    [Header("Move and Marker Panel Settings")]
    [SerializeField]
    private GameObject movePanel; // Hierarchy의 move_panel을 연결할 필드

    [SerializeField]
    private GameObject makrerPanel; // Hierarchy의 Makrer 패널을 연결할 필드

    // Menu 버튼에 연결할 함수: 패널을 활성화(보이게) 합니다.
    public void OpenPanel()
    {
        if (hiddenPanel != null)
        {
            hiddenPanel.SetActive(true);
        }
    }

    // move_back 버튼에 연결할 함수: 패널을 비활성화(숨기게) 합니다.
    public void ClosePanel()
    {
        if (hiddenPanel != null)
        {
            hiddenPanel.SetActive(false);
        }
    }

    public void ToggleMovePanel()
    {
        // 연결된 오브젝트가 있는지 다시 한번 확인
        if (movePanel == null || makrerPanel == null)
        {
            Debug.LogError("MovePanel 또는 MakrerPanel이 Inspector에 연결되지 않았습니다.");
            return;
        }

        // 1. movePanel의 현재 활성화 상태를 반전시켜 새로운 상태를 결정합니다.
        //    (예: 닫혀 있으면 true(열기), 열려 있으면 false(닫기))
        bool isMovePanelOpening = !movePanel.activeSelf;

        // 2. movePanel의 상태를 토글합니다.
        movePanel.SetActive(isMovePanelOpening);

        // 3. makrerPanel의 상태를 movePanel의 새로운 상태와 정반대로 설정합니다.
        //    (즉, movePanel이 열리면 MakrerPanel은 닫히고, movePanel이 닫히면 MakrerPanel은 열립니다.)
        makrerPanel.SetActive(!isMovePanelOpening);
    }
}
