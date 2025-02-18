using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticResponse : MonoBehaviour
{
    public enum HapticUpdateMode
    {
        Continuous,
        Discrete,
        Transient,
    }

    [SerializeField]
    private bool m_IsContinuousHaptic = true;
    [SerializeField, DrawIf("DrawUpdateIntensityAndSharpnessProperty")]
    private bool m_IsUpdateIntensityAndSharpness;
    [SerializeField, DrawIf("DrawIntensityAndSharpnessCurveProperty")]
    private AnimationCurve m_IntensityCurve, m_SharpnessCurve;
    [SerializeField, DrawIf("DrawFrequencyProperty")]
    private float m_HapticFrequency = 0.25f;
    [SerializeField, DrawIf("DrawIntensityAndSharpnessProperty")]
    private float m_HapticIntensity = 0.5f;
    [SerializeField, DrawIf("DrawIntensityAndSharpnessProperty")]
    private float m_HapticSharpness = 0.5f;
    [SerializeField]
    private HapticTypes m_HapticTypes = HapticTypes.MediumImpact;
    [SerializeField]
    private HapticUpdateMode m_HapticUpdateMode = HapticUpdateMode.Continuous;
    [SerializeField]
    private MonoBehaviour m_HapticNotifierMono;
    private IHapticNotifier hapticNotifier => m_HapticNotifierMono as IHapticNotifier;

    private float m_HapticDiscreteTimestamp = float.MinValue;
    private Coroutine m_HapticContinuousCoroutine;
    private HapticManager m_HapticService;

    private void OnEnable()
    {
        if (hapticNotifier == null)
            return;
        hapticNotifier.onBeginHaptic += OnBeginHaptic;
        hapticNotifier.onStopHaptic += OnStopHaptic;
    }
    private void Start()
    {
        m_HapticService = HapticManager.Instance;
    }
    private void OnDisable()
    {
        if (hapticNotifier == null)
            return;
        hapticNotifier.onBeginHaptic -= OnBeginHaptic;
        hapticNotifier.onStopHaptic -= OnStopHaptic;
    }

    private void StopCoroutineIfNotNull(ref Coroutine coroutine)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = null;
    }
    private IEnumerator PlayContinuousHaptic_CR()
    {
        var timeStamp = float.MinValue;
        var startTimeStamp = Time.time;
        while (true)
        {
            var currentTime = Time.time;
            if (currentTime - timeStamp > m_HapticFrequency)
            {
                timeStamp = currentTime;
                switch (m_HapticUpdateMode)
                {
                    case HapticUpdateMode.Continuous:
                        m_HapticService.PlayContinuousHaptic(m_HapticIntensity, m_HapticSharpness, m_HapticFrequency, m_HapticTypes);
                        break;
                    case HapticUpdateMode.Discrete:
                        m_HapticService.PlayFlashHaptic(m_HapticTypes);
                        break;
                    case HapticUpdateMode.Transient:
                        m_HapticService.PlayTransientHaptic(m_HapticIntensity, m_HapticSharpness);
                        break;
                    default:
                        break;
                }
            }
            if (m_IsUpdateIntensityAndSharpness && m_HapticUpdateMode != HapticUpdateMode.Discrete)
            {
                var timeFromStart = currentTime - startTimeStamp;
                m_HapticService.UpdateContinuousHaptic(m_IntensityCurve.Evaluate(timeFromStart), m_SharpnessCurve.Evaluate(timeFromStart));
            }
            yield return null;
        }
    }
    private void PlayDiscreteHapticWithFrequency()
    {
        var currentTimestamp = Time.time;
        if(currentTimestamp - m_HapticDiscreteTimestamp >= m_HapticFrequency)
        {
            m_HapticDiscreteTimestamp = currentTimestamp;
            switch (m_HapticUpdateMode)
            {
                case HapticUpdateMode.Continuous:
                    m_HapticService.PlayContinuousHaptic(m_HapticIntensity, m_HapticSharpness, m_HapticFrequency, m_HapticTypes);
                    break;
                case HapticUpdateMode.Discrete:
                    m_HapticService.PlayFlashHaptic(m_HapticTypes);
                    break;
                case HapticUpdateMode.Transient:
                    m_HapticService.PlayTransientHaptic(m_HapticIntensity, m_HapticSharpness);
                    break;
                default:
                    break;
            }
        }
    }
    private void OnBeginHaptic()
    {
        if (m_IsContinuousHaptic)
        {
            OnStopHaptic();
            m_HapticContinuousCoroutine = StartCoroutine(PlayContinuousHaptic_CR());
        }
        else
        {
            PlayDiscreteHapticWithFrequency();
        }
    }
    private void OnStopHaptic()
    {
        if (m_IsContinuousHaptic)
        {
            if (m_HapticContinuousCoroutine == null)
                return;
            StopCoroutineIfNotNull(ref m_HapticContinuousCoroutine);
            m_HapticService.StopContinuousHaptic();
        }
    }

#if UNITY_EDITOR
    private bool DrawFrequencyProperty()
    {
        return m_IsContinuousHaptic;
    }
    private bool DrawIntensityAndSharpnessProperty()
    {
        return m_HapticUpdateMode != HapticUpdateMode.Discrete;
    }
    private bool DrawUpdateIntensityAndSharpnessProperty()
    {
        return m_IsContinuousHaptic && m_HapticUpdateMode != HapticUpdateMode.Discrete;
    }
    private bool DrawIntensityAndSharpnessCurveProperty()
    {
        return m_IsUpdateIntensityAndSharpness;
    }
#endif
}
