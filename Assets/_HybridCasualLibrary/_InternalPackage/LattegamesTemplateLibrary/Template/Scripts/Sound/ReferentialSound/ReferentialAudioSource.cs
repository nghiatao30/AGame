using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LatteGames
{
    [RequireComponent(typeof(AudioSource))]
    public class ReferentialAudioSource : MonoBehaviour
    {
        private AudioSource audioSource;
        public AudioSource Source => audioSource;
        [SerializeField] private ReferentialSoundClip clip;
        public ReferentialSoundClip Clip
        {
            set
            {
                clip = value;
                audioSource.clip = clip.Clip;
                if (audioSource.isPlaying)
                    audioSource.Play();
            }
            get => clip;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (clip.Clip == null) return;
            if (clip.clips.Length > 0)
            {
                StartCoroutine(CR_PlaySound());
                return;
            }
            if (clip != null) audioSource.clip = clip.Clip;
            if (audioSource.playOnAwake)
                audioSource.Play();
        }
        IEnumerator CR_PlaySound()
        {
            for (int i = 0; i < clip.clips.Length; i++)
            {
                audioSource.clip = clip.clips[i];
                if (audioSource.playOnAwake)
                    audioSource.Play();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}