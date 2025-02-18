using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;
using HyrphusQ.Events;
#if MOREMOUNTAINS_NICEVIBRATIONS
using MoreMountains.NiceVibrations;
#elif MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
using Lofelt.NiceVibrations;
#endif

public enum HapticTypes
{
    Selection,
    Success,
    Warning,
    Failure,
    LightImpact,
    MediumImpact,
    HeavyImpact,
    RigidImpact,
    SoftImpact,
    None
}
public class HapticManager : Singleton<HapticManager>
{
    #region Haptic Service Implementation
    public interface HapticServiceImpl
    {
        bool IsHapticsSupported();
        void PlayTransientHaptic(float intensity = 1f, float sharpness = 1f, MonoBehaviour mono = null);
        void PlayFlashHaptic(HapticTypes hapticType = HapticTypes.MediumImpact);
        void PlayContinuousHaptic(float intensity = 1f, float sharpness = 1f, float duration = 1 / 60f, HapticTypes hapticType = HapticTypes.MediumImpact, MonoBehaviour mono = null);
        void UpdateContinuousHaptic(float intensity = 1f, float sharpness = 1f);
        void StopContinuousHaptic();
        void StopAllHaptics();
        void SetHapticsActive(bool active);
        void Initialize();
    }
#if MOREMOUNTAINS_NICEVIBRATIONS
    public class NiceVibrationsVers3Impl : HapticServiceImpl
    {
        private MoreMountains.NiceVibrations.HapticTypes MapToNiceVibrationHapticTypes(HapticTypes hapticTypes)
        {
            switch (hapticTypes)
            {
                case HapticTypes.Selection:
                    return MoreMountains.NiceVibrations.HapticTypes.Selection;
                case HapticTypes.Success:
                    return MoreMountains.NiceVibrations.HapticTypes.Success;
                case HapticTypes.Warning:
                    return MoreMountains.NiceVibrations.HapticTypes.Warning;
                case HapticTypes.Failure:
                    return MoreMountains.NiceVibrations.HapticTypes.Failure;
                case HapticTypes.LightImpact:
                    return MoreMountains.NiceVibrations.HapticTypes.LightImpact;
                case HapticTypes.MediumImpact:
                    return MoreMountains.NiceVibrations.HapticTypes.MediumImpact;
                case HapticTypes.HeavyImpact:
                    return MoreMountains.NiceVibrations.HapticTypes.HeavyImpact;
                case HapticTypes.RigidImpact:
                    return MoreMountains.NiceVibrations.HapticTypes.RigidImpact;
                case HapticTypes.SoftImpact:
                    return MoreMountains.NiceVibrations.HapticTypes.SoftImpact;
                case HapticTypes.None:
                    return MoreMountains.NiceVibrations.HapticTypes.None;
                default:
                    return MoreMountains.NiceVibrations.HapticTypes.None;
            }
        }
        public bool IsHapticsSupported()
        {
            return MMVibrationManager.HapticsSupported();
        }
        public void PlayTransientHaptic(float intensity = 1f, float sharpness = 1f, MonoBehaviour mono = null)
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.TransientHaptic(intensity, sharpness, false, mono);
        }
        public void PlayFlashHaptic(HapticTypes hapticType = HapticTypes.MediumImpact)
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.Haptic(MapToNiceVibrationHapticTypes(hapticType));
        }
        public void PlayContinuousHaptic(float intensity = 1f, float sharpness = 1f, float duration = 1 / 60f, HapticTypes hapticType = HapticTypes.MediumImpact, MonoBehaviour mono = null)
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.ContinuousHaptic(true, intensity, sharpness, MapToNiceVibrationHapticTypes(hapticType), true, intensity, sharpness, true, false, intensity, sharpness, -1, duration, mono, false, true);
        }
        public void UpdateContinuousHaptic(float intensity = 1f, float sharpness = 1f)
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.UpdateContinuousHaptic(intensity, sharpness);
        }
        public void StopContinuousHaptic()
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.StopContinuousHaptic(true);
        }
        public void StopAllHaptics()
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.StopAllHaptics(true);
        }
        public void SetHapticsActive(bool active)
        {
            if (!IsHapticsSupported())
                return;
            MMVibrationManager.SetHapticsActive(active);
        }
        public void Initialize()
        {

        }
    }
