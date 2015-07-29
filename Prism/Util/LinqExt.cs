using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Util
{
    public static class LinqExt
    {
        public static bool IsEmpty    <T>(this IEnumerable<T> coll)
        {
            foreach (var t in coll)
                return false;

            return true;
        }
        public static bool IsSingleton<T>(this IEnumerable<T> coll)
        {
            bool foundFirst = false;

            foreach (var t in coll)
            {
                if (foundFirst)
                    return false;

                foundFirst = true;
            }

            return foundFirst;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict)
        {
            var ret = new Dictionary<TKey, TValue>(dict.Count());

            foreach (var kvp in dict)
                ret.Add(kvp.Key, kvp.Value);

            return ret;
        }
    }
}
