using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.Defs;
using Terraria.ID;

namespace Prism.API
{
    public abstract class EntityRef<TEntityDef> : IEquatable<EntityRef<TEntityDef>>
        where TEntityDef : EntityDef
    {
        public string ResourceName
        {
            get;
            private set;
        }
        public string ModName
        {
            get;
            private set;
        }

        public ModInfo Mod
        {
            get
            {
                return String.IsNullOrEmpty(ModName) || ModName == EntityDef.VanillaString || ModName == EntityDef.TerrariaString
                    ? PrismApi.VanillaInfo : ModData.mods.Keys.FirstOrDefault(mi => mi.InternalName == ModName);
            }
        }

        public EntityRef(string resourceName, string modName = null)
        {
            ResourceName = resourceName;
            ModName = modName; //== EntityDef.VanillaString || modName == EntityDef.TerrariaString ? null : modName;
        }

        public abstract TEntityDef Resolve();

        public bool Equals(EntityRef<TEntityDef> other)
        {
            return ResourceName == other.ResourceName && Mod == other.Mod;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is EntityRef<TEntityDef>)
                return Equals((EntityRef<TEntityDef>)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return ResourceName.GetHashCode() + Mod.GetHashCode();
        }
        public override string ToString()
        {
            return "{" + Mod.InternalName + "." + ResourceName + "}";
        }
    }
}
