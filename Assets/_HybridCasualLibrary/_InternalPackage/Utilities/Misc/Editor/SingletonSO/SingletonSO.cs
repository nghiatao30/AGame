using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace UnityEditor
{
    /// <summary>
    /// This class is created to work in Editor only
    /// </summary>
    public abstract class SingletonSO<T> : SerializedScriptableObject where T : SerializedScriptableObject
    {
        protected static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = EditorUtils.FindAssetOfType<T>();
                return s_Instance;
            }
        }
    }
}