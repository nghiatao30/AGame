using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LatteGames
{
    public class EZAnimSequence : EZAnimBase
    {
        [SerializeField] protected float delay;
        [SerializeField] protected List<EZAnimInfo> animInfos;

        protected Coroutine coroutine;
#if UNITY_EDITOR
        protected EditorCoroutine.Coroutine editorCoroutine;
#endif

        public override void SetToStart()
        {
            foreach (var animInfo in animInfos)
            {
                foreach (var anim in animInfo.anims)
                {
                    anim.SetToStart();
                }
            }
        }

        public override void SetToEnd()
        {
            foreach (var animInfo in animInfos)
            {
                foreach (var anim in animInfo.anims)
                {
                    anim.SetToEnd();
                }
            }
        }

        public override void Play(Action onComplete)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && editorCoroutine != null)
            {
                EditorCoroutine.Stop(editorCoroutine);
            }
            else
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
#else
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
#endif

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                editorCoroutine = EditorCoroutine.Execute(CR_Play(onComplete));
            }
            else
            {
                coroutine = StartCoroutine(CR_Play(onComplete));
            }
#else
            coroutine = StartCoroutine(CR_Play(onComplete));
#endif

        }

        public override void InversePlay(Action onComplete)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && editorCoroutine != null)
            {
                EditorCoroutine.Stop(editorCoroutine);
            }
            else
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
#else
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
#endif

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                editorCoroutine = EditorCoroutine.Execute(CR_InversePlay(onComplete));
            }
            else
            {
                coroutine = StartCoroutine(CR_InversePlay(onComplete));
            }
#else
            coroutine = StartCoroutine(CR_InversePlay(onComplete));
#endif

        }

        public virtual IEnumerator CR_Play(Action onComplete)
        {
            yield return isIgnoreTimeScale ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
            foreach (var animInfo in animInfos)
            {
                foreach (var anim in animInfo.anims)
                {
                    anim.Play();
                }
                if (animInfo.waitForSeconds > 0)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        var lastTimeSinceStartup = (float)UnityEditor.EditorApplication.timeSinceStartup;
                        while ((float)UnityEditor.EditorApplication.timeSinceStartup - lastTimeSinceStartup < animInfo.waitForSeconds)
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return isIgnoreTimeScale ? new WaitForSecondsRealtime(animInfo.waitForSeconds) : new WaitForSeconds(animInfo.waitForSeconds);
                    }
#else
                    yield return isIgnoreTimeScale ? new WaitForSecondsRealtime(animInfo.waitForSeconds) : new WaitForSeconds(animInfo.waitForSeconds);
#endif

                }
            }
            onComplete?.Invoke();
        }

        public virtual IEnumerator CR_InversePlay(Action onComplete)
        {
            yield return isIgnoreTimeScale ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
            for (var i = animInfos.Count - 1; i >= 0; i--)
            {
                var animInfo = animInfos[i];
                if (animInfo.waitForSeconds > 0)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        var lastTimeSinceStartup = (float)UnityEditor.EditorApplication.timeSinceStartup;
                        while ((float)UnityEditor.EditorApplication.timeSinceStartup - lastTimeSinceStartup < animInfo.waitForSeconds)
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return isIgnoreTimeScale ? new WaitForSecondsRealtime(animInfo.waitForSeconds) : new WaitForSeconds(animInfo.waitForSeconds);
                    }
#else
                    yield return isIgnoreTimeScale ? new WaitForSecondsRealtime(animInfo.waitForSeconds) : new WaitForSeconds(animInfo.waitForSeconds);
#endif

                }
                foreach (var anim in animInfo.anims)
                {
                    anim.InversePlay();
                }
            }
            onComplete?.Invoke();
        }

        [Serializable]
        public struct EZAnimInfo
        {
            public List<EZAnimBase> anims;
            public float waitForSeconds;
        }
    }
}