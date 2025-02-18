using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEvent : MonoBehaviour
{
    public event Action<Collision> OnColliderEnteredEvent = delegate {};
    public event Action<Collision> OnColliderExitedEvent = delegate {};
    public event Action<Collision> OnColliderStayEvent = delegate {};

    private List<Collision> enteredObjects = new List<Collision>();
    
    public List<string> TagFilter = new List<string>();
    public List<string> TagFilterExclude = new List<string>();

    private void OnCollisionEnter(Collision other) {
        if(TagFilter.Count > 0 && !TagFilter.Contains(other.gameObject.tag))
            return;
        if(TagFilterExclude.Count > 0 && TagFilterExclude.Contains(other.gameObject.tag))
            return;
        enteredObjects.Add(other);
        OnColliderEnteredEvent(other);
    }

    private void OnCollisionExit(Collision other) {
        if(TagFilter.Count > 0 && !TagFilter.Contains(other.gameObject.tag))
            return;
        if(TagFilterExclude.Count > 0 && TagFilterExclude.Contains(other.gameObject.tag))
            return;
        enteredObjects.Remove(other);
        OnColliderExitedEvent(other);
    }

    private void OnCollisionStay(Collision other) {
        if(TagFilter.Count > 0 && !TagFilter.Contains(other.gameObject.tag))
            return;
        if(TagFilterExclude.Count > 0 && TagFilterExclude.Contains(other.gameObject.tag))
            return;
        OnColliderStayEvent(other);
    }

    public List<Collision> GetEnteredObjects()
    {
        return enteredObjects;
    }
}
