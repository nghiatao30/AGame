using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualJoyStickVisual : MonoBehaviour
{
    [SerializeField] private VirtualJoyStickSharedData data = null;
    [SerializeField] private Image mouseDownImage = null;
    [SerializeField] private Image mouseCurrImage = null;

    private Canvas parentCanvas;
    private RectTransform mouseDownRectTransform;
    private RectTransform mouseCurrRectTransform;

    private void Awake() {
        parentCanvas = GetComponentInParent<Canvas>();
        mouseDownRectTransform = mouseDownImage.transform as RectTransform;
        mouseCurrRectTransform = mouseCurrImage.transform as RectTransform;

        (mouseCurrImage.transform as RectTransform).anchorMax = Vector2.zero;
        (mouseCurrImage.transform as RectTransform).anchorMin = Vector2.zero;
        (mouseDownImage.transform as RectTransform).anchorMax = Vector2.zero;
        (mouseDownImage.transform as RectTransform).anchorMin = Vector2.zero;
    }
    
    private void Update() {
        mouseDownImage.enabled = data.Dragging;
        mouseCurrImage.enabled = data.Dragging;

        if(!data.Dragging)
            return;
        mouseDownRectTransform.anchoredPosition = ConvertScreenToCanvasSpace(data.MouseDown);
        mouseCurrRectTransform.anchoredPosition = ConvertScreenToCanvasSpace(data.ClampedCurrentMouse);
    }

    private Vector3 ConvertScreenToCanvasSpace(Vector3 screenPos)
    {
        return screenPos/parentCanvas.scaleFactor;
    }
}
