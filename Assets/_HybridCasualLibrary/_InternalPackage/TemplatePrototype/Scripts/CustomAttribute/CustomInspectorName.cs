using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
public class CustomInspectorName : PropertyAttribute
{
    //
    // Summary:
    //     Name to display in the Inspector.
    public readonly string displayName;

    //
    // Summary:
    //     Specify a display name for an enum value.
    //
    // Parameters:
    //   displayName:
    //     The name to display.
    public CustomInspectorName(string displayName)
    {
        this.displayName = displayName;
    }
}