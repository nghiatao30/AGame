using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HyrphusQ.Helpers
{
    public static class PlayerInputHelper
    {
        public static bool IsPressedGUIElement()
        {
#if UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject();
#else
        foreach (var touch in Input.touches)
        {
            var touchID = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(touchID))
                return true;
        }
        return false;
#endif
        }
    }
}