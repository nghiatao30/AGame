using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineImageSelectionResponse : MonoBehaviour, ISelectionResponse
{
    public enum OutlineSelectionBehaviour
    {
        ChangeColor,
        SetActive
    }
    [SerializeField]
    protected OutlineSelectionBehaviour m_Behaviour = OutlineSelectionBehaviour.ChangeColor;
    [SerializeField, DrawIf("DrawColorPredicate")]
    protected Color m_SelectColor, m_DeselectColor;
    [SerializeField]
    protected Image m_OutlineImage;

    private bool DrawColorPredicate() => m_Behaviour == OutlineSelectionBehaviour.ChangeColor;

    public void Select(bool isForceSelect = false)
    {
        switch (m_Behaviour)
        {
            case OutlineSelectionBehaviour.ChangeColor:
                m_OutlineImage.color = m_SelectColor;
                break;
            case OutlineSelectionBehaviour.SetActive:
                m_OutlineImage.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void Deselect()
    {
        switch (m_Behaviour)
        {
            case OutlineSelectionBehaviour.ChangeColor:
                m_OutlineImage.color = m_DeselectColor;
                break;
            case OutlineSelectionBehaviour.SetActive:
                m_OutlineImage.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}