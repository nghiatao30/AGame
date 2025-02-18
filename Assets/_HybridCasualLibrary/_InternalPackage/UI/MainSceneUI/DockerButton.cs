using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using HyrphusQ.Events;

public class DockerButton : MonoBehaviour, IDockerButton
{
    public UnityEvent onClickEvent;
    public UnityEvent onDeselectEvent;

    [SerializeField] protected ButtonType buttonType;
    [SerializeField] protected EventCode eventCode;

    [SerializeField] protected Color selectedColor;
    [SerializeField] protected Image buttonIcon, outlineImg;
    [SerializeField] protected TextMeshProUGUI displayText;
    [SerializeField] protected LayoutElement layoutElement;
    [SerializeField] protected Image insideImage;

    [SerializeField] protected Sprite normalSprite;
    [SerializeField] protected Sprite selectedSprite;

    protected Color firstIconColor;
    protected Color firstOutlineColor;
    protected DockerTab dockerTab;
    protected Button button;

    public ButtonType ButtonType => buttonType;
    public EventCode EventCode => eventCode;
    public DockerTab DockerTab
    {
        get
        {
            if (dockerTab == null)
                dockerTab = Array.Find(FindObjectsOfType<DockerTab>(true), item => item.ButtonType == ButtonType);
            return dockerTab;
        }
    }
    public Button Button
    {
        get
        {
            if (button == null)
                button = GetComponent<Button>();
            return button;
        }
    }

    protected void Awake()
    {
        firstIconColor = buttonIcon.color;
        firstOutlineColor = outlineImg.color;
    }

    public virtual void Select(bool isForceSelect = false)
    {
        displayText.gameObject.SetActive(true);
        displayText.transform.localScale = Vector3.one * 0.5f;
        displayText.color = Color.clear;
        displayText.transform.DOScale(1, 0.25f);
        buttonIcon.sprite = selectedSprite;
        displayText.DOColor(selectedColor, 0.25f);
        outlineImg.DOColor(selectedColor, 0.25f);
        insideImage.transform.DOScale(0.96f, 0.25f);
        buttonIcon.transform.DOScale(1.3f, 0.25f);
        layoutElement.DOMinSize(new Vector2(120f, 0), 0.25f);
        onClickEvent?.Invoke();
    }

    public virtual void Deselect()
    {
        displayText.gameObject.SetActive(false);
        buttonIcon.sprite = normalSprite;
        displayText.DOColor(firstIconColor, 0.25f);
        outlineImg.DOColor(firstOutlineColor, 0.25f);
        layoutElement.DOMinSize(new Vector2(0f, 0), 0.25f);
        buttonIcon.transform.DOScale(1, 0.25f);
        insideImage.transform.DOScale(1, 0.25f);
        onDeselectEvent?.Invoke();
    }
}