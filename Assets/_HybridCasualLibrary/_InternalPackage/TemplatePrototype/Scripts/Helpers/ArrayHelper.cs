using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class ArrayHelper
    {
        #region Extension Methods
        public static T[,] FillAll<T>(this T[,] array, T value)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = value;
                }
            }
            return array;
        }

        public static T[] FillAll<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
            return array;
        }

        public static List<T> FillAll<T>(this List<T> list, T value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = value;
            }
            return list;
        }

        public static void Swap<T>(this List<T> list, int currentIndex, int newIndex)
        {
            var currentValue = list[currentIndex];
            list[currentIndex] = list[newIndex];
            list[newIndex] = currentValue;
        }

        public static void Swap<T>(this T[] array, int currentIndex, int newIndex)
        {
            var currentValue = array[currentIndex];
            array[currentIndex] = array[newIndex];
            array[newIndex] = currentValue;
        }

        public static bool IsValidIndex<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static bool IsValidIndex<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list == null || list.Count <= 0)
                return default(T);
            return list[Random.Range(0, list.Count)];
        }

        public static T GetRandom<T>(this T[] array)
        {
            if (array == null || array.Length <= 0)
                return default(T);
            return array[Random.Range(0, array.Length)];
        }
        #endregion
    }
}