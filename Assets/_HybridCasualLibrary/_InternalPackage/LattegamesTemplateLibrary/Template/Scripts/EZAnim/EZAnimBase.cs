using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EZAnimBase : MonoBehaviour
{
    [SerializeField]
    protected bool isIgnoreTimeScale;

    [Button, HorizontalGroup("Set Group")]
    public virtual void SetToStart()
    {

    }

    [Button, HorizontalGroup("Set Group")]
    public virtual void SetToEnd()
    {

    }

    [Button, HorizontalGroup("Play Group")]
    public virtual void Play()
    {
        Play(null);
    }

    [Button, HorizontalGroup("Play Group")]
    public virtual void InversePlay()
    {
        InversePlay(null);
    }

    public virtual void Play(Action onComplete)
    {

    }

    public virtual void InversePlay(Action onComplete)
    {

    }
}