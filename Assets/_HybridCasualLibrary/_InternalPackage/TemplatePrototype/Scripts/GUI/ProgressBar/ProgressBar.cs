using HyrphusQ.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ProgressBar : MonoBehaviour
    {
        public enum RangeValueType
        {
            None,
            Int,
            Float,
        }
        [Serializable]
        public struct ProgressBarConfig
        {
            public bool showByDefault;
            public bool modifiedAnimationDuration;
            public bool inverseAnimationDuration;
            public bool inverseValue;
            public float animationDuration;

            public ProgressBarConfig(bool showByDefault = true, bool modifiedAnimationDuration = true, bool inverseAnimationDuration = false, bool inverseValue = false, float animationDuration = 0f)
            {
                this.showByDefault = showByDefault;
                this.modifiedAnimationDuration = modifiedAnimationDuration;
                this.inverseAnimationDuration = inverseAnimationDuration;
                this.inverseValue = inverseValue;
                this.animationDuration = animationDuration;
            }
        }

        [SerializeField]
        protected ProgressBarConfig m_Config;
        [SerializeField]
        protected RangeValueType m_RangeValueType;
        [SerializeField, DrawIf("DrawRangeIntProgressSO")]
        protected RangeIntProgressSO m_RangeIntProgressSO;
        [SerializeField, DrawIf("DrawRangeFloatProgressSO")]
        protected RangeFloatProgressSO m_RangeFloatProgressSO;

        protected RangeProgress<int> m_MinMaxIntProgress;
        protected RangeProgress<float> m_MinMaxFloatProgress;
        protected RectTransform m_RectTransform;
        protected Coroutine m_LerpCoroutine;

        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                    m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }
        public ProgressBarConfig config
        {
            get => m_Config;
            set => m_Config = value;
        }

        protected virtual void Awake()
        {
            if (m_RangeValueType == RangeValueType.Int && m_RangeIntProgressSO != null)
                Init(m_RangeIntProgressSO.rangeProgress);
            else if (m_RangeValueType == RangeValueType.Float && m_RangeFloatProgressSO != null)
                Init(m_RangeFloatProgressSO.rangeProgress);
            gameObject.SetActive(m_Config.showByDefault);
        }
        protected virtual void OnDestroy()
        {
            if (m_MinMaxIntProgress != null)
                m_MinMaxIntProgress.onValueChanged -= OnValueChanged;
            if (m_MinMaxFloatProgress != null)
                m_MinMaxFloatProgress.onValueChanged -= OnValueChanged;
        }

#if UNITY_EDITOR
        protected bool DrawRangeIntProgressSO()
        {
            return m_RangeValueType == RangeValueType.Int;
        }
        protected bool DrawRangeFloatProgressSO()
        {
            return m_RangeValueType == RangeValueType.Float;
        }
        protected void OnValidateCompoundProgressBar(ProgressBar compoundProgressBar, List<ProgressBar> childProgressBars)
        {
            foreach (var progressBar in childProgressBars)
            {
                progressBar.m_Config = compoundProgressBar.m_Config;
                progressBar.m_RangeValueType = compoundProgressBar.m_RangeValueType;
                progressBar.m_RangeIntProgressSO = compoundProgressBar.m_RangeIntProgressSO;
                progressBar.m_RangeFloatProgressSO = compoundProgressBar.m_RangeFloatProgressSO;
            }
        }
#endif
        protected IEnumerator LerpFactor(float duration, Action<float> callback)
        {
            float t = 0.0f;
            callback(t / duration);
            while (t < duration)
            {
                yield return null;
                t += Time.deltaTime;
                callback(t / duration);
            }
            callback(1f);
        }
        protected abstract void OnValueChanged(ValueDataChanged<int> data);
        protected abstract void OnValueChanged(ValueDataChanged<float> data);

        public virtual void Init(RangeProgress<int> minMaxIntProgress)
        {
            this.m_MinMaxIntProgress = minMaxIntProgress;
            this.m_MinMaxIntProgress.onValueChanged += OnValueChanged;
            SetValueImmediately(m_Config.inverseValue ? minMaxIntProgress.maxValue - minMaxIntProgress.value : minMaxIntProgress.value);
        }
        public virtual void Init(RangeProgress<float> minMaxFloatProgress)
        {
            this.m_MinMaxFloatProgress = minMaxFloatProgress;
            this.m_MinMaxFloatProgress.onValueChanged += OnValueChanged;
            SetValueImmediately(m_Config.inverseValue ? minMaxFloatProgress.maxValue - minMaxFloatProgress.value : minMaxFloatProgress.value);
        }
        public abstract void SetValueImmediately(int value);
        public abstract void SetValueImmediately(float value);
        public abstract void SetValue(int oldValue, int value, float animationDuration);
        public abstract void SetValue(float oldValue, float value, float animationDuration);
    }
}