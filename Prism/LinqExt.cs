using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism
{
    public static class LinqExt
    {
        public static bool IsEmpty<T>(this IEnumerable<T> coll)
        {
            foreach (var t in coll)
                return false;

            return true;
        }
    }
}
