using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.Defs;
using Terraria.ID;

namespace Prism.API
{
    public abstract class EntityRef<TEntityDef>
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

        public EntityRef(int resourceId)
            : this(resourceId < ItemID.Count ? ItemDefHandler.DefFromType[resourceId].InternalName : String.Empty)
        {
            if (resourceId >= ItemID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Item type or netID.");
        }
        public EntityRef(string resourceName, string modName = null)
        {
            ResourceName = resourceName;
            ModName = modName;
        }

        public abstract TEntityDef Resolve();
    }
}
