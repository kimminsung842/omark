using UnityEngine;

[System.Serializable]
public class WallSegment {
    public Vector3 start;
    public Vector3 end;

    public WallSegment(Vector3 s, Vector3 e) {
        start = s;
        end = e;
    }
}