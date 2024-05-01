using System.Collections.Generic;
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

        public static Vector3 ClosestTo(this List<Vector3> vectors, Vector3 target)
        {
            Vector3 closest = vectors[0];
            float closestDistance = Vector3.Distance(vectors[0], target);

            for (int i = 1; i < vectors.Count; i++)
            {
                float distance = Vector3.Distance(vectors[i], target);

                if (distance < closestDistance)
                {
                    closest = vectors[i];
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}
