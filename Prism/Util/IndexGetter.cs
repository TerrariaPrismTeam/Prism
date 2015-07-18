using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Util
{
    public struct IndexGetter<TKey, TValue>
    {
        IDictionary<TKey, TValue> d;

        public TValue this[TKey key]
        {
            get
            {
                return d[key];
            }
        }

        public IndexGetter(IDictionary<TKey, TValue> dict)
        {
            d = dict;
        }
    }
}
