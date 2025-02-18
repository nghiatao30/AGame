using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerInfoUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField]
    protected TextMeshProUGUI m_PlayerName;
    [SerializeField]
    protected Image m_AvatarImage;
    [SerializeField]
    protected Image m_NationalFlagImage;
    [Header("Data")]
    [SerializeField]
    protected PlayerInfoVariable m_InfoVariableOfMine;

    public virtual void ShowInfo()
    {
        m_PlayerName.text = m_InfoVariableOfMine.value.personalInfo.name;
        m_AvatarImage.sprite = m_InfoVariableOfMine.value.personalInfo.avatar;
        m_NationalFlagImage.sprite = m_InfoVariableOfMine.value.personalInfo.nationalFlag;
    }
}