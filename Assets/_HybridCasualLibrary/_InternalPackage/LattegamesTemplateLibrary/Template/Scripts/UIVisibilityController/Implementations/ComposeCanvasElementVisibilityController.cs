using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LatteGames
{
    public class ComposeCanvasElementVisibilityController : MonoBehaviour, IUIVisibilityController
    {
        public virtual SubscriptionEvent GetOnEndHideEvent()
        {
            return uiVisibilityControllers[0].GetOnEndHideEvent();
        }

        public virtual SubscriptionEvent GetOnEndShowEvent()
        {
            return uiVisibilityControllers[0].GetOnEndShowEvent();
        }

        public virtual SubscriptionEvent GetOnStartHideEvent()
        {
            return uiVisibilityControllers[0].GetOnStartHideEvent();
        }

        public virtual SubscriptionEvent GetOnStartShowEvent()
        {
            return uiVisibilityControllers[0].GetOnStartShowEvent();
        }

        [Header("Require IUIVisibilityController component")]
        [SerializeField]
        protected List<GameObject> visibilityControllers = new List<GameObject>();
        protected List<IUIVisibilityController> _UIVisibilityControllers;
        protected List<IUIVisibilityController> uiVisibilityControllers
        {
            get
            {
                if (_UIVisibilityControllers == null)
                {
                    _UIVisibilityControllers = new List<IUIVisibilityController>();
                    foreach (var item in visibilityControllers)
                    {
                        _UIVisibilityControllers.AddRange(item.GetComponents<IUIVisibilityController>().Where(item => (item as UnityEngine.Object) != this));
                    }
                }
                return _UIVisibilityControllers;
            }
        }

        public virtual void Hide()
        {
            foreach (var item in uiVisibilityControllers)
            {
                var unityObj = item as UnityEngine.Object;
                if (unityObj != null)
                    item?.Hide();
            }
        }

        public virtual void HideImmediately()
        {
            foreach (var item in uiVisibilityControllers)
            {
                var unityObj = item as UnityEngine.Object;
                if (unityObj != null)
                    item.HideImmediately();
            }
        }

        public virtual void Show()
        {
            foreach (var item in uiVisibilityControllers)
            {
                var unityObj = item as UnityEngine.Object;
                if (unityObj != null)
                    item.Show();
            }
        }

        public virtual void ShowImmediately()
        {
            foreach (var item in uiVisibilityControllers)
            {
                var unityObj = item as UnityEngine.Object;
                if (unityObj != null)
                    item.ShowImmediately();
            }
        }

        protected virtual void OnValidate()
        {
            List<GameObject> rejectList = new List<GameObject>();
            foreach (var item in visibilityControllers)
            {
                var controller = item.GetComponent<IUIVisibilityController>();
                if (controller == null)
                    rejectList.Add(item);
            }
            foreach (var item in rejectList)
            {
                visibilityControllers.Remove(item);
            }
        }
    }
}