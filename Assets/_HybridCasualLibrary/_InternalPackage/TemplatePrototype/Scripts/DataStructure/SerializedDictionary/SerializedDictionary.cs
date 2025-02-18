using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Helpers;

namespace HyrphusQ.SerializedDataStructure
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        #region Constructor
        public SerializedDictionary()
        {
            m_SerializedDictionary = new Dictionary<TKey, TValue>();
        }
        public SerializedDictionary(IDictionary<TKey, TValue> collection)
        {
            m_SerializedDictionary = new Dictionary<TKey, TValue>(collection);
        }
        #endregion

        [SerializeField]
        protected List<TKey> m_Keys = new List<TKey>();
        [SerializeField]
        protected List<TValue> m_Values = new List<TValue>();

        protected Dictionary<TKey, TValue> m_SerializedDictionary = new Dictionary<TKey, TValue>();

        #region IDictionary Implementation
        public TValue this[TKey key] { get => m_SerializedDictionary[key]; set => m_SerializedDictionary[key] = value; }

        public ICollection<TKey> Keys => m_SerializedDictionary.Keys;

        public ICollection<TValue> Values => m_SerializedDictionary.Values;

        public int Count => m_SerializedDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            m_SerializedDictionary.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            m_SerializedDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            m_SerializedDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if(m_SerializedDictionary.TryGetValue(item.Key, out TValue value))
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return m_SerializedDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            try
            {
                m_SerializedDictionary.InvokeMethod<object>("CopyTo", array, arrayIndex);
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_SerializedDictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return m_SerializedDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_SerializedDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_SerializedDictionary.GetEnumerator();
        }

        public static implicit operator Dictionary<TKey, TValue>(SerializedDictionary<TKey, TValue> dictionary) => dictionary.m_SerializedDictionary;
        #endregion

        #region ISerializationCallbackReceiver Implementation
        public void OnAfterDeserialize()
        {
            m_SerializedDictionary.Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                m_SerializedDictionary.Set(m_Keys[i], m_Values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var item in m_SerializedDictionary)
            {
                m_Keys.Add(item.Key);
                m_Values.Add(item.Value);
            }
        }
        #endregion

        #region UNITY_EDITOR
        [SerializeField]
        protected TKey m_KeyTemp;
        [SerializeField]
        protected TValue m_ValueTemp;
        protected Type GetKeyType()
        {
            return typeof(TKey);
        }
        protected Type GetValueType()
        {
            return typeof(TValue);
        }
        #endregion
    }
}