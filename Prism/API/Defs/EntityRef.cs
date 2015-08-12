using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Util;

namespace Prism.API.Defs
{
    public abstract class EntityRef<T> : IEquatable<EntityRef<T>>
    {
        Lazy<string> resName;

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

        protected internal EntityRef(Func<string> toResName)
        {
            resName = new Lazy<string>(toResName ?? Empty<string>.Func);
        }
        protected EntityRef(ObjectRef objRef, Assembly calling)
            : this(() => objRef.Name)
        {
            resName = new Lazy<string>(() => objRef.Name);

            ModName = objRef.ModName;

            Requesting = ModData.ModFromAssembly(calling);
        }

        public abstract T Resolve();

        public virtual bool Equals(EntityRef<T> other)
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
            return String.IsNullOrEmpty(ResourceName) ? "<empty>" : ("{" + Mod.InternalName + "." + ResourceName + "}");
        }

        public static implicit operator ObjectRef(EntityRef<T> e)
        {
            return new ObjectRef(e.ResourceName, e.Mod)
            {
                requesting = e.Requesting
            };
        }
    }
    public abstract class EntityRefWithId<T> : EntityRef<T>
    {
        public int? ResourceID
        {
            get;
            private set;
        }

        protected EntityRefWithId(int resourceId, Func<int, string> toResName)
            : base(() => resourceId == 0 ? String.Empty : toResName(resourceId))
        {
            ResourceID = resourceId;
        }
        protected EntityRefWithId(ObjectRef objRef, Assembly calling)
            : base(objRef, calling)
        {

        }

        public override bool Equals(EntityRef<T> other)
        {
            if (ResourceID.HasValue && other is EntityRefWithId<T>)
            {
                var erid = (EntityRefWithId<T>)other;

                if (erid.ResourceID.HasValue)
                    return ResourceID.Value == erid.ResourceID.Value;
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return ResourceID.HasValue ? ResourceID.GetHashCode() : base.GetHashCode();
        }
        public override string ToString()
        {
            return (ResourceID.HasValue ? ("#" + ResourceID.Value + " ") : String.Empty) + base.ToString();
        }
    }
}
