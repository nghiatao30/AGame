using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryHelper
{
    #region Extension Methods
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
    {
        if (!dictionary.ContainsKey(key))
            return defaultValue;
        return dictionary[key];
    }
    public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
        else
            dictionary[key] = value;
    }
    #endregion
}