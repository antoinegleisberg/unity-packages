using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Tools
{
    public static class ListUtilities
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

        public static List<T> Shuffle<T>(List<T> list)
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

        public static bool ContainsDuplicates<T>(List<T> list)
        {
            HashSet<T> set = new HashSet<T>();
            foreach (T element in list)
            {
                if (set.Contains(element))
                {
                    return true;
                }
                set.Add(element);
            }
            return false;
        }
    }
}
