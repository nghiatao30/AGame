using System;
using UnityEngine;
using UnityEngine.Audio;

namespace LatteGames
{
    [CreateAssetMenu(fileName = "MixerVolumeControl", menuName = "LatteGames/ScriptableObject/MixerVolumeControl", order = 0)]
    public class MixerVolumeControl : ScriptableObject {
        [SerializeField] private AudioMixer mixer = null;
        [SerializeField] private string playerPrefPrefix = "AudioPreference_";
        
        private PlayerPrefPersistent.Boolean backgroundMusicEnabled => new PlayerPrefPersistent.Boolean($"{playerPrefPrefix}_BGM_ON", true);
        private PlayerPrefPersistent.Boolean fXEnabled => new PlayerPrefPersistent.Boolean($"{playerPrefPrefix}_FX_ON", true);

        private PlayerPrefPersistent.Float backgroundVolume => new PlayerPrefPersistent.Float($"{playerPrefPrefix}_BGM_VOL", 1);
        private PlayerPrefPersistent.Float fXVolume => new PlayerPrefPersistent.Float($"{playerPrefPrefix}_FX_VOL", 1);

        public AudioMixer Mixer => mixer;

        public bool IsBGMEnabled {
            get => backgroundMusicEnabled.Value;
            set {
                backgroundMusicEnabled.Value = value;
                UpdateMixer();
            }
        }

        public bool IsFXEnabled {
            get => fXEnabled.Value;
            set {
                fXEnabled.Value = value;
                UpdateMixer();
            }
        }

        public float BGMVol {
            get => IsBGMEnabled?backgroundVolume.Value:0;
            set {
                if(IsBGMEnabled)
                    backgroundVolume.Value = value;
                UpdateMixer();
            }
        }
        public float FXVol {
            get => IsFXEnabled?fXVolume.Value:0;
            set {
                if(IsFXEnabled)
                    fXVolume.Value = value;
                UpdateMixer();
            }
        }

        private void OnEnable() {
            UpdateMixer();
        }

        public void UpdateMixer()
        {
            if(mixer == null)
                return;
            mixer.SetFloat("BGM_Volume", LinearToLogVolume(BGMVol));
            mixer.SetFloat("FX_Volume", LinearToLogVolume(FXVol));
            mixer.SetFloat("IMP_Volume", LinearToLogVolume(FXVol));
        }

        private float LinearToLogVolume(float linearScale)
        {
            return  Mathf.Log10(Mathf.Clamp(linearScale, 0.0001f, 1)) * 20;
        }
    }
}