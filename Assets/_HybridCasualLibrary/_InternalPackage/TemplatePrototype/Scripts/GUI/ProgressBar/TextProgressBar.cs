using System;
using System.Collections;
using UnityEngine;
using HyrphusQ.Events;

namespace HyrphusQ.GUI
{
    [AddComponentMenu("HyrphusQ/GUI/ProgressBar/TextProgressBar")]
    public class TextProgressBar : ProgressBar
    {
        [Flags]
        public enum TextFormat
        {
            None = 0,
            Prefix = 1 << 0,
            Value = 1 << 1,
            MaxValue = 1 << 2,
            Postfix = 1 << 3
        }

        [SerializeField]
        private TextAdapter m_TextAdapter;
        [SerializeField]
        private TextFormat m_Format = TextFormat.Value;
        [SerializeField, DrawIf("DrawFieldPrefix")]
        private string m_Prefix = string.Empty;
        [SerializeField, DrawIf("DrawFieldPostfix")]
        private string m_Postfix = string.Empty;
        private string m_TextValue = string.Empty;

        protected override void Awake()
        {
            m_TextAdapter.Init();
            base.Awake();
        }

        protected override void OnValueChanged(ValueDataChanged<int> data)
        {
            var duration = m_Config.animationDuration;
            if (m_Config.modifiedAnimationDuration)
            {
                float inverseLerpValue = m_Config.inverseAnimationDuration ? (1f - m_MinMaxIntProgress.inverseLerpValue) : m_MinMaxIntProgress.inverseLerpValue;
                duration *= inverseLerpValue;
            }
            var oldValue = m_Config.inverseValue ? (m_MinMaxIntProgress.maxValue - data.oldValue) : data.oldValue;
            var newValue = m_Config.inverseValue ? (m_MinMaxIntProgress.maxValue - data.newValue) : data.newValue;
            SetValue(oldValue, newValue, duration);
        }
        protected override void OnValueChanged(ValueDataChanged<float> data)
        {
            var duration = m_Config.animationDuration;
            if (m_Config.modifiedAnimationDuration)
            {
                float inverseLerpValue = m_Config.inverseAnimationDuration ? (1f - m_MinMaxFloatProgress.inverseLerpValue) : m_MinMaxFloatProgress.inverseLerpValue;
                duration *= inverseLerpValue;
            }
            var oldValue = m_Config.inverseValue ? (m_MinMaxFloatProgress.maxValue - data.oldValue) : data.oldValue;
            var newValue = m_Config.inverseValue ? (m_MinMaxFloatProgress.maxValue - data.newValue) : data.newValue;
            SetValue(oldValue, newValue, duration);
        }
        private IEnumerator TextCounterCR(int oldValue, int newValue, float animationDuration)
        {
            yield return LerpFactor(animationDuration, t => {
                var value = (int)Mathf.Lerp(oldValue, newValue, Mathf.Clamp01(t));
                SetValueImmediately(value);
            });
        }
        private IEnumerator TextCounterCR(float oldValue, float newValue, float animationDuration)
        {
            yield return LerpFactor(animationDuration, t => {
                var value = Mathf.Lerp(oldValue, newValue, Mathf.Clamp01(t));
                SetValueImmediately(value);
            });
        }
        private bool DrawFieldPrefix()
        {
            return (m_Format & TextFormat.Prefix) == TextFormat.Prefix;
        }
        private bool DrawFieldPostfix()
        {
            return (m_Format & TextFormat.Postfix) == TextFormat.Postfix;
        }

        public override void SetValue(int oldValue, int value, float animationDuration)
        {
            if(animationDuration <= 0f)
            {
                SetValueImmediately(value);
                return;
            }
            if (m_LerpCoroutine != null)
            {
                StopCoroutine(m_LerpCoroutine);    
            }
            m_LerpCoroutine = StartCoroutine(TextCounterCR(oldValue, value, animationDuration));
        }
        public override void SetValue(float oldValue, float value, float animationDuration)
        {
            if (animationDuration <= 0f)
            {
                SetValueImmediately(value);
                return;
            }
            if (m_LerpCoroutine != null)
            {
                StopCoroutine(m_LerpCoroutine);
            }
            m_LerpCoroutine = StartCoroutine(TextCounterCR(oldValue, value, animationDuration));
        }
        public override void SetValueImmediately(int value)
        {
            m_TextValue = string.Empty;
            if ((m_Format & TextFormat.Prefix) == TextFormat.Prefix)
                m_TextValue += $"{m_Prefix}";
            if ((m_Format & TextFormat.Value) == TextFormat.Value)
                m_TextValue += $"{value}";
            if ((m_Format & TextFormat.MaxValue) == TextFormat.MaxValue)
                m_TextValue += $"/{m_MinMaxIntProgress.maxValue}";
            if ((m_Format & TextFormat.Postfix) == TextFormat.Postfix)
                m_TextValue += $"{m_Postfix}";

            m_TextAdapter.SetText(m_TextValue);
        }
        public override void SetValueImmediately(float value)
        {
            m_TextValue = string.Empty;
            if ((m_Format & TextFormat.Prefix) == TextFormat.Prefix)
                m_TextValue += $"{m_Prefix}";
            if ((m_Format & TextFormat.Value) == TextFormat.Value)
                m_TextValue += $"{value.ToString("0.00")}";
            if ((m_Format & TextFormat.MaxValue) == TextFormat.MaxValue)
                m_TextValue += $"/{m_MinMaxFloatProgress.maxValue.ToString("0.00")}";
            if ((m_Format & TextFormat.Postfix) == TextFormat.Postfix)
                m_TextValue += $"{m_Postfix}";

            m_TextAdapter.SetText(m_TextValue);
        }
    }
}