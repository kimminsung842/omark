using UnityEngine;

public enum ObjectType {
    Wall,
    Room
}

public class EditableObject : MonoBehaviour
{
    public ObjectType type;

    // 방일 때 사용
    public float width;   // X 방향
    public float height;  // Z 방향

    // 벽일 때 사용
    public float length;  // 벽 길이
}
