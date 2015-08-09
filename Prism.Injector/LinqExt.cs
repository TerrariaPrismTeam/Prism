using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Prism.Injector
{
    public static class LinqExt
    {
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
                var prev = getPrevious == null || i > 0              || getPrevious(arr[i]) == null || ((object)getPrevious(arr[i])).Equals(arr[i - 1]);
                var next = getNext     == null || i < arr.Length - 1 || getNext    (arr[i]) == null || ((object)getNext    (arr[i])).Equals(arr[i + 1]);

                if (!(prev && next))
                    return false;
            }
            return true;
        }
        [DebuggerStepThrough]
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> coll)
        {
            return coll.DefaultIfEmpty(new T[0]).Aggregate((a, b) => a.SafeConcat(b));
        }
        [DebuggerStepThrough]
        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> coll, IEnumerable<T> other)
        {
            if (coll == null && other == null)
                return new T[0];

            if (coll  == null) return other;
            if (other == null) return coll ;

            return coll.Concat(other);
        }
    }
}
