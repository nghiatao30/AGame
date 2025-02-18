using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    [AddComponentMenu("HyrphusQ/Events/GameEventNotifier")]
    public class GameEventNotifier : MonoBehaviour
    {
        public event Action<GameEventNotifier> onEventRaised;

        [SerializeField]
        private EventCode m_EventCode;
        [SerializeField]
        private List<UnityEngine.Object> m_ParameterObjects = new List<UnityEngine.Object>();

        private IEnumerator RaiseEventDelay_CR(float delayTime, Action callback = null)
        {
            yield return new WaitForSeconds(delayTime);
            callback?.Invoke();
        }

        public void RaiseEvent()
        {
            if (m_EventCode == null || m_EventCode.eventCode == null)
                return;
            var eventParams = new object[m_ParameterObjects.Count];
            for (int i = 0; i < m_ParameterObjects.Count; i++)
                eventParams[i] = m_ParameterObjects[i];
            onEventRaised?.Invoke(this);
            GameEventHandler.Invoke(m_EventCode.eventCode, null, eventParams);
        }
        public void RaiseEvent(float delayTime = AnimationDuration.SHORT)
        {
            StartCoroutine(RaiseEventDelay_CR(delayTime, RaiseEvent));
        }
    }
}