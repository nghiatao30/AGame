using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public event Action<Collider> OnTriggerEnteredEvent = delegate {};
    public event Action<Collider> OnTriggerExitedEvent = delegate {};
    public event Action<Collider> OnTriggerStayEvent = delegate {};

    private List<Collider> enteredObjects = new List<Collider>();
    
    public List<string> TagFilter = new List<string>();
    public List<string> TagFilterExclude = new List<string>();

    private void OnTriggerEnter(Collider other) {
        if(TagFilter.Count > 0 && !TagFilter.Contains(other.tag))
            return;
        if(TagFilterExclude.Count > 0 && TagFilterExclude.Contains(other.tag))
            return;
        enteredObjects.Add(other);
        OnTriggerEnteredEvent(other);
    }
    private void OnTriggerExit(Collider other) {
        if(TagFilter.Count > 0 && !TagFilter.Contains(other.tag))
            return;
        if(TagFilterExclude.Count > 0 && TagFilterExclude.Contains(other.tag))
            return;
        enteredObjects.Remove(other);
        OnTriggerExitedEvent(other);
    }

    private void OnTriggerStay(Collider other) {
        if(TagFilter.Count > 0 && !TagFilter.Contains(other.tag))
            return;
        if(TagFilterExclude.Count > 0 && TagFilterExclude.Contains(other.tag))
            return;
        OnTriggerStayEvent(other);
    }

    public List<Collider> GetEnteredObjects()
    {
        return enteredObjects;
    }
}
