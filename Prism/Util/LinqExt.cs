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

        public static IEnumerable<T> Flatten   <T>(this IEnumerable<IEnumerable<T>> coll)
        {
            return coll.DefaultIfEmpty(Empty<T>.Array).Aggregate((a, b) => a.SafeConcat(b));
        }
        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> coll, IEnumerable<T> other)
        {
            if (coll == null && other == null)
                return Empty<T>.Array;

            if (coll == null)
                return other;
            if (other == null)
                return coll;

            return coll.Concat(other);
        }
        public static IEnumerable<TOut> SafeSelect<TIn, TOut>(this IEnumerable<TIn> coll, Func<TIn, TOut> fn)
        {
            if (fn == null)
                throw new ArgumentNullException("fn");

            if (coll == null)
                yield break;

            foreach (var i in coll)
                yield return fn(i);

            yield break;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict)
        {
            var ret = new Dictionary<TKey, TValue>();

            foreach (var kvp in dict)
                ret.Add(kvp.Key, kvp.Value);

            return ret;
        }

        public static bool All(this IEnumerable<bool> coll)
        {
            return coll.All(MiscExtensions.Identity);
        }
        public static bool Any(this IEnumerable<bool> coll)
        {
            return coll.Any(MiscExtensions.Identity);
        }

        public static T[] Subarray<T>(this T[] arr, int s, int l)
        {
            if (s == 0 && l == arr.Length)
                return arr;

            if (s < 0 || s + l >= arr.Length)
                throw new ArgumentOutOfRangeException();

            var ret = new T[l];

            Array.Copy(arr, s, ret, 0, l);

            return ret;
        }
    }
}