#elif MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
    public class NiceVibrationsVers4Impl : HapticServiceImpl
    {
        private HapticPatterns.PresetType MapToNiceVibrationPresetType(HapticTypes hapticTypes)
        {
            switch (hapticTypes)
            {
                case HapticTypes.Selection:
                    return HapticPatterns.PresetType.Selection;
                case HapticTypes.Success:
                    return HapticPatterns.PresetType.Success;
                case HapticTypes.Warning:
                    return HapticPatterns.PresetType.Warning;
                case HapticTypes.Failure:
                    return HapticPatterns.PresetType.Failure;
                case HapticTypes.LightImpact:
                    return HapticPatterns.PresetType.LightImpact;
                case HapticTypes.MediumImpact:
                    return HapticPatterns.PresetType.MediumImpact;
                case HapticTypes.HeavyImpact:
                    return HapticPatterns.PresetType.HeavyImpact;
                case HapticTypes.RigidImpact:
                    return HapticPatterns.PresetType.RigidImpact;
                case HapticTypes.SoftImpact:
                    return HapticPatterns.PresetType.SoftImpact;
                case HapticTypes.None:
                    return HapticPatterns.PresetType.None;
                default:
                    return HapticPatterns.PresetType.None;
            }
        }
        public bool IsHapticsSupported()
        {
            return DeviceCapabilities.isVersionSupported;
        }
        public void PlayTransientHaptic(float intensity = 1f, float sharpness = 1f, MonoBehaviour mono = null)
        {
            if (!IsHapticsSupported())
                return;
            HapticPatterns.PlayEmphasis(intensity, sharpness);
        }
        public void PlayFlashHaptic(HapticTypes hapticType = HapticTypes.MediumImpact)
        {
            if (!IsHapticsSupported())
                return;
            HapticPatterns.PlayPreset(MapToNiceVibrationPresetType(hapticType));
        }
        public void PlayContinuousHaptic(float intensity = 1f, float sharpness = 1f, float duration = 1 / 60f, HapticTypes hapticType = HapticTypes.MediumImpact, MonoBehaviour mono = null)
        {
            if (!IsHapticsSupported())
                return;
            HapticPatterns.PlayConstant(intensity, sharpness, duration);
        }
        public void UpdateContinuousHaptic(float intensity = 1f, float sharpness = 1f)
        {
            if (!IsHapticsSupported())
                return;
            HapticController.clipLevel = intensity;
            HapticController.clipFrequencyShift = sharpness;
        }
        public void StopContinuousHaptic()
        {
            if (!IsHapticsSupported())
                return;
            HapticController.Stop();
        }
        public void StopAllHaptics()
        {
            if (!IsHapticsSupported())
                return;
            HapticController.Stop();
        }
        public void SetHapticsActive(bool active)
        {
            if (!IsHapticsSupported())
                return;
            HapticController.hapticsEnabled = active;
        }
        public void Initialize()
        {
            HapticController.Init();
        }
    }
#endif
    #endregion
    [SerializeField]
    private PPrefBoolVariable m_IsEnableHaptic;

    private bool m_IsInitialzed;
    private HapticServiceImpl m_HapticServiceImpl;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        m_IsEnableHaptic.onValueChanged += OnValueChanged;
    }

    private void OnDestroy()
    {
        m_IsEnableHaptic.onValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(ValueDataChanged<bool> eventData)
    {
        SetActiveService(m_IsEnableHaptic);
    }

    public void Initialize()
    {
        if (m_IsInitialzed)
            return;
#if MOREMOUNTAINS_NICEVIBRATIONS
        m_HapticServiceImpl = new NiceVibrationsVers3Impl();
#elif MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
        m_HapticServiceImpl = new NiceVibrationsVers4Impl();
#endif
        m_HapticServiceImpl.Initialize();
        m_IsInitialzed = true;
        SetActiveService(m_IsEnableHaptic);
    }
    public bool IsHapticsSupported()
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        return m_HapticServiceImpl.IsHapticsSupported();
    }
    public void PlayTransientHaptic(float intensity = 1f, float sharpness = 1f)
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.PlayTransientHaptic(intensity, sharpness, this);

        // #if UNITY_EDITOR
        //         Debug.Log($"Play transient haptic: {intensity}-{sharpness}-{DateTime.Now.TimeOfDay}");
        // #endif
    }
    public void PlayFlashHaptic(HapticTypes hapticType = HapticTypes.MediumImpact)
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.PlayFlashHaptic(hapticType);

        // #if UNITY_EDITOR
        //         Debug.Log($"Play flash haptic: {hapticType}-{DateTime.Now.TimeOfDay}");
        // #endif
    }
    public void PlayContinuousHaptic(float intensity = 1f, float sharpness = 1f, float duration = 1 / 60f, HapticTypes hapticType = HapticTypes.MediumImpact)
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.PlayContinuousHaptic(intensity, sharpness, duration, hapticType, this);

        // #if UNITY_EDITOR
        //         Debug.Log($"Play continous haptic: {intensity}-{sharpness}-{duration}-{DateTime.Now.TimeOfDay}");
        // #endif
    }
    public void UpdateContinuousHaptic(float intensity = 1f, float sharpness = 1f)
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.UpdateContinuousHaptic(intensity, sharpness);

        // #if UNITY_EDITOR
        //         Debug.Log($"Update continous haptic: {intensity}-{sharpness}-{DateTime.Now.TimeOfDay}");
        // #endif
    }
    public void StopContinuousHaptic()
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.StopContinuousHaptic();
    }
    public void StopAllHaptics()
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.StopAllHaptics();
    }
    public void SetActiveService(bool active)
    {
        Assert.IsNotNull(m_HapticServiceImpl, "Missing implementation of haptic service");
        m_HapticServiceImpl.SetHapticsActive(active);
    }
}