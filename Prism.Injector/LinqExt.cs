using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Injector
{
    public static class LinqExt
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> coll)
        {
            return coll.DefaultIfEmpty(new T[0]).Aggregate((a, b) => a.SafeConcat(b));
        }
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
