using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HyrphusQ.Events
{
    [DisallowMultipleComponent]
    [AddComponentMenu("HyrphusQ/Events/GameEventListeners")]
    public class GameEventListeners : MonoBehaviour
    {
        [Serializable]
        public class EventTuple
        {
            public EventCode m_EventCode;
            public UnityEvent m_UnityEvent;

            public EventTuple(EventCode eventCode)
            {
                m_EventCode = eventCode;
                m_UnityEvent = new UnityEvent();
            }
        }

        [HideInInspector]
        public List<EventTuple> m_EventsList = new List<EventTuple>();

        private void Awake()
        {
            foreach (var item in m_EventsList)
                GameEventHandler.AddActionEvent(item.m_EventCode, item.m_UnityEvent.Invoke);
        }
        private void OnDestroy()
        {
            foreach (var item in m_EventsList)
                GameEventHandler.RemoveActionEvent(item.m_EventCode, item.m_UnityEvent.Invoke);
        }

        public bool ContainsEvent(string eventCode)
        {
            return m_EventsList.Exists(item => item.m_EventCode.eventCode.ToString() == eventCode);
        }
    }
}