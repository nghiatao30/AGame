using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressBarVFX : MonoBehaviour
{
    [SerializeField] Material progressBarMaterial;
    Image imageInstance;
    Material materialInstance;

    private void OnEnable()
    {
        Setup();
    }

    void Setup()
    {
        Slider slider = GetComponent<Slider>();
        Image filler = slider.fillRect.GetComponent<Image>();
        RectTransform parent = filler.transform.parent.GetComponent<RectTransform>();
        if (imageInstance == null) imageInstance = Instantiate(filler, filler.transform).GetComponent<Image>();
        if (materialInstance == null) materialInstance = Instantiate(progressBarMaterial);
        materialInstance.SetFloat("_Width", parent.sizeDelta.x);
        imageInstance.material = materialInstance;
        imageInstance.sprite = null;
        SetAndStretchToParentSize(imageInstance.rectTransform, filler.rectTransform);
        filler.gameObject.GetOrAddComponent<Mask>();
    }

    void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
    {
        _mRect.anchoredPosition = _parent.anchoredPosition;
        _mRect.anchorMin = new Vector2(0, 0);
        _mRect.anchorMax = new Vector2(1, 1);
        _mRect.pivot = new Vector2(0.5f, 0.5f);
    }
}
