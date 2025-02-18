using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HyrphusQ.DataStructure
{
    public class Stack<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
        private LinkedList<T> linkedList = new LinkedList<T>();

        public int Count => linkedList.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return linkedList.GetEnumerator();
        }
        public void Clear()
        {
            linkedList.Clear();
        }
        public bool Contains(T item)
        {
            return linkedList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            linkedList.CopyTo(array, arrayIndex);
        }
        public T Peek()
        {
            try
            {
                return linkedList.Last.Value;
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
                return default(T);
            }
        }
        public T Pop()
        {
            try
            {
                var item = Peek();
                linkedList.RemoveLast();
                return item;
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
                return default(T);
            }
        }
        public void Push(T item)
        {
            linkedList.AddLast(item);
        }
        public T[] ToArray()
        {
            return linkedList.ToArray();
        }
    }
}