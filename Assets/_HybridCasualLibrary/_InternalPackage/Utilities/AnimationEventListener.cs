using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(AnimationEventTrigger))]
public class AnimationEventListener : MonoBehaviour
{
    [Serializable]
    private struct EventTuple
    {
        public string eventString;
        public UnityEvent action;
    }

    [SerializeField] List<EventTuple> eventList = new();

    AnimationEventTrigger trigger;

    private void Awake()
    {
        trigger = GetComponent<AnimationEventTrigger>();
        trigger.OnAnimationEventTriggered += HandleAnimationEventTriggered;
    }

    private void OnDestroy()
    {
        if (trigger == null) return;
        trigger.OnAnimationEventTriggered -= HandleAnimationEventTriggered;
    }

    void HandleAnimationEventTriggered(AnimationEventTrigger.TriggeredEventData data)
    {
        foreach (var _event in eventList)
        {
            if (_event.eventString.Equals(data.stringParam)) _event.action.Invoke();
        }
    }
}
