using UnityEngine;
using UnityEngine.EventSystems; // Event Trigger 사용을 위해 필요

public class CameraMovementController : MonoBehaviour
{
    // [Inspector 연결]
    [Header("이동 대상 설정")]
    // 카메라를 담고 있는 Pivot 오브젝트의 Transform을 연결합니다. (예: Camera Offset)
    public Transform targetPivot;

    [Header("이동 속도")]
    // 초당 이동할 거리 (m/s). Time.deltaTime과 곱해져 사용됩니다.
    public float moveSpeed = 1f;

    // 내부 상태 변수: 현재 이동 방향 상태를 저장할 변수들
    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private bool isMovingRight = false;
    private bool isMovingLeft = false;

    // ======================================================================
    // 1. 상태 설정 함수 (Event Trigger - Pointer Down/Up 에 연결)
    // ======================================================================

    // 앞으로 이동 상태 설정 (UP 버튼의 Pointer Down/Up에 연결)
    public void SetMoveForward(bool isPressed)
    {
        isMovingForward = isPressed;
    }

    // 뒤로 이동 상태 설정 (DOWN 버튼의 Pointer Down/Up에 연결)
    public void SetMoveBackward(bool isPressed)
    {
        isMovingBackward = isPressed;
    }

    // 오른쪽 이동 상태 설정 (RIGHT 버튼의 Pointer Down/Up에 연결)
    public void SetMoveRight(bool isPressed)
    {
        isMovingRight = isPressed;
    }

    // 왼쪽 이동 상태 설정 (LEFT 버튼의 Pointer Down/Up에 연결)
    public void SetMoveLeft(bool isPressed)
    {
        isMovingLeft = isPressed;
    }

    // ======================================================================
    // 2. 지속 이동 로직 (Update 함수)
    // ======================================================================
    void Update()
    {
        if (targetPivot == null) return;

        // Time.deltaTime을 곱하여 프레임 속도에 관계없이 일정한 속도를 유지합니다.
        float distance = moveSpeed * Time.deltaTime;

        // 로컬 축(Space.Self)을 따라 이동해야 카메라가 바라보는 방향으로 움직입니다.
        if (isMovingForward)
        {
            targetPivot.Translate(Vector3.forward * distance, Space.Self);
        }
        if (isMovingBackward)
        {
            targetPivot.Translate(Vector3.back * distance, Space.Self);
        }
        if (isMovingRight)
        {
            targetPivot.Translate(Vector3.right * distance, Space.Self);
        }
        if (isMovingLeft)
        {
            targetPivot.Translate(Vector3.left * distance, Space.Self);
        }
    }
}