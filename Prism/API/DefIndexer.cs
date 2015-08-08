using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;

namespace Prism.API
{
    public struct DefIndexer<T> : IEnumerable<KeyValuePair<ObjectRef, T>>
    {
        Func<ObjectRef, T> byObjRef;
        Func<int, T> byId;

        IEnumerable<KeyValuePair<ObjectRef, T>> allDefs;

        public T this[int id]
        {
            get
            {
                return byId(id);
            }
        }
        public T this[ObjectRef objRef]
        {
            get
            {
                return byObjRef(objRef);
            }
        }
        public T this[string internalName, string modName = null]
        {
            get
            {
                return byObjRef(new ObjectRef(internalName, modName));
            }
        }
        public T this[string internalName, ModInfo mod]
        {
            get
            {
                return byObjRef(new ObjectRef(internalName, mod));
            }
        }

        public IEnumerable<ObjectRef> Keys
        {
            get
            {
                return allDefs.Select(kvp => kvp.Key);
            }
        }
        public IEnumerable<T> Values
        {
            get
            {
                return allDefs.Select(kvp => kvp.Value);
            }
        }

        public DefIndexer(IEnumerable<KeyValuePair<ObjectRef, T>> allDefs, Func<ObjectRef, T> byObjRef, Func<int, T> byId)
        {
            this.allDefs  = allDefs ;
            this.byObjRef = byObjRef;
            this.byId     = byId    ;
        }

        public IEnumerator<KeyValuePair<ObjectRef, T>> GetEnumerator()
        {
            return allDefs.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return allDefs.GetEnumerator();
        }
    }
}
