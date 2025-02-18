using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Utils
{
    public class ObjectPooling<T>
    {
        public delegate T InstantiateMethod();
        public delegate void DestroyMethod(T obj);
        public delegate void ResetState(T obj);
        public delegate void PreAddToPool(T obj);
        public delegate void PreLeavePool(T obj);

        private InstantiateMethod instantiateMethod;
        private DestroyMethod destroyMethod;
        private ResetState resetMethod;
        private PreLeavePool preLeavePool;
        private PreAddToPool preAddToPool;

        private int pregenerateOffset = 10;
        public int PregenerateOffset{
            get => pregenerateOffset;
            set {
                if(value > 0)
                    pregenerateOffset = value;
            }
        }
        private Stack<T> pool = new Stack<T>();

        public ObjectPooling(
            InstantiateMethod instantiateMethod, 
            DestroyMethod destroyMethod,
            ResetState resetMethod = null,
            PreAddToPool preAddToPool = null,
            PreLeavePool preLeavePool = null)
        {
            this.instantiateMethod = instantiateMethod;
            this.destroyMethod = destroyMethod;
            this.resetMethod = resetMethod;
            this.preAddToPool = preAddToPool;
            this.preLeavePool = preLeavePool;
        }

        public T Get(){
            if(pool.Count == 0)
                RefillPool();
            T obj = pool.Pop();
            resetMethod?.Invoke(obj);
            preLeavePool?.Invoke(obj);
            return obj;
        }

        public void Add(T obj)
        {
            preAddToPool?.Invoke(obj);
            resetMethod?.Invoke(obj);
            pool.Push(obj);
        }

        public void DestroyPool()
        {
            foreach (var obj in pool)
                destroyMethod(obj);
        }

        private void RefillPool()
        {
            for (int i = 0; i < PregenerateOffset; i++)
                Add(instantiateMethod());
        }
    }
}