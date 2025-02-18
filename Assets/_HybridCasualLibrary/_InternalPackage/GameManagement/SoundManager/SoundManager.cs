using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Audio;
using DG.Tweening;

namespace LatteGames.Template
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField, Tooltip("General sound settings preference. If this is turned off, both SFX and BGM will also be off")]
        PPrefBoolVariable m_Sound_PPref;
        [SerializeField, Tooltip("Invidual SFX settings preference")]
        PPrefBoolVariable m_SFXSound_PPref;
        [SerializeField, Tooltip("Invidual BGM settings preference")]
        PPrefBoolVariable m_BGMSound_PPref;
        [SerializeField]
        SoundLibrarySO m_GeneralSFXLibrarySO;
        [SerializeField]
        SoundLibrarySO m_GeneralBGMLibrarySO;
        [SerializeField]
        AudioSource m_SFXAudioSource;
        [SerializeField]
        AudioSource m_BGMAudioSource;
        [SerializeField]
        MixerVolumeControl m_MixerVolumeControl;
        List<SoundLibrarySO> m_SceneSFXLibrarySO = new List<SoundLibrarySO>();
        List<SoundLibrarySO> m_SceneBGMLibrarySO = new List<SoundLibrarySO>();

        Dictionary<GameObject, AudioSource> m_LoopSource = new Dictionary<GameObject, AudioSource>();

        // public AudioSource SFXAudioSource => m_SFXAudioSource;
        // public AudioSource BGMAudioSource => m_BGMAudioSource; //better use built-in methods
        private KeyValuePair<Enum, AudioClip> m_CurrentPlayingBGM;
        public KeyValuePair<Enum, AudioClip> CurrentPlayingBGM => m_CurrentPlayingBGM.Key == null ? new KeyValuePair<Enum, AudioClip>(null, null) : m_CurrentPlayingBGM;

        protected override void Awake()
        {
            base.Awake();
            m_Sound_PPref.onValueChanged += OnSoundPrefValueChanged;
            m_SFXSound_PPref.onValueChanged += OnSFXSoundPrefValueChanged;
            m_BGMSound_PPref.onValueChanged += OnBGMSoundPrefValueChanged;
            m_BGMAudioSource.enabled = m_Sound_PPref.value;
            m_SFXAudioSource.enabled = m_Sound_PPref.value;
        }

        private void Start()
        {
            m_MixerVolumeControl.IsBGMEnabled = m_Sound_PPref.value;
            m_MixerVolumeControl.IsFXEnabled = m_Sound_PPref.value;
        }

        private void OnBGMSoundPrefValueChanged(ValueDataChanged<bool> changed)
        {
            m_MixerVolumeControl.IsBGMEnabled = m_BGMSound_PPref.value;
        }

        private void OnSFXSoundPrefValueChanged(ValueDataChanged<bool> changed)
        {
            m_MixerVolumeControl.IsFXEnabled = m_SFXSound_PPref.value;
        }

        private void OnDestroy()
        {
            m_Sound_PPref.onValueChanged -= OnSoundPrefValueChanged;
            m_SFXSound_PPref.onValueChanged -= OnSFXSoundPrefValueChanged;
            m_BGMSound_PPref.onValueChanged -= OnBGMSoundPrefValueChanged;
        }

        private void OnSoundPrefValueChanged(ValueDataChanged<bool> obj)
        {
            if (!m_Sound_PPref.value)
            {
                m_BGMAudioSource.Stop();
                m_SFXAudioSource.Stop();
            }
            m_BGMAudioSource.enabled = m_Sound_PPref.value;
            m_SFXAudioSource.enabled = m_Sound_PPref.value;
            m_MixerVolumeControl.IsBGMEnabled = m_Sound_PPref.value;
            m_MixerVolumeControl.IsFXEnabled = m_Sound_PPref.value;
        }

        public void AddSceneSoundLibrary(SceneSoundLibrary sceneSoundLibrary)
        {
            // var existenceSFXLibrarySO = m_SceneSFXLibrarySO.Intersect(sceneSoundLibrary.SceneSFXLibrarySO).ToList();
            // var existenceBGMLibrarySO = m_SceneBGMLibrarySO.Intersect(sceneSoundLibrary.SceneBGMLibrarySO).ToList();
            // var filtertedSFXLibrarySO = new List<SoundLibrarySO>(sceneSoundLibrary.SceneSFXLibrarySO.RemoveAll(x => existenceSFXLibrarySO.Contains(x)));
            // var filtertedBGMLibrarySO = new List<SoundLibrarySO>(sceneSoundLibrary.SceneBGMLibrarySO.RemoveAll(x => existenceBGMLibrarySO.Contains(x)));
            m_SceneSFXLibrarySO.AddRange(sceneSoundLibrary.SceneSFXLibrarySO);
            m_SceneBGMLibrarySO.AddRange(sceneSoundLibrary.SceneBGMLibrarySO);
        }

        public void RemoveSceneSoundLibrary(SceneSoundLibrary sceneSoundLibrary)
        {
            m_SceneSFXLibrarySO.RemoveAll(item => sceneSoundLibrary.SceneSFXLibrarySO.Contains(item));
            m_SceneBGMLibrarySO.RemoveAll(item => sceneSoundLibrary.SceneBGMLibrarySO.Contains(item));
            m_SFXAudioSource.Stop();
            m_BGMAudioSource.Stop();
        }

        /// <summary>
        /// Play oneshot SFX with given audio clip
        /// </summary>
        /// <param name="clip"></param>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null)
                return;
            m_SFXAudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Play oneshot SFX by enum key with volumn
        /// </summary>
        /// <param name="SFXEnumKey">SFX enum key defined in SFX Sound Library SO</param>
        /// <param name="volumn">SFX volumn</param>
        ///
        public void PlaySFX(Enum SFXEnumKey, float volumn = 1f)
        {
            string sfxKey = String.Format("{0}.{1}", SFXEnumKey.GetType().Name, SFXEnumKey.ToString());
            if (m_GeneralSFXLibrarySO.Library.ContainsKey(sfxKey))
            {
                var clip = m_GeneralSFXLibrarySO.Library[sfxKey];
                m_SFXAudioSource.PlayOneShot(clip, volumn);
            }
            else
            {
                if (m_SceneSFXLibrarySO != null)
                {
                    var soundAvailable = false;
                    foreach (var lib in m_SceneSFXLibrarySO)
                    {
                        if (lib.Library.ContainsKey(sfxKey))
                        {
                            var clip = lib.Library[sfxKey];
                            m_SFXAudioSource.PlayOneShot(clip, volumn);
                            soundAvailable = true;
                            break;
                        }
                    }
                    if (!soundAvailable)
                        Debug.LogWarning($"{sfxKey} not found");
                }
                else
                {
                    Debug.LogWarning($"{sfxKey} not found");
                }
            }
        }
        public void PlayLoopSFX(Enum SFXEnumKey, float volumn = 1f, bool loop = false, bool is3DSound = false, GameObject ownedGameObject = null)
        {
            if (!loop)
            {
                PlaySFX(SFXEnumKey, volumn);
            }
            else
            {
                if (ownedGameObject == null)
                    return;
                if (m_LoopSource.ContainsKey(ownedGameObject))
                {
                    var loopAudioSource = m_LoopSource[ownedGameObject];
                    string sfxKey = String.Format("{0}.{1}", SFXEnumKey.GetType().Name, SFXEnumKey.ToString());
                    if (m_GeneralSFXLibrarySO.Library.ContainsKey(sfxKey))
                    {
                        var clip = m_GeneralSFXLibrarySO.Library[sfxKey];
                        if (clip != loopAudioSource.clip)
                        {
                            loopAudioSource.Stop();
                            loopAudioSource.clip = clip;
                            loopAudioSource.volume = volumn;
                            loopAudioSource.Play();
                        }
                        else
                        {
                            loopAudioSource.volume = volumn;
                        }
                    }
                    else
                    {
                        if (m_SceneSFXLibrarySO != null)
                        {
                            var soundAvailable = false;
                            foreach (var lib in m_SceneSFXLibrarySO)
                            {
                                if (lib.Library.ContainsKey(sfxKey))
                                {
                                    var clip = lib.Library[sfxKey];
                                    if (clip != loopAudioSource.clip)
                                    {
                                        loopAudioSource.Stop();
                                        loopAudioSource.clip = clip;
                                        loopAudioSource.volume = volumn;
                                        loopAudioSource.Play();



                                    }
                                    else
                                    {
                                        loopAudioSource.volume = volumn;
                                    }
                                    soundAvailable = true;
                                    break;
                                }
                            }
                            if (!soundAvailable)
                                Debug.LogWarning($"{sfxKey} not found");
                        }
                        else
                        {
                            Debug.LogWarning($"{sfxKey} not found");
                        }
                    }
                    if (is3DSound)
                    {
                        loopAudioSource.rolloffMode = AudioRolloffMode.Linear;
                        loopAudioSource.minDistance = 0f;
                        loopAudioSource.maxDistance = 1f;
                    }
                }
                else
                {
                    GameObject loopAudioSourceGO = new GameObject();
                    AudioSource loopAudioSource = loopAudioSourceGO.AddComponent<AudioSource>();
                    m_LoopSource.Add(ownedGameObject, loopAudioSource);
                    loopAudioSourceGO.transform.parent = ownedGameObject.transform;
                    loopAudioSourceGO.transform.localPosition = Vector3.zero;
                    loopAudioSourceGO.name = ownedGameObject.name + "_AudioSource";
                    string sfxKey = String.Format("{0}.{1}", SFXEnumKey.GetType().Name, SFXEnumKey.ToString());
                    if (m_GeneralSFXLibrarySO.Library.ContainsKey(sfxKey))
                    {
                        var clip = m_GeneralSFXLibrarySO.Library[sfxKey];
                        loopAudioSource.clip = clip;
                        loopAudioSource.loop = true;
                        loopAudioSource.playOnAwake = false;
                        loopAudioSource.volume = volumn;
                        loopAudioSource.outputAudioMixerGroup = m_SFXAudioSource.outputAudioMixerGroup;
                        loopAudioSource.Play();
                    }
                    else
                    {
                        if (m_SceneSFXLibrarySO != null)
                        {
                            var soundAvailable = false;
                            foreach (var lib in m_SceneSFXLibrarySO)
                            {
                                if (lib.Library.ContainsKey(sfxKey))
                                {
                                    var clip = lib.Library[sfxKey];
                                    loopAudioSource.clip = clip;
                                    loopAudioSource.loop = true;
                                    loopAudioSource.playOnAwake = false;
                                    loopAudioSource.outputAudioMixerGroup = m_SFXAudioSource.outputAudioMixerGroup;
                                    loopAudioSource.volume = volumn;
                                    loopAudioSource.Play();
                                    soundAvailable = true;
                                    break;
                                }
                            }
                            if (!soundAvailable)
                                Debug.LogWarning($"{sfxKey} not found");
                        }
                        else
                        {
                            Debug.LogWarning($"{sfxKey} not found");
                        }
                    }
                    if (is3DSound)
                    {
                        loopAudioSource.rolloffMode = AudioRolloffMode.Linear;
                        loopAudioSource.minDistance = 0f;
                        loopAudioSource.maxDistance = 1f;

                    }
                }
            }
        }

        /// <summary>
        /// Stop the current playing SFX 
        /// </summary>
        public void StopCurrentSFX()
        {
            if (m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();
        }
        /// <summary>
        /// Pause the looped SFX
        /// </summary>
        /// <param name="ownedGameObject"></param>
        public void PauseLoopSFX(GameObject ownedGameObject)
        {
            if (ownedGameObject == null) return;
            foreach (var i in m_LoopSource)
            {
                if (i.Key.Equals(ownedGameObject))
                {
                    var currentVolume = m_LoopSource[ownedGameObject].volume;

                    StartCoroutine(CommonCoroutine.LerpFactor(0.2f, (float t) =>
                    {
                        if (m_LoopSource.ContainsKey(ownedGameObject) && m_LoopSource[ownedGameObject] != null && ownedGameObject != null)
                        {
                            t = math.remap(0f, 1f, 0f, currentVolume, t);
                            m_LoopSource[ownedGameObject].volume = currentVolume - t;
                        }
                    }));

                    break;
                }
            }
        }

        /// <summary>
        /// Stop the looped SFX
        /// </summary>
        /// <param name="ownedGameObject"></param>
        public void StopLoopSFX(GameObject ownedGameObject)
        {
            if (ownedGameObject == null) return;
            foreach (var i in m_LoopSource)
            {
                if (i.Key.Equals(ownedGameObject))
                {
                    m_LoopSource[ownedGameObject].Stop();
                    Destroy(m_LoopSource[ownedGameObject].gameObject);
                    m_LoopSource.Remove(ownedGameObject);
                    break;
                }
            }
        }

        /// <summary>
        /// Play BGM by enum key
        /// </summary>
        /// <param name="BGMEnumKey">BGM enum key defined in BGM Sound Library SO</param>
        public void PlayBGM(Enum BGMEnumKey)
        {
            string bgmKey = String.Format("{0}.{1}", BGMEnumKey.GetType().Name, BGMEnumKey.ToString());
            if (m_GeneralBGMLibrarySO != null && m_GeneralBGMLibrarySO.Library.ContainsKey(bgmKey))
            {
                var clip = m_GeneralBGMLibrarySO.Library[bgmKey];
                m_CurrentPlayingBGM = new KeyValuePair<Enum, AudioClip>(BGMEnumKey, clip);
                m_BGMAudioSource.clip = clip;
                m_BGMAudioSource.Play();
            }
            else
            {
                if (m_SceneBGMLibrarySO != null)
                {
                    var soundAvailable = false;
                    foreach (var lib in m_SceneBGMLibrarySO)
                    {
                        if (lib.Library.ContainsKey(bgmKey))
                        {
                            var clip = lib.Library[bgmKey];
                            m_BGMAudioSource.clip = clip;
                            m_BGMAudioSource.Play();
                            soundAvailable = true;
                            m_CurrentPlayingBGM = new KeyValuePair<Enum, AudioClip>(BGMEnumKey, clip);
                            break;
                        }
                    }
                    if (!soundAvailable)
                        Debug.LogWarning($"{BGMEnumKey} not found");
                }
                else
                {
                    Debug.LogWarning($"{BGMEnumKey} not found");
                }
            }
        }

        /// <summary>
        /// Stop the current playing BGM 
        /// </summary>
        public void StopCurrentBGM()
        {
            if (m_BGMAudioSource.isPlaying)
                m_BGMAudioSource.Stop();
        }

        public void SetSFXMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            if (m_SFXAudioSource == null)
                throw new Exception("SFX audio source is null");
            m_SFXAudioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        public AudioMixerGroup GetSFXMixerGroup()
        {
            return m_SFXAudioSource == null ? null : m_SFXAudioSource.outputAudioMixerGroup;
        }

        public void SetBGMMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            if (m_BGMAudioSource == null)
                throw new Exception("BGM audio source is null");
            m_BGMAudioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        public AudioMixerGroup GetBGMMixerGroup()
        {
            return m_BGMAudioSource == null ? null : m_BGMAudioSource.outputAudioMixerGroup;
        }

        public void DOFadeAudioVolume(AudioType audioType, float endValue, float duration)
        {
            switch (audioType)
            {
                case AudioType.SFX:
                    {
                        m_SFXAudioSource.DOFade(endValue, duration);
                        break;
                    }
                case AudioType.BGM:
                    {
                        m_BGMAudioSource.DOFade(endValue, duration);
                        break;
                    }
            }
        }
    }
}

public enum AudioType
{
    SFX,
    BGM
}