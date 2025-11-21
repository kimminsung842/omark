using UnityEngine;
using UnityEngine.EventSystems;

// IDragHandler 인터페이스를 사용하여 드래그 입력을 받습니다.
public class CameraRotationController : MonoBehaviour, IDragHandler
{
    // [Inspector 연결]
    [Header("회전 대상 설정")]
    // 카메라를 담고 있는 Pivot 오브젝트의 Transform을 연결합니다. (예: Camera Offset)
    public Transform targetPivot;

    [Header("회전 설정")]
    public float rotationSpeed = 0.05f; // 드래그 민감도/속도

    [Header("Pitch 제한 설정")]
    public float pitchMin = -60f; // 최대 위로 꺾는 각도 (Inspector에서 설정)
    public float pitchMax = 60f;  // 최대 아래로 꺾는 각도 (Inspector에서 설정)

    // 내부 상태 변수: 현재 X축(Pitch) 회전 각도를 누적 저장
    private float currentPitch = 0f;

    void Start()
    {
        if (targetPivot != null)
        {
            // 시작 시 현재 회전 각도를 초기화하고 -180 ~ 180 범위로 변환하여 clamping에 용이하게 합니다.
            currentPitch = targetPivot.localEulerAngles.x;
            if (currentPitch > 180f)
            {
                currentPitch -= 360f;
            }
        }
    }


    // 드래그가 발생할 때마다 호출됨
    public void OnDrag(PointerEventData eventData)
    {
        if (targetPivot == null)
        {
            Debug.LogError("회전 대상 Pivot이 연결되지 않았습니다.");
            return;
        }

        // 1. Y축 회전 (좌우 드래그)
        float rotationY = eventData.delta.x * rotationSpeed * -1; // -1을 곱하여 드래그 방향과 일치시킵니다.
        targetPivot.Rotate(Vector3.up, rotationY, Space.World); // World Space 기준으로 회전

        // 2. X축 회전량 계산 (상하 드래그)
        float rotationX = eventData.delta.y * rotationSpeed;

        // 3. **핵심 수정: Pitch 각도 제한 및 적용**

        // 현재 Pitch 각도 업데이트 (누적)
        currentPitch += rotationX;

        // 각도를 제한합니다. (카메라가 뒤집히는 것을 방지)
        currentPitch = Mathf.Clamp(currentPitch, pitchMin, pitchMax);

        // 새로운 회전 값 생성: X축은 제한된 Pitch, Y축은 기존 Yaw
        // targetPivot의 현재 Y축 회전 값(localEulerAngles.y)을 가져와서 사용합니다.
        Quaternion targetRotation = Quaternion.Euler(currentPitch, targetPivot.localEulerAngles.y, 0);

        // 회전 적용: 현재 피벗의 Rotation을 새로운 Quaternion 값으로 설정
        targetPivot.localRotation = targetRotation;

        // Debug.Log($"Camera Dragged: Y={rotationY}, Pitch={currentPitch}");
    }
}