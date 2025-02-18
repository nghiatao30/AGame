using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class StringSelectorAttribute : PropertyAttribute
{
    public Type classType { get; private set; }
    public string optionsGetterMethodName { get; private set; }

    /// <summary>
    /// Constructor for attribute
    /// </summary>
    /// <param name="optionsGetterMethodName">Method to get options. Method must be static</param>
    /// <param name="classType"></param>
    public StringSelectorAttribute(string optionsGetterMethodName, Type classType)
    {
        this.classType = classType;
        this.optionsGetterMethodName = optionsGetterMethodName;
    }
}