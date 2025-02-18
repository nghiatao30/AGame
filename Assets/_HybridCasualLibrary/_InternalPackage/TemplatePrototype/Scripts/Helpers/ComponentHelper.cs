using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.SceneManagement.SceneManager;

namespace HyrphusQ.Helpers
{
    public static class ComponentHelper
    {
        #region Extension Methods
        public static bool TryGetComponentInParent<T>(this Component baseComponent, out T component)
        {
            component = baseComponent.GetComponentInParent<T>();
            return component != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component baseComponent, out T component)
        {
            component = baseComponent.GetComponentInChildren<T>();
            return component != null;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent(out T component))
                return component;
            return gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component baseComponent) where T : Component
        {
            if (baseComponent.TryGetComponent(out T component))
                return component;
            return baseComponent.gameObject.AddComponent<T>();
        }

        public static T GetComponentInParent<T>(this Component baseComponent, bool includeInactive)
        {
            while (baseComponent != null)
            {
                if (baseComponent.TryGetComponent(out T component))
                {
                    if (includeInactive)
                        return component;
                    else if (baseComponent.gameObject.activeSelf)
                        return component;
                }
                baseComponent = baseComponent.transform.parent;
            }
            return default(T);
        }

        public static T FindObjectOfType<T>(this Component baseComponent, bool includeInactive)
        {
            var gameObject = GetActiveScene().GetRootGameObjects().FirstOrDefault(go => go.GetComponentInChildren<T>(includeInactive) != null);
            return gameObject == null ? default(T) : gameObject.GetComponentInChildren<T>();
        }
        #endregion
    }
}