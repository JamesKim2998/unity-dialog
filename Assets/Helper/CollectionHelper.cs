using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    public static class CollectionHelper
    {
        private static void LogEmpty()
        {
            Debug.LogError("empty.");
        }

        private static void LogKeyExists<T>(T key)
        {
            Debug.LogError("key exists: " + key);
        }

        public static bool Empty<T>(this T c) where T : ICollection
        {
            return c.Count == 0;
        }

        public static bool RemoveBack<T>(this List<T> c)
        {
            if (c.Count == 0)
            {
                LogEmpty();
                return false;
            }

            c.RemoveAt(c.Count - 1);
            return true;
        }

        public static bool RemoveIf<T>(this IList<T> c, Predicate<T> pred)
        {
            var i = 0;

            foreach (var data in c)
            {
                if (pred(data))
                {
                    c.RemoveAt(i);
                    return true;
                }

                ++i;
            }

            return false;
        }

        public static IEnumerable<T> GetReverseEnum<T>(this List<T> c)
        {
            for (var i = c.Count - 1; i >= 0; i--)
                yield return c[i];
        }

        public static bool TryGet<K, V>(this IDictionary<K, V> c, K key, out V val)
        {
            if (!c.TryGetValue(key, out val))
            {
                Debug.LogWarning("key " + key + " not exists.");
                return false;
            }
            return true;
        }

        public static bool TryGetAndParse<K>(this IDictionary<K, string> c, K key, out int val)
        {
            string valStr;

            if (!c.TryGet(key, out valStr))
            {
                val = 0;
                return false;
            }

            return int.TryParse(valStr, out val);
        }

        public static V GetOrDefault<K, V>(this IDictionary<K, V> c, K key, V _default = default(V))
        {
            V val;
            return c.TryGetValue(key, out val) ? val : _default;
        }

        public static void Add<K, V>(this IDictionary<K, V> c, KeyValuePair<K, V> kv)
        {
            c.Add(kv.Key, kv.Value);
        }

        public static bool TryAdd<K, V>(this IDictionary<K, V> c, K key, V val)
        {
            try
            {
                c.Add(key, val);
                return true;
            }
            catch (Exception)
            {
                LogKeyExists(key);
                return false;
            }
        }

        public static void RemoveIf<K, V>(this IDictionary<K, V> c, Func<K, V, bool> pred)
        {
            var remove = new List<K>();

            foreach (var kv in c)
            {
                if (pred(kv.Key, kv.Value))
                    remove.Add(kv.Key);
            }

            foreach (var key in remove)
            {
                c.Remove(key);
            }
        }
    }
}