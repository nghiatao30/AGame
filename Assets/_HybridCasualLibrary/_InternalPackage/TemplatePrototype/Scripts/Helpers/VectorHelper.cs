using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace HyrphusQ.Helpers
{
    public static class VectorHelper
    {
        #region Extension Methods
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
        }
        
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max, Axis axis)
        {
            Vector3 outVector = value;
            if ((axis & Axis.X) == Axis.X)
                outVector.x = Mathf.Clamp(value.x, min.x, max.x);
            if ((axis & Axis.Y) == Axis.Y)
                outVector.y = Mathf.Clamp(value.y, min.y, max.y);
            if ((axis & Axis.Z) == Axis.Z)
                outVector.z = Mathf.Clamp(value.z, min.z, max.z);
            return outVector;
        }
        #endregion
    }
}