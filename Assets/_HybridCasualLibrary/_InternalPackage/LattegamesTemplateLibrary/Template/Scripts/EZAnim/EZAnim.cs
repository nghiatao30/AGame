using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LatteGames
{
    public class EZAnim<T> : EZAnimBase
    {
        protected Action<float> AnimationCallBack;

        [SerializeField, BoxGroup("General")]
        protected float duration = 0;
        [SerializeField, BoxGroup("General")]
        protected AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField, BoxGroup("General")]
        protected bool defaultIsStart = true;
        [SerializeField, BoxGroup("Specific")]
        protected T from;
        [SerializeField, BoxGroup("Specific")]
        protected T to;

        public T From { get => from; set => from = value; }
        public T To { get => to; set => to = value; }

        protected bool hasFirstRun;
        protected Coroutine coroutine;
#if UNITY_EDITOR
        protected EditorCoroutine.Coroutine editorCoroutine;
#endif

        private void Awake()
        {
            if (!hasFirstRun)
            {
                if (defaultIsStart)
                {
                    SetToStart();
                }
                else
                {
                    SetToEnd();
                }
            }
        }

        public override void SetToStart()
        {
            if (!hasFirstRun)
            {
                hasFirstRun = true;
            }
            SetAnimationCallBack();
            AnimationCallBack?.Invoke(0f);
        }

        public override void SetToEnd()
        {
            if (!hasFirstRun)
            {
                hasFirstRun = true;
            }
            SetAnimationCallBack();
            AnimationCallBack?.Invoke(1);
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

            StartLerp(false, onComplete);
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
            StartLerp(true, onComplete);
        }

        protected virtual void StartLerp(bool isInverse, Action onComplete)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            SetAnimationCallBack();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                editorCoroutine = EditorCoroutine.Execute(Lerp(isInverse, onComplete));
            }
            else
            {
                coroutine = StartCoroutine(Lerp(isInverse, onComplete));
            }
#else
            coroutine = StartCoroutine(Lerp(isInverse, onComplete));
#endif
        }

        protected virtual IEnumerator Lerp(bool isInverse, Action onComplete)
        {
            if (!hasFirstRun)
            {
                hasFirstRun = true;
            }
            float t = 0.0f;
            float GetLerpValue(float t)
            {
                if (isInverse)
                {
                    return 1 - t;
                }
                return t;
            }
            AnimationCallBack?.Invoke(GetLerpValue(animationCurve.Evaluate(0 / duration)));
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var lastTimeSinceStartup = (float)UnityEditor.EditorApplication.timeSinceStartup;
                while (t < duration)
                {
                    yield return null;
                    var editorDeltaTime = (float)UnityEditor.EditorApplication.timeSinceStartup - lastTimeSinceStartup;
                    lastTimeSinceStartup = (float)UnityEditor.EditorApplication.timeSinceStartup;
                    t += editorDeltaTime;
                    AnimationCallBack?.Invoke(GetLerpValue(animationCurve.Evaluate(t / duration)));
                }
            }
            else
            {
                while (t < duration)
                {
                    yield return null;
                    t += isIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                    AnimationCallBack?.Invoke(GetLerpValue(animationCurve.Evaluate(t / duration)));
                }
            }
#else
            while (t < duration)
            {
                yield return null;
                t += isIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                AnimationCallBack?.Invoke(GetLerpValue(animationCurve.Evaluate(t / duration)));
            }
#endif
            AnimationCallBack?.Invoke(GetLerpValue(animationCurve.Evaluate(1 / duration)));
            onComplete?.Invoke();
        }

        protected virtual void SetAnimationCallBack()
        {

        }
    }
}