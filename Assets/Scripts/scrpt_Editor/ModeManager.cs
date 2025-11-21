using UnityEngine;

public enum EditMode {
    Wall,
    Room,
    Move
}

public class ModeManager : MonoBehaviour 
{
    public static ModeManager Instance;

    public EditMode CurrentMode = EditMode.Move;

    private void Awake() 
    {
        Instance = this;
    }

    public void SetMode(EditMode mode) 
    {
        CurrentMode = mode;
    }

    public void OnWallModeClicked() {ModeManager.Instance.SetMode(EditMode.Wall); }
    public void OnRoomModeClicked() {ModeManager.Instance.SetMode(EditMode.Room); }
    public void OnMoveModeClicked() {ModeManager.Instance.SetMode(EditMode.Move); }
}

