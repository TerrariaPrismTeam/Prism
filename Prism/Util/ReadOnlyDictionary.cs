using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Util
{
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        IDictionary<TKey, TValue> inner;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dict)
        {
            inner = dict;
        }

        public TValue this[TKey key]
        {
            get
            {
                return inner[key];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return inner.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return inner.Values;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        public void Add(TKey key, TValue value)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return inner.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return inner.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(TKey key)
        {
            throw new InvalidOperationException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return inner.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }
}
