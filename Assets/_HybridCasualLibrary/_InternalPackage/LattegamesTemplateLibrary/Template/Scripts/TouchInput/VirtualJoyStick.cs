using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoyStick : MonoBehaviour, VirtualJoyStickSharedData.Source
{
    [SerializeField] private VirtualJoyStickSharedData data = null;

    private Vector3 current;
    private Vector3 mouseDown;
    private bool dragging;

    public Vector3 GetCurrent()
    {
        return current;
    }

    public bool GetDragging()
    {
        return dragging;
    }

    public Vector3 GetMouseDown()
    {
        return mouseDown;
    }

    private void Awake() {
        data.SetSource(this);
    }

    private void Update() {
        current = Input.mousePosition;
        if(Input.GetMouseButtonDown(0))
        {
            dragging = true;
            mouseDown = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
        if(data.MouseDownFollow)
        {
            if((current - mouseDown).magnitude > data.DragSize)
            {
                var dragVector = current - mouseDown;
                mouseDown = current - dragVector.normalized*data.DragSize;
            }
        }
    }
}