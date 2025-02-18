using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class OnTriggerCallback : MonoBehaviour
{
    public event Action<Collider> onTriggerEnter = delegate { };
    public event Action<Collider> onTriggerStay = delegate { };
    public event Action<Collider> onTriggerExit = delegate { };

    public bool isFilterByTag = true;
    [TagSelector, ShowIf("isFilterByTag")]
    public List<string> tagFilter = new List<string>() { "Untagged" };

    [SerializeField]
    private UnityEvent onTriggerEnterEvent;
    [SerializeField]
    private UnityEvent onTriggerStayEvent;
    [SerializeField]
    private UnityEvent onTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!isFilterByTag || tagFilter.Any(item => other.CompareTag(item)))
        {
            onTriggerEnter?.Invoke(other);
            onTriggerEnterEvent?.Invoke();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isFilterByTag || tagFilter.Any(item => other.CompareTag(item)))
        {
            onTriggerStay?.Invoke(other);
            onTriggerStayEvent?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isFilterByTag || tagFilter.Any(item => other.CompareTag(item)))
        {
            onTriggerExit?.Invoke(other);
            onTriggerExitEvent?.Invoke();
        }
    }
}