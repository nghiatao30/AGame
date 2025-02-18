using UnityEngine;
using System;

public class AnimationEventTrigger : MonoBehaviour
{
    public event Action<TriggeredEventData> OnAnimationEventTriggered = delegate { };

    public void TriggerAnimationEvent(string stringParam)
    {
        OnAnimationEventTriggered(new TriggeredEventData()
        {
            animationEventTrigger = this,
            stringParam = stringParam
        });
    }

    public Delegate[] GetInvocationList() => OnAnimationEventTriggered.GetInvocationList();

    public class TriggeredEventData
    {
        public AnimationEventTrigger animationEventTrigger;
        public string stringParam;
    }
}
