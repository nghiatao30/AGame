using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class FloatHelper
    {
        #region Extension Methods
        public static bool IsValidRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static float Ceil(this float value, int digits)
        {
            var m = Mathf.Pow(10, digits);
            value *= m;
            value = Mathf.Ceil(value);
            return value / m;
        }

        public static float Floor(this float value, int digits)
        {
            var m = Mathf.Pow(10, digits);
            value *= m;
            value = Mathf.Floor(value);
            return value / m;
        }

        public static float Step(this float value, float threshold)
        {
            return value >= threshold ? 1f : 0f;
        }
        #endregion
    }
}
