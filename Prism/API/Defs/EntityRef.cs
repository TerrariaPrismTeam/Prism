using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Behaviours;
using Prism.Mods;

namespace Prism.API.Defs
{
    //TODO: (?) remove some/most/all generic constraints so this can be used for more things than only EntityDefs (and maybe rename this, too, then nuke ObjectRef?)
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

        protected ModDef Requesting
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

        protected EntityRef(int resourceId, Func<int, string> toResName)
        {
            ResourceID = resourceId;

            resName = new Lazy<string>(() => resourceId == 0 ? String.Empty : toResName(resourceId));
        }
        protected EntityRef(ObjectRef objRef, Assembly calling)
        {
            resName = new Lazy<string>(() => objRef.Name);

            ModName = objRef.ModName;

            Requesting = ModData.ModFromAssembly(calling);
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

        public static implicit operator ObjectRef(EntityRef<TEntityDef, TBehaviour, TEntity> e)
        {
            return new ObjectRef(e.ResourceName, e.Mod)
            {
                requesting = e.Requesting
            };
        }
    }
}
