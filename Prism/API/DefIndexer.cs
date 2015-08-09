using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Behaviours;
using Prism.Mods;

namespace Prism.API
{
    public struct DefIndexer<T> : IEnumerable<KeyValuePair<ObjectRef, T>>
    {
        Func<ObjectRef, ModDef, T> byObjRef;
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
                return byObjRef(objRef, ModData.ModFromAssembly(Assembly.GetCallingAssembly()));
            }
        }
        public T this[string internalName, string modName = null]
        {
            get
            {
                return byObjRef(new ObjectRef(internalName, modName), ModData.ModFromAssembly(Assembly.GetCallingAssembly()));
            }
        }
        public T this[string internalName, ModInfo mod]
        {
            get
            {
                return byObjRef(new ObjectRef(internalName, mod), ModData.ModFromAssembly(Assembly.GetCallingAssembly()));
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

        public DefIndexer(IEnumerable<KeyValuePair<ObjectRef, T>> allDefs, Func<ObjectRef, ModDef, T> byObjRef, Func<int, T> byId)
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
