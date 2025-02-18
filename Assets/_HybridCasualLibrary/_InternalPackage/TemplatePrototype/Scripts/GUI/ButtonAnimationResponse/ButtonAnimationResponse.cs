using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonAnimationResponse : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    protected Button m_Button;

    protected virtual bool isInteractable => m_Button.interactable && m_Button.enabled;

    protected virtual void OnValidate()
    {
        if (m_Button == null)
            m_Button = GetComponent<Button>();
    }

    protected virtual void OnPointerDown_Internal(PointerEventData eventData)
    {

    }

    protected virtual void OnPointerUp_Internal(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        if (!isInteractable)
            return;
        OnPointerDown_Internal(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if (!isInteractable)
            return;
        OnPointerUp_Internal(eventData);
    }
}