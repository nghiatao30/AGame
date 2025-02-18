using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class IntHelper
    {
        #region Extension Methods
        public static bool IsValidRange(this int index, int min, int max)
        {
            return index >= min && index <= max;
        }
        #endregion
    }
}