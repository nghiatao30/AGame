using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HyrphusQ.Events
{
    public enum ActivateBehaviour
    {
        Immediately,
        Delay
    }
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(50)]
    [AddComponentMenu("HyrphusQ/Events/ActivateOnlyRaiseEvent")]
    [Obsolete("This is obsolete and will be removed in the future", true)]
    public class ActivateOnlyRaiseEvent : MonoBehaviour
    {
        public UnityEvent onActiveEvent;

        [SerializeField]
        private ActivateBehaviour activateBehaviour;
        [SerializeField, DrawIf("DrawDelayTime")]
        private float delayTime = 1f;
        [SerializeField]
        private EventCode eventCode;

        private CoroutineRunnerTemporary coroutineRunner = null;

        private void Start()
        {
            GameEventHandler.AddActionEvent(eventCode, OnEventTrigger);

            if (activateBehaviour == ActivateBehaviour.Delay)
            {
                var coroutineRunnerGO = new GameObject("CoroutineRunnerTemp");
                coroutineRunnerGO.transform.SetParent(transform.parent);
                coroutineRunner = coroutineRunnerGO.AddComponent<CoroutineRunnerTemporary>();
            }

            gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            if (coroutineRunner != null)
                Destroy(coroutineRunner);
        }

        private IEnumerator ActivateGameObject_CR(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            ActiveGameObject();
        }
        private void ActiveGameObject()
        {
            gameObject.SetActive(true);
            onActiveEvent?.Invoke();
        }
        private void OnEventTrigger()
        {
            if (activateBehaviour == ActivateBehaviour.Immediately)
                ActiveGameObject();
            else
                coroutineRunner.StartCoroutine(ActivateGameObject_CR(delayTime));
        } 
        private bool DrawDelayTime() => activateBehaviour == ActivateBehaviour.Delay;

        class CoroutineRunnerTemporary : MonoBehaviour
        {

        }
    }
}