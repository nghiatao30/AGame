using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HyrphusQ.Events
{
    [EventCode]
    public enum LevelEventCode
    {
        OnWinLevel,
        OnLoseLevel
    }
    public class UnityParamEvent : UnityEvent<object[]> { }
    public static class GameEventHandler
    {
        private static Dictionary<Enum, UnityEvent> s_ActionEventDictionary = new Dictionary<Enum, UnityEvent>();
        private static Dictionary<Enum, UnityParamEvent> s_ParamActionEventDictionary = new Dictionary<Enum, UnityParamEvent>();

        public static void AddActionEvent(Enum eventID, UnityAction callback)
        {
            if (s_ActionEventDictionary.TryGetValue(eventID, out UnityEvent actionEvent))
            {
                actionEvent.AddListener(callback);
            }
            else
            {
                actionEvent = new UnityEvent();
                actionEvent.AddListener(callback);
                s_ActionEventDictionary.Add(eventID, actionEvent);
            }
        }

        public static void AddActionEvent(Enum eventID, UnityAction<object[]> callback)
        {
            if (s_ParamActionEventDictionary.TryGetValue(eventID, out UnityParamEvent actionEvent))
            {
                actionEvent.AddListener(callback);
            }
            else
            {
                actionEvent = new UnityParamEvent();
                actionEvent.AddListener(callback);
                s_ParamActionEventDictionary.Add(eventID, actionEvent);
            }
        }
        public static void Invoke(Enum eventID, params object[] param)
        {
            try
            {
                if (s_ActionEventDictionary.TryGetValue(eventID, out UnityEvent events))
                    events?.Invoke();
                if (s_ParamActionEventDictionary.TryGetValue(eventID, out UnityParamEvent paramEvents))
                    paramEvents?.Invoke(param);
            }
            catch (Exception exc)
            {
                Debug.LogError(eventID);
                Debug.LogException(exc);
            }
        }
        public static void RemoveActionEvent(Enum eventID, UnityAction callback)
        {
            try
            {
                if (s_ActionEventDictionary.ContainsKey(eventID))
                    s_ActionEventDictionary[eventID].RemoveListener(callback);
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }
        public static void RemoveActionEvent(Enum eventID, UnityAction<object[]> callback)
        {
            try
            {
                if (s_ParamActionEventDictionary.ContainsKey(eventID))
                    s_ParamActionEventDictionary[eventID].RemoveListener(callback);
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }
        public static void RemoveActionEvent(Enum eventID)
        {
            try
            {
                if (s_ActionEventDictionary.ContainsKey(eventID))
                    s_ActionEventDictionary[eventID].RemoveAllListeners();
                if (s_ParamActionEventDictionary.ContainsKey(eventID))
                    s_ParamActionEventDictionary[eventID].RemoveAllListeners();
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }
        public static bool IsEventExists(Enum eventID)
        {
            return s_ActionEventDictionary.ContainsKey(eventID) || s_ParamActionEventDictionary.ContainsKey(eventID);
        }
        public static void ClearAllActionEvents()
        {
            s_ActionEventDictionary.Clear();
            s_ParamActionEventDictionary.Clear();
        }
    }
}