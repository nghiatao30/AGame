using HyrphusQ.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.GUI
{
    [AddComponentMenu("HyrphusQ/GUI/ProgressBar/CompoundProgressBar")]
    public class CompoundProgressBar : ProgressBar
    {
        [SerializeField]
        private List<ProgressBar> m_ProgressBars;

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (m_ProgressBars.Count <= 0)
                return;
            OnValidateCompoundProgressBar(this, m_ProgressBars);   
        }
#endif

        protected override void OnValueChanged(ValueDataChanged<int> data)
        {
            // Do Nothing
        }
        protected override void OnValueChanged(ValueDataChanged<float> data)
        {
            // Do Nothing
        }

        public override void Init(RangeProgress<int> minMaxIntProgress)
        {
            foreach (var progressBar in m_ProgressBars)
                progressBar.Init(minMaxIntProgress);
            this.m_MinMaxIntProgress = minMaxIntProgress;
            SetValueImmediately(minMaxIntProgress.value);
        }
        public override void Init(RangeProgress<float> minMaxFloatProgress)
        {
            foreach (var progressBar in m_ProgressBars)
                progressBar.Init(minMaxFloatProgress);
            this.m_MinMaxFloatProgress = minMaxFloatProgress;
            SetValueImmediately(minMaxFloatProgress.value);
        }
        public override void SetValue(int oldValue, int value, float animationDuration)
        {
            if (animationDuration <= 0f)
            {
                SetValueImmediately(value);
                return;
            }

            foreach (var progressBar in m_ProgressBars)
                progressBar.SetValue(oldValue, value, animationDuration);
        }
        public override void SetValue(float oldValue, float value, float animationDuration)
        {
            if (animationDuration <= 0f)
            {
                SetValueImmediately(value);
                return;
            }

            foreach (var progressBar in m_ProgressBars)
                progressBar.SetValue(oldValue, value, animationDuration);
        }
        public override void SetValueImmediately(int value)
        {
            foreach (var progressBar in m_ProgressBars)
                progressBar.SetValueImmediately(value);
        }
        public override void SetValueImmediately(float value)
        {
            foreach (var progressBar in m_ProgressBars)
                progressBar.SetValueImmediately(value);
        }
    }
}