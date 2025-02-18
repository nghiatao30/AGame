using UnityEngine;

[CreateAssetMenu(fileName = "VirtualJoyStickSharedData", menuName = "LatteGames/ScriptableObject/VirtualJoystickShareData", order = 0)]
public class VirtualJoyStickSharedData : ScriptableObject {
    public float DragSize = 200;
    public bool Clamp = true;
    public bool MouseDownFollow = false;

    private Source source;
    public Vector3 MouseDown => source?.GetMouseDown()??Vector3.zero;
    public Vector3 CurrentMouse => source?.GetCurrent()??Vector3.zero;
    public Vector3 ClampedCurrentMouse => GetClampedCurrentMouse();

    public bool Dragging => source?.GetDragging()??false;

    public Vector3 NormalizedDrag
    {
        get {
            var dragMag = RawDragVector.magnitude/DragSize;
            if(Clamp)
                dragMag = Mathf.Clamp01(dragMag);
            return RawDragVector.normalized*dragMag;
        }
    }

    public Vector3 RawDragVector
    {
        get
        {
            if(!Dragging)
                return Vector3.zero;
            return CurrentMouse - MouseDown;
        }
    }

    private Vector3 GetClampedCurrentMouse()
    {
        if(!Clamp)
            return CurrentMouse;
        var mouseVector = CurrentMouse - MouseDown;
        var dragMag = RawDragVector.magnitude/DragSize;
        var clampFactor = dragMag/Mathf.Clamp01(dragMag);
        return MouseDown + mouseVector/clampFactor;
    }

    public void SetSource(Source source)
    {
        this.source = source;
    }

    public interface Source{
        Vector3 GetCurrent();
        Vector3 GetMouseDown();
        bool GetDragging();
    }
}