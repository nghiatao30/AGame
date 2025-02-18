using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum ApplicationLifecycleEventCode
{
    /// <summary>
    /// This event is raised when the application gains or loses focus
    /// <para> <typeparamref name="bool"/>: focus </para>
    /// </summary>
    OnApplicationFocus,
    /// <summary>
    /// This event is raised when the application is paused or resumed
    /// <para> <typeparamref name="bool"/>: pauseStatus </para>
    /// </summary>
    OnApplicationPause,
    /// <summary>
    /// This event is raised when the application quit
    /// </summary>
    OnApplicationQuit,
}