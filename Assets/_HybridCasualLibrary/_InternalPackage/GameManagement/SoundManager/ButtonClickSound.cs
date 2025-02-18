using System;
using System.Collections;
using System.Collections.Generic;
using LatteGames.Template;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float m_Volumn = 1f;
    [SerializeField]
    private SoundID m_SoundEnum = new SoundID(GeneralSFX.UITapButton.ToString(), typeof(GeneralSFX).AssemblyQualifiedName);

    private void Start()
    {
        if (TryGetComponent(out Button button))
            button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        SoundManager.Instance.PlaySFX(m_SoundEnum, m_Volumn);
    }
}