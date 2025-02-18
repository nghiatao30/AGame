using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract base class representing a collection variable with generic type support.
/// </summary>
/// <typeparam name="GenericCollection">The generic type of the collection to be used.</typeparam>
/// <typeparam name="T">The type of elements within the collection.</typeparam>
public abstract class CollectionVariable<GenericCollection, T> : Variable<GenericCollection> where GenericCollection : ICollection<T>
{
    /// <summary>
    /// Event triggered when an item is added to the collection.
    /// </summary>
    public abstract event Action<T> onItemAdded;
    /// <summary>
    /// Event triggered when an item is removed from the collection.
    /// </summary>
    public abstract event Action<T> onItemRemoved;
    /// <summary>
    /// Event triggered when all items are cleared from the collection.
    /// </summary>
    public abstract event Action onItemCleared;

    /// <summary>
    /// Gets the count of elements within the collection.
    /// </summary>
    public abstract int count { get; }
}