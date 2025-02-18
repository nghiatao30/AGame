using UnityEngine;
using UnityEngine.UI;
using HyrphusQ.Events;

namespace HyrphusQ.GUI
{
    [AddComponentMenu("HyrphusQ/GUI/ProgressBar/SlicedFilledProgressBar")]
    public class SlicedFilledProgressBar : ProgressBar
    {
        [SerializeField]
        private ParticleSystem.MinMaxGradient m_Color;
        [SerializeField]
        private SlicedFilledImage m_ProgressImage;

        protected override void OnValueChanged(ValueDataChanged<int> data)
        {
            var duration = m_Config.animationDuration;
            if (m_Config.modifiedAnimationDuration)
            {
                float inverseLerpValue = m_Config.inverseAnimationDuration ? (1f - m_MinMaxIntProgress.inverseLerpValue) : m_MinMaxIntProgress.inverseLerpValue;
                duration *= inverseLerpValue;
            }
            var oldValue = m_Config.inverseValue ? (m_MinMaxIntProgress.maxValue - m_MinMaxIntProgress.CalcInterpolatedValue(m_ProgressImage.fillAmount)) : m_MinMaxIntProgress.CalcInterpolatedValue(m_ProgressImage.fillAmount);
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
            var oldValue = m_Config.inverseValue ? (m_MinMaxFloatProgress.maxValue - m_MinMaxFloatProgress.CalcInterpolatedValue(m_ProgressImage.fillAmount)) : m_MinMaxFloatProgress.CalcInterpolatedValue(m_ProgressImage.fillAmount);
            var newValue = m_Config.inverseValue ? (m_MinMaxFloatProgress.maxValue - data.newValue) : data.newValue;
            SetValue(oldValue, newValue, duration);
        }

        public override void SetValue(int oldValue, int value, float animationDuration)
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
            m_LerpCoroutine = StartCoroutine(LerpFactor(animationDuration, t =>
            {
                SetValueImmediately((int)Mathf.Lerp(oldValue, value, t));
            }));
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
            m_LerpCoroutine = StartCoroutine(LerpFactor(animationDuration, t =>
            {
                SetValueImmediately(Mathf.Lerp(oldValue, value, t));
            }));
        }
        public override void SetValueImmediately(int value)
        {
            m_ProgressImage.fillAmount = m_MinMaxIntProgress.CalcInverseLerpValue(value);
            m_ProgressImage.color = m_Color.Evaluate(m_ProgressImage.fillAmount);
        }
        public override void SetValueImmediately(float value)
        {
            m_ProgressImage.fillAmount = m_MinMaxFloatProgress.CalcInverseLerpValue(value);
            m_ProgressImage.color = m_Color.Evaluate(m_ProgressImage.fillAmount);
        }
    }
}