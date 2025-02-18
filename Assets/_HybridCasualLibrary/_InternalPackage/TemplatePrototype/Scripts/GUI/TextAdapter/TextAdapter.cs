using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HyrphusQ.GUI
{
    [Serializable]
    public class TextAdapter
    {
        #region Constructors
        public TextAdapter () { }
        public TextAdapter (TextType textType, Graphic text)
        {
            m_TextType = textType;
            switch (textType)
            {
                case TextType.BuildIn:
                    m_TextBuiltIn = text as Text;
                    break;
                case TextType.TextMeshPro:
                    m_TextMeshPro = text as TMP_Text;
                    break;
                default:
                    break;
            }
            Init();
        }
        #endregion

        public enum TextType
        {
            None,
            BuildIn,
            TextMeshPro
        }

        #region Fields
        [SerializeField]
        private TextType m_TextType = TextType.None;
        [SerializeField]
        private Text m_TextBuiltIn;
        [SerializeField]
        private TMP_Text m_TextMeshPro;

        private string m_BlueprintText;
        private Func<string> m_GetTextMethod;
        private Action<string> m_SetTextMethod;
        #endregion

        #region Properties
        public bool isInitialized
        {
            get => m_GetTextMethod != null && m_SetTextMethod != null;
        }
        public string text
        {
            get => GetText();
            set => SetText(value);
        }
        public string blueprintText
        {
            get
            {
                if (string.IsNullOrEmpty(m_BlueprintText))
                    m_BlueprintText = GetText();
                return m_BlueprintText;
            }
        }
        public GameObject gameObject => GetAdapteeText()?.gameObject;
        #endregion

        #region Methods
        public void Init()
        {
            switch (m_TextType)
            {
                case TextType.BuildIn:
                    m_GetTextMethod = () => m_TextBuiltIn.text;
                    m_SetTextMethod = text => m_TextBuiltIn.text = text;
                    m_BlueprintText = m_TextBuiltIn.text;
                    break;
                case TextType.TextMeshPro:
                    m_GetTextMethod = () => m_TextMeshPro.text;
                    m_SetTextMethod = text => m_TextMeshPro.text = text;
                    m_BlueprintText = m_TextMeshPro.text;
                    break;
                default:
                    break;
            }
        }

        public MaskableGraphic GetAdapteeText()
        {
            switch (m_TextType)
            {
                case TextType.BuildIn:
                    return m_TextBuiltIn;
                case TextType.TextMeshPro:
                    return m_TextMeshPro;
                default:
                    return null;
            }
        }

        public T GetAdapteeText<T>() where T : MaskableGraphic
        {
            if (typeof(T).Equals(typeof(Text)))
                return m_TextBuiltIn as T;
            else if (typeof(T).Equals(typeof(TMP_Text)))
                return m_TextMeshPro as T;
            Debug.LogError($"Type mismatch exception {typeof(T)}");
            return null;
        }

        public string GetText()
        {
            if (!isInitialized)
                Init();
            return m_GetTextMethod?.Invoke() ?? null;
        }

        public void SetText(string text)
        {
            if (!isInitialized)
                Init();
            m_SetTextMethod?.Invoke(text);
        }
        #endregion
    }
}