using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace antoinegleisberg.Types
{
    public static class MethodExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyList<Pair<TKey, TValue>> pairs)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            
            foreach (Pair<TKey, TValue> pair in pairs)
            {
                dict.Add(pair.First, pair.Second);
            }

            return dict;
        }

        public static Dictionary<TKey, int> Merge<TKey>(this IReadOnlyDictionary<TKey, int> dict1, IReadOnlyDictionary<TKey, int> dict2)
        {
            Dictionary<TKey, int> dict = new Dictionary<TKey, int>(dict1);

            foreach (KeyValuePair<TKey, int> kvp in dict2)
            {
                if (dict.ContainsKey(kvp.Key))
                {
                    dict[kvp.Key] = dict[kvp.Key] + kvp.Value;
                }
                else
                {
                    dict.Add(kvp.Key, kvp.Value);
                }
            }

            return dict;
        }

        /// <summary>
        /// Returns a dictionary containing the difference between the two dictionaries.
        /// The returned dictionary contains the keys that are in dict1, witht the value being the difference between the value in dict1 and the value in dict2.
        /// All values in both dict1 and dict2 are assumed to be positive or 0
        /// In other words, dict1.Diff(dict2)[key] = dict1[key] - dict2.get(key, 0)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static Dictionary<TKey, int> Diff<TKey>(this IReadOnlyDictionary<TKey, int> dict1, IReadOnlyDictionary<TKey, int> dict2)
        {
            Dictionary<TKey, int> result = new Dictionary<TKey, int>();
            foreach (TKey key in dict1.Keys)
            {
                int value = dict1[key] - dict2.GetValueOrDefault(key, 0);
                if (value > 0)
                {
                    result[key] = value;
                }
            }
            return result;
        }

        public static Vector3 ClosestTo(this IEnumerable<Vector3> vectors, Vector3 target)
        {
            if (vectors.Count() == 0)
            {
                throw new ArgumentException("Cannot find the closest vector to a target in an empty list of vectors");
            }

            Vector3 closest = vectors.First();
            float closestDistance = Vector3.Distance(closest, target);

            foreach (Vector3 vector in vectors)
            {
                float distance = Vector3.Distance(vector, target);

                if (distance < closestDistance)
                {
                    closest = vector;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;
            TSource maxItem = default(TSource);
            TKey maxValue = default(TKey);
            bool hasValue = false;

            foreach (var item in source)
            {
                TKey value = selector(item);
                if (!hasValue || comparer.Compare(value, maxValue) > 0)
                {
                    maxItem = item;
                    maxValue = value;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return maxItem;
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;
            TSource minItem = default(TSource);
            TKey minValue = default(TKey);
            bool hasValue = false;

            foreach (var item in source)
            {
                TKey value = selector(item);
                if (!hasValue || comparer.Compare(value, minValue) < 0)
                {
                    minItem = item;
                    minValue = value;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return minItem;
        }

        public static Vector3Int ToVector3Int(this Vector2Int vector)
        {
            return new Vector3Int(vector.x, vector.y, 0);
        }

        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
