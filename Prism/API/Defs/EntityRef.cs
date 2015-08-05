using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;

namespace Prism.API.Defs
{
    public abstract class EntityRef<TEntityDef, TBehaviour, TEntity> : IEquatable<EntityRef<TEntityDef, TBehaviour, TEntity>>
        where TEntity : class
        where TBehaviour : EntityBehaviour<TEntity>
        where TEntityDef : EntityDef<TBehaviour, TEntity>
    {
        Lazy<string> resName;

        public int? ResourceID
        {
            get;
            private set;
        }
        public string ResourceName
        {
            get
            {
                return resName.Value;
            }
        }
        public string ModName
        {
            get;
            private set;
        }

        public bool IsVanillaRef
        {
            get
            {
                return String.IsNullOrEmpty(ModName) || ModName == PrismApi.VanillaString || ModName == PrismApi.TerrariaString;
            }
        }

        public ModInfo Mod
        {
            get
            {
                return IsVanillaRef ? PrismApi.VanillaInfo : ModData.mods.Keys.FirstOrDefault(mi => mi.InternalName == ModName);
            }
        }

        public EntityRef(int resourceId, Func<int, string> toResName)
        {
            ResourceID = resourceId;

            resName = new Lazy<string>(() => resourceId == 0 ? String.Empty : toResName(resourceId));
        }
        public EntityRef(string resourceName, string modName = null)
        {
            resName = new Lazy<string>(() => resourceName);
            // resName /* force */ .Value /* make it compile */.ElementAt(0);

            ModName = String.IsNullOrEmpty(resourceName) || modName == PrismApi.VanillaString || modName == PrismApi.TerrariaString ? null : modName;
        }

        public abstract TEntityDef Resolve();

        public bool Equals(EntityRef<TEntityDef, TBehaviour, TEntity> other)
        {
            return ResourceName == other.ResourceName && Mod == other.Mod;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is EntityRef<TEntityDef, TBehaviour, TEntity>)
                return Equals((EntityRef<TEntityDef, TBehaviour, TEntity>)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return ResourceName.GetHashCode() + Mod.GetHashCode();
        }
        public override string ToString()
        {
            return (ResourceID.HasValue ? ("#" + ResourceID.Value + " ") : String.Empty) + (String.IsNullOrEmpty(ResourceName) ? "<empty>" : ("{" + Mod.InternalName + "." + ResourceName + "}"));
        }
    }
}
