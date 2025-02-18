using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class OnCollisionCallback : MonoBehaviour
{
    public event Action<Collision> onCollisionEnter = delegate { };
    public event Action<Collision> onCollisionStay = delegate { };
    public event Action<Collision> onCollisionExit = delegate { };

    public bool isFilterByTag = true;
    [TagSelector, ShowIf("isFilterByTag")]
    public List<string> tagFilter = new List<string>() { "Untagged" };

    [SerializeField]
    private UnityEvent onCollisionEnterEvent;
    [SerializeField]
    private UnityEvent onCollisionStayEvent;
    [SerializeField]
    private UnityEvent onCollisionExitEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFilterByTag || tagFilter.Any(item => collision.gameObject.CompareTag(item)))
        {
            onCollisionEnter?.Invoke(collision);
            onCollisionEnterEvent?.Invoke();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!isFilterByTag || tagFilter.Any(item => collision.gameObject.CompareTag(item)))
        {
            onCollisionStay?.Invoke(collision);
            onCollisionStayEvent?.Invoke();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!isFilterByTag || tagFilter.Any(item => collision.gameObject.CompareTag(item)))
        {
            onCollisionExit?.Invoke(collision);
            onCollisionExitEvent?.Invoke();
        }
    }
}