using System.Collections;
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
    }
}
