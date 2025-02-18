using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LatteGames
{
    public class EZAnimUnityEvent<T> : EZAnim<T>
    {
        [SerializeField, BoxGroup("Specific")]
        protected UnityEvent<T> UnityAnimationCallBack;

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = 0; i < UnityAnimationCallBack?.GetPersistentEventCount(); i++)
            {
                UnityAnimationCallBack?.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
            }
        }
#endif
    }
}
