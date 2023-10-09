using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Tools
{
    public static class Utilities
    {
        public static List<T> ShallowCopy<T>(List<T> list)
        {
            List<T> result = new List<T>();

            foreach (T element in list)
            {
                result.Add(element);
            }

            return result;
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> result = new List<T>();

            List<T> copy = ShallowCopy(list);

            int n = copy.Count;
            while (n > 0)
            {
                int idx = Random.Range(0, n);
                result.Add(copy[idx]);
                copy.RemoveAt(idx);
                n--;
            }

            return result;
        }
    }
}
