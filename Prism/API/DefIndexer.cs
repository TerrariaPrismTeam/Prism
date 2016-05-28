using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods;
using Prism.Util;

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
    public class DIHelper<T>
    {
        protected int maxIdValue;
        protected string objName;

        protected Func<ModDef, IDictionary<string, T>> GetModDefs;
        protected IDictionary<string, T> VanillaDefsByName;
        protected IDictionary<int, T> VanillaDefsById;

        public DIHelper(int maxIdValue, string objName, Func<ModDef, IDictionary<string, T>> getModDefs, IDictionary<string, T> vByName, IDictionary<int, T> vById)
        {
            this.maxIdValue = maxIdValue;
            this.objName = objName;

            GetModDefs = getModDefs;
            VanillaDefsByName = vByName;
            VanillaDefsById = vById;
        }

        public T ByObjRef(ObjectRef or, ModDef requesting)
        {
            var req = requesting ?? or.requesting;
            T ret;

            if (String.IsNullOrEmpty(or.ModName) && req != null && GetModDefs(req).TryGetValue(or.Name, out ret))
                return ret;

            if (or.Mod == PrismApi.VanillaInfo)
            {
                if (VanillaDefsByName.TryGetValue(or.Name, out ret))
                    return ret;

                throw new InvalidOperationException("Vanilla " + objName + " definition '" + or.Name + "' is not found.");
            }

            ModDef md;

            if (!ModData.ModsFromInternalName.TryGetValue(or.ModName, out md))
                throw new InvalidOperationException(objName + " definition '" + or.Name + "' in mod '" + or.ModName + "' could not be returned because the mod is not loaded.");

            if (GetModDefs(md).TryGetValue(or.Name, out ret))
                return ret;

            throw new InvalidOperationException(objName + " definition '" + or.Name + "' in mod '" + or.ModName + "' could not be resolved because the " + objName + " is not loaded.");
        }
        public T ById(int id)
        {
            if (id >= maxIdValue || !VanillaDefsById.ContainsKey(id))
                throw new ArgumentOutOfRangeException("id", "The id must be a vanilla " + objName + " id.");

            return VanillaDefsById[id];
        }
    }
    public sealed class EntityDIH<TEntity, TBehaviour, TEntityDef> : DIHelper<TEntityDef>
        where TEntity : class
        where TBehaviour : EntityBehaviour<TEntity>
        where TEntityDef : EntityDef<TBehaviour, TEntity>
    {
        public EntityDIH(int maxIdValue, string objName, Func<ModDef, IDictionary<string, TEntityDef>> getModDefs, IDictionary<string, TEntityDef> vByName, IDictionary<int, TEntityDef> vById)
            : base(maxIdValue, objName, getModDefs, vByName, vById)
        {

        }

        public IEnumerable<KeyValuePair<ObjectRef, TEntityDef>> GetEnumerable()
        {
            // welcome to VERY GODDAMN VERBOSE functional programming
            // seriously, type inferrence FTW
            var vanillaDefs = VanillaDefsByName.Values.Select(id => new KeyValuePair<ObjectRef, TEntityDef>(new ObjectRef(id.InternalName), id));
            var modDefs = ModData.mods.Select(GetModDefsInUsefulFormat).Flatten();

            return vanillaDefs.Concat(modDefs);
        }
        IEnumerable<KeyValuePair<ObjectRef, TEntityDef>> GetModDefsInUsefulFormat(KeyValuePair<ModInfo, ModDef> kvp)
        {
            return GetModDefs(kvp.Value).SafeSelect(kvp_ => new KeyValuePair<ObjectRef, TEntityDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
        }
    }
}
