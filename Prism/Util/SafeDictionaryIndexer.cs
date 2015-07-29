using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.Util
{
    public struct SafeDictionaryIndexer<TKey, TValue>
    {
        public Dictionary<TKey, TValue> Dictionary
        {
            get;
            private set;
        }

        public readonly Func<Dictionary<TKey, TValue>> FuncDefaults;

        public readonly Func<TKey, TValue> FuncMissingEntry;

        public bool HasBeenFirstAccessed
        {
            get;
            private set;
        }

        public SafeDictionaryIndexer(Func<Dictionary<TKey, TValue>> funcDef, bool immediateDefaults = true, Func<TKey, TValue> funcMissingEntry = null)
        {
            Dictionary = new Dictionary<TKey, TValue>();
            FuncDefaults = funcDef;
            FuncMissingEntry = funcMissingEntry ?? ((TKey k) => { return default(TValue); });
            HasBeenFirstAccessed = false;

            if (immediateDefaults)
            {
                Dictionary = FuncDefaults();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                CheckAccess();
                return Dictionary.ContainsKey(key) ? Dictionary[key] : FuncMissingEntry(key);
            }
        }

        private void CheckAccess()
        {
            if (!HasBeenFirstAccessed)
            {
                Dictionary = FuncDefaults();                
            }

            HasBeenFirstAccessed = true;
        }
    }
}
