using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class TransformHelper
    {
        #region Extension Methods
        public static void ResetTransform(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void CloneTransform(this Transform transform, Transform target, TransformConstraint constraint = TransformConstraint.None, Space space = Space.World)
        {
            if (space == Space.World)
            {
                // Bitwise apply
                if ((constraint & TransformConstraint.Position) == TransformConstraint.Position)
                    transform.position = target.position;
                if ((constraint & TransformConstraint.Rotation) == TransformConstraint.Rotation)
                    transform.rotation = target.rotation;
            }
            else
            {
                // Bitwise apply
                if ((constraint & TransformConstraint.Position) == TransformConstraint.Position)
                    transform.localPosition = target.localPosition;
                if ((constraint & TransformConstraint.Rotation) == TransformConstraint.Rotation)
                    transform.localRotation = target.localRotation;
            }
            if ((constraint & TransformConstraint.Scale) == TransformConstraint.Scale)
                transform.localScale = target.localScale;
        }
        
        public static void CloneRectTransform(this RectTransform transform, RectTransform target)
        {
            transform.pivot = target.pivot;
            transform.sizeDelta = target.sizeDelta;
            transform.anchorMin = target.anchorMin;
            transform.anchorMax = target.anchorMax;
            transform.offsetMin = target.offsetMin;
            transform.offsetMax = target.offsetMax;
            transform.anchoredPosition3D = target.anchoredPosition3D;
        }
        #endregion
    }
}