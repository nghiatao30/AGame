using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public static class CommonCoroutine
    {
        public static IEnumerator LerpFactor(float duration, Action<float> callback)
        {
            float t = 0.0f;
            callback(t / duration);
            while (t < duration)
            {
                yield return null;
                t += Time.deltaTime;
                callback(t / duration);
            }
            callback(1);
        }

        public static IEnumerator LerpAnimation(float duration, AnimationCurve timeCurve, Action<float> callback)
        {
            return LerpFactor(duration, f => callback(timeCurve.Evaluate(f)));
        }

        public static IEnumerator Delay(float time, bool waitInRealtime, Action callback)
        {
            if (waitInRealtime)
                yield return new WaitForSecondsRealtime(time);
            else
                yield return new WaitForSeconds(time);
            callback();
        }

        public static IEnumerator Interval(
            float interval,
            bool waitInRealtime,
            Func<bool> stopCondition,
            Action<int> callback)
        {
            int count = 0;
            callback(count);
            while (stopCondition() == false)
            {
                count++;
                yield return Delay(interval, waitInRealtime, () => callback(count));
            }
        }

        public static IEnumerator Wait(IEnumerator enumerator, Action callback)
        {
            yield return enumerator;
            callback();
        }

        public static IEnumerator WaitUntil(Func<bool> predicate, Action callback)
        {
            yield return new WaitUntil(predicate);
            callback?.Invoke();
        }
        static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);

        static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { return _endOfFrame; }
        }

        static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get { return _fixedUpdate; }
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (!_timeInterval.ContainsKey(seconds))
                _timeInterval.Add(seconds, new WaitForSeconds(seconds));
            return _timeInterval[seconds];
        }
    }
}