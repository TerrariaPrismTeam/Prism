using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Prism.Util
{
    /// <summary>
    /// A <see cref="Dictionary{TKey, TValue}"/> that works in forward and reverse ways.
    /// </summary>
    /// <typeparam name="TForward">The forward type, TKey for the forward dictionary and TValue for the reversed dictionary.</typeparam>
    /// <typeparam name="TReverse">The reverse type, TValue for the forward dictionary and TKey for the reversed dictionary.</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}")]
    public class BiDictionary<TForward, TReverse> : IDictionary<TForward, TReverse>,
        ICollection<KeyValuePair<TForward, TReverse>>, IEnumerable<KeyValuePair<TForward, TReverse>>
    {
        /// <summary>
        /// Index helper for a <see cref="BiDictionary{TForward, TReverse}"/>.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        public class Indexer<TKey, TValue>
        {
            Dictionary<TKey, TValue> dict;

            internal Indexer(Dictionary<TKey, TValue> dictionary)
            {
                dict = dictionary;
            }

            /// <summary>
            /// Gets or sets the value of a <see cref="BiDictionary{TForward, TReverse}"/> with a specified key
            /// </summary>
            /// <param name="key">The key of the Key/Value pair</param>
            /// <returns>The value of the Key/Value pair</returns>
            public TValue this[TKey key]
            {
                get
                {
                    return dict[key];
                }
                set
                {
                    dict[key] = value;
                }
            }
        }

        Dictionary<TForward, TReverse> fw;
        Dictionary<TReverse, TForward> rev;

        /// <summary>
        /// The forward accessor
        /// </summary>
        public Indexer<TForward, TReverse> Forward
        {
            get;
            private set;
        }
        /// <summary>
        /// The reverse accossor
        /// </summary>
        public Indexer<TReverse, TForward> Reverse
        {
            get;
            private set;
        }
        /// <summary>
        /// All keys of the current instance
        /// </summary>
        public ICollection<TForward> Keys
        {
            get
            {
                return fw.Keys;
            }
        }
        /// <summary>
        /// All values of the current instance
        /// </summary>
        public ICollection<TReverse> Values
        {
            get
            {
                return fw.Values;
            }
        }
        /// <summary>
        /// Gets the <typeparamref name="TReverse"/> with the specified <typeparamref name="TForward"/>
        /// </summary>
        /// <param name="tfw">The <typeparamref name="TForward"/> used to retrieve the <typeparamref name="TReverse"/></param>
        /// <returns>The <typeparamref name="TReverse"/> associated with the specified <typeparamref name="TForward"/></returns>
        public TReverse this[TForward tfw]
        {
            get
            {
                return Forward[tfw];
            }
            set
            {
                Forward[tfw] = value;
            }
        }
        /// <summary>
        /// Gets the Key/Value pair at the specified index
        /// </summary>
        /// <param name="index">The index of the Key/Value pair</param>
        /// <returns>The Key/Value pair at the specified index</returns>
        public KeyValuePair<TForward, TReverse> this[int index]
        {
            get
            {
                return new KeyValuePair<TForward, TReverse>(Keys.ElementAt(index), Values.ElementAt(index));
            }
        }

        /// <summary>
        /// Tries to get a <typeparamref name="TReverse"/> value from the specified <typeparamref name="TForward"/> value
        /// </summary>
        /// <param name="tfw">The specified forward key</param>
        /// <param name="trev">If the key exists, this will be the matching <typeparamref name="TReverse"/></param>
        /// <returns>True whether the key exist (and the value is return by trev parameter) or not.</returns>
        public bool TryGetValue(TForward tfw, out TReverse trev)
        {
            return fw.TryGetValue(tfw, out trev);
        }

        /// <summary>
        /// The amount of Key/Value pairs in the <see cref="BiDictionary{TForward, TReverse}"/>
        /// </summary>
        public int Count
        {
            get
            {
                return fw.Count;
            }
        }
        /// <summary>
        /// whether the <see cref="BiDictionary{TForward, TReverse}"/> is read-only or not. This will always return false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BiDictionary{TForward, TReverse}"/> class.
        /// </summary>
        public BiDictionary()
            : this(new Dictionary<TForward, TReverse>(), new Dictionary<TReverse, TForward>())
        {

        }
        BiDictionary(Dictionary<TForward, TReverse> forward, Dictionary<TReverse, TForward> reverse)
        {
            fw = forward;
            rev = reverse;
            Forward = new Indexer<TForward, TReverse>(fw);
            Reverse = new Indexer<TReverse, TForward>(rev);
        }

        /// <summary>
        /// Adds a Key/Value pair to the collection
        /// </summary>
        /// <param name="tfw">The forward key to add</param>
        /// <param name="trev">The forward value (reverse key) to add</param>
        public void Add(TForward tfw, TReverse trev)
        {
            fw.Add(tfw, trev);
            rev.Add(trev, tfw);
        }
        /// <summary>
        /// Adds a Key/Value pair to the collection as a <see cref="KeyValuePair{TKey, TValue}"/>
        /// </summary>
        /// <param name="kvp">The Key/Value pair to add</param>
        public void Add(KeyValuePair<TForward, TReverse> kvp)
        {
            fw.Add(kvp.Key, kvp.Value);
            rev.Add(kvp.Value, kvp.Key);
        }

        /// <summary>
        /// Clears the collection of Key/Value pairs
        /// </summary>
        public void Clear()
        {
            fw.Clear();
            rev.Clear();
        }

        /// <summary>
        /// whether the Key/Value pair collection contains a specified <typeparamref name="TForward"/> as forward key
        /// </summary>
        /// <param name="tfw">The forward key to check</param>
        /// <returns>whether the forward key exists in the collection or not</returns>
        public bool ContainsKey(TForward tfw)
        {
            return fw.ContainsKey(tfw);
        }
        /// <summary>
        /// whether the Key/Value pair collection contains a specified <typeparamref name="TReverse"/> as forward value
        /// </summary>
        /// <param name="trev">The forward value to check</param>
        /// <returns>whether the forward value exists in the collection or not</returns>
        public bool ContainsValue(TReverse trev)
        {
            return fw.ContainsValue(trev);
        }
        /// <summary>
        /// whether the forward collection contains a specified <typeparamref name="TForward"/> as key
        /// </summary>
        /// <param name="tfw">The forward key to check</param>
        /// <returns>whether the forward collection contains the specified <typeparamref name="TForward"/> or not</returns>
        public bool ForwardContainsKey(TForward tfw)
        {
            return fw.ContainsKey(tfw);
        }
        /// <summary>
        /// whether the reverse collection contains a specified <typeparamref name="TReverse"/> as key
        /// </summary>
        /// <param name="trev">The reverse key to check</param>
        /// <returns>whether the reverse collection contains the specified <typeparamref name="TReverse"/> or not</returns>
        public bool ReverseContainsKey(TReverse trev)
        {
            return rev.ContainsKey(trev);
        }
        /// <summary>
        /// Checks whether the Key/Value pair collection contains the specified <see cref="KeyValuePair{TKey, TValue}"/>
        /// </summary>
        /// <param name="kvp">The Key/Value pair to check</param>
        /// <returns>whether the collection contains the specified Key/Value pair or not</returns>
        public bool Contains(KeyValuePair<TForward, TReverse> kvp)
        {
            return fw.ContainsKey(kvp.Key) && rev.ContainsKey(kvp.Value);
        }

        /// <summary>
        /// Removes the Key/Value pair specified with the given <typeparamref name="TForward"/> as forward key
        /// </summary>
        /// <param name="tfw">The <typeparamref name="TForward"/> used as forward key</param>
        /// <returns>whether the items have been removed or not</returns>
        public bool Remove(TForward tfw)
        {
            return rev.Remove(fw[tfw]) && fw.Remove(tfw);
        }
        /// <summary>
        /// Removes the Key/Value pair specified with the given <typeparamref name="TReverse"/> as reverse key
        /// </summary>
        /// <param name="trev">The <typeparamref name="TReverse"/> used as reverse key</param>
        /// <returns>whether the items have been removed or not</returns>
        public bool Remove(TReverse trev)
        {
            return fw.Remove(rev[trev]) && rev.Remove(trev);
        }
        /// <summary>
        /// Removes the Key/Value pair specified with the given <see cref="KeyValuePair{TKey, TValue}"/>
        /// </summary>
        /// <param name="kvp">The Key/Value pair to remove</param>
        /// <returns>whether the Key/Value pair has been removed or not</returns>
        public bool Remove(KeyValuePair<TForward, TReverse> kvp)
        {
            return Remove(kvp.Key);
        }

        /// <summary>
        /// Copies the colleciton data to an array of Key/Value pairs
        /// </summary>
        /// <param name="array">The array to copy the data to</param>
        /// <param name="startIndex">The starting index of the copying; default is 0</param>
        public void CopyTo(KeyValuePair<TForward, TReverse>[] array, int startIndex = 0)
        {
            for (int i = startIndex; i < startIndex + Count; i++)
                if (array.Length <= i)
                {
                    List<KeyValuePair<TForward, TReverse>> l = array.ToList();
                    l.Add(this[i]);
                    array = l.ToArray();
                }
                else
                    array[i] = this[i];
        }

        /// <summary>
        /// Gets the generic Enumerator of the Key/Value pair collection
        /// </summary>
        /// <returns>The generic Enumerator of the Key/Value pair collection</returns>
        IEnumerator<KeyValuePair<TForward, TReverse>> IEnumerable<KeyValuePair<TForward, TReverse>>.GetEnumerator()
        {
            return fw.GetEnumerator();
        }
        /// <summary>
        /// Gets the forward Enumerator of the Key/Value pair collection
        /// </summary>
        /// <returns>The forward Enumerator of the Key/Value pair collection</returns>
        public IEnumerator<KeyValuePair<TForward, TReverse>> GetForwardEnumerator()
        {
            return fw.GetEnumerator();
        }
        /// <summary>
        /// Gets the reverse Enumerator of the Key/Value pair collection
        /// </summary>
        /// <returns>The reverse Enumerator of the Key/Value pair collection</returns>
        public IEnumerator<KeyValuePair<TReverse, TForward>> GeReversetEnumerator()
        {
            return rev.GetEnumerator();
        }
        /// <summary>
        /// Gets the Enumerator of the Key/Value pair collection
        /// </summary>
        /// <returns>The Enumerator of the Key/Value pair collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return fw.GetEnumerator();
        }

        /// <summary>
        /// Gets the current <see cref="BiDictionary{TKey, TValue}"/> in its reversed state
        /// </summary>
        public BiDictionary<TReverse, TForward> Reversed
        {
            get
            {
                return new BiDictionary<TReverse, TForward>(rev, fw);
            }
        }
    }
}
