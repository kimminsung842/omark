using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public static WallManager Instance;

    public List<WallSegment> walls = new List<WallSegment>();

    void Awake() {
        Instance = this;
    }

    public void AddWall(Vector3 start, Vector3 end) {
        walls.Add(new WallSegment(start, end));
    }
}