using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Events
{
    public class HashtableBundle : IBundle
    {
        private Hashtable m_HashTableEventData = new Hashtable();

        public void Add<T>(string key, T data)
        {
            try
            {
                if (m_HashTableEventData.ContainsKey(key))
                {
                    m_HashTableEventData[key] = data;
                    return;
                }
                m_HashTableEventData.Add(key, data);
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
            }
        }
        public void Remove(string key)
        {
            try
            {
                if (m_HashTableEventData.ContainsKey(key))
                    m_HashTableEventData.Remove(key);
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
            }
        }
        public T Get<T>(string key)
        {
            try
            {
                if(m_HashTableEventData.Contains(key))
                    return (T)m_HashTableEventData[key];
                return default(T);
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
                return default(T);
            }
        }
        public bool TryGet<T>(string key, out T value)
        {
            value = default(T);
            if (!m_HashTableEventData.Contains(key))
                return false;
            value = (T)m_HashTableEventData[key];
            return true;
        }
        public bool Contains(string key) => m_HashTableEventData.ContainsKey(key);
        public void ClearAll() => m_HashTableEventData.Clear();
    }
}