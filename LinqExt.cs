using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// one of these is defined for Prism.csproj
// otherwise, it must be Prism.Injector.csproj
#if WINDOWS || LINUX || OSX
namespace Prism.Util
#else
namespace Prism.Injector
#endif
{
    public static partial class Empty<T>
    {
        public static T[] Array = new T[0];
    }
    public static partial class MiscExtensions
    {
        public static T Identity<T>(T x)
        {
            return x;
        }
    }
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
            if (coll.IsEmpty())
                return Empty<T>.Array;
            return coll.Aggregate((a, b) => a.SafeConcat(b));
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

        public static void Resize2D<T>(ref T[,] array, int x, int y)
        {
            int ox = array.GetLength(0), oy = array.GetLength(1);

            var orig = array;
            array = new T[x, y];

            int mx = Math.Min(ox, x), my = Math.Min(oy, y);

            for (int i = 0; i < my; i++)
                Array.Copy(orig, i * ox, array, i * x, mx);
        }

        public static void Add<T>(ref T[] array, T toAdd)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = toAdd;
        }
        public static void AddArray<T>(ref T[] array, T[] toAdd)
        {
            var ol = array.Length;
            Array.Resize(ref array, array.Length + toAdd.Length);

            for (int i = 0; i < toAdd.Length; i++)
                array[i + ol] = toAdd[i];
        }

        public static IEnumerable<TOut> SelectIndex<TIn, TOut>(this TIn[] arr, Func<int, TIn, TOut> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            for (int i = 0; i < arr.Length; i++)
                yield return selector(i, arr[i]);

            yield break;
        }
        public static IEnumerable<TOut> SelectIndex<TIn, TOut>(this IList<TIn> list, Func<int, TIn, TOut> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            for (int i = 0; i < list.Count; i++)
                yield return selector(i, list[i]);

            yield break;
        }
        public static IEnumerable<TOut> SelectIndex<TIn, TOut>(this IEnumerable<TIn> coll, Func<int, TIn, TOut> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            int i = 0;
            foreach (var e in coll)
            {
                yield return selector(i, e);

                i++;
            }

            yield break;
        }

        static bool Equality<T>(T a, T b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) ||ReferenceEquals(b, null))
                return false;

            if (a is IEquatable<T>)
                return ((IEquatable<T>)a).Equals(b);
            if (b is IEquatable<T>)
                return ((IEquatable<T>)b).Equals(a);

            return a.Equals(b);
        }
        [DebuggerStepThrough]
        public static bool Equals<T>(this IList<T> a, IList<T> b)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
                if (!Equality(a[i], b[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Gets whether an <see cref="IEnumerable{T}"/> is in chronological order by supplying the adjacent items for each item.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="coll">This collection.</param>
        /// <param name="getPrevious">
        /// Passes an item in the collection and returns either the previous item or null to ignore the check.
        /// <para/>Note: pass null for the function itself to skip it. However, if both the getPrevious and getNext functions are null, an <see cref="ArgumentException"/> will be thrown.
        /// </param>
        /// <param name="getNext">
        /// Passes an item in the collection and returns either the next item or null to ignore the check.
        /// <para/>Note: pass null for the function itself to skip it. However, if both the getPrevious and getNext functions are null, an <see cref="ArgumentException"/> will be thrown.
        /// </param>
        /// <returns>Whether the collection is chronological.</returns>
        [DebuggerStepThrough]
        public static bool Chronological<T>(this IEnumerable<T> coll, Func<T, T> getPrevious, Func<T, T> getNext)
        {
            var arr = coll.ToArray();
            if (getPrevious == null && getNext == null)
                throw new ArgumentException("At least one of the two adjacent item functions must be defined.");
            for (int i = 0; i < arr.Length; i++)
            {
                var prev = getPrevious == null || i == 0 || ReferenceEquals(getPrevious(arr[i]), null) || getPrevious(arr[i]).Equals(arr[i - 1]);
                var next = getNext == null || i == arr.Length - 1 || ReferenceEquals(getNext(arr[i]), null) || getNext(arr[i]).Equals(arr[i + 1]);

                if (!(prev && next))
                    return false;
            }
            return true;
        }
    }
}
