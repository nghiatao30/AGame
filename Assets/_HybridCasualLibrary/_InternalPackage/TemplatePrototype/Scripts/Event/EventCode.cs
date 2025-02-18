using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    [Serializable]
    public class EventCode
    {
        #region Constructors
        public EventCode()
        {

        }
        public EventCode(string eventCode, string eventType)
        {
            m_EventCode = eventCode;
            m_EventType = eventType;
        }
        public EventCode(Enum enumValue)
        {
            m_EventCode = enumValue.ToString();
            m_EventType = enumValue.GetType().AssemblyQualifiedName;
        }
        #endregion

        [SerializeField]
        private string m_EventType;
        [SerializeField]
        private string m_EventCode;

        public Enum eventCode => this;

        public static implicit operator Enum(EventCode eventCode)
        {
            if (eventCode == null || string.IsNullOrEmpty(eventCode.m_EventType) || string.IsNullOrEmpty(eventCode.m_EventCode))
                return null;
            return Enum.Parse(Type.GetType(eventCode.m_EventType), eventCode.m_EventCode) as Enum;
        }
    }
}