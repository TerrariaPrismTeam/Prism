using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;

namespace Prism.API.Defs
{
    public abstract class EntityRef<T> : IEquatable<EntityRef<T>>
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

        public abstract T Resolve();

        public bool Equals(EntityRef<T> other)
        {
            return ResourceName == other.ResourceName && Mod == other.Mod;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is EntityRef<T>)
                return Equals((EntityRef<T>)obj);

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

        public static implicit operator ObjectRef(EntityRef<T> e)
        {
            return new ObjectRef(e.ResourceName, e.Mod)
            {
                requesting = e.Requesting
            };
        }
    }
}
