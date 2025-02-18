using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example Stat struct implementation
/// </summary>
/// <typeparam name="TId">Type of stat id</typeparam>
/// <typeparam name="TValue">Type of stat value</typeparam>
[Obsolete("This is example only", true)]
public struct Stat<TId, TValue> : IStat<TId, TValue> where TId : Enum
{
    #region Constructor
    public Stat(TId id, TValue value)
    {
        m_Id = id;
        m_Value = value;
    }
    #endregion

    private TId m_Id;
    private TValue m_Value;

    public TId id => m_Id;
    public string label => "Example & No Implementation";
    public TValue value => m_Value;
}
public interface IStat<TId, TValue> where TId : Enum
{
    /// <summary>
    /// Unique ID of stat
    /// </summary>
    TId id { get; }
    /// <summary>
    /// Label of stat (a.k.a display name)
    /// </summary>
    string label { get; }
    /// <summary>
    /// Real value of stat
    /// </summary>
    TValue value { get; }
}