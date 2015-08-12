using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class ItemRef : EntityRef<ItemDef>
    {
        public ItemRef(int resourceId)
            : base(resourceId, id => Handler.ItemDef.DefsByType.ContainsKey(id) ? Handler.ItemDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= ItemID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Item type or netID.");
        }
        public ItemRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public ItemRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public ItemRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        public override ItemDef Resolve()
        {
            if (ResourceID.HasValue && Handler.ItemDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.ItemDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.ItemDefs.ContainsKey(ResourceName))
                return Requesting.ItemDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.ItemDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla item reference '" + ResourceName + "' is not found.");

                return Handler.ItemDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Item reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].ItemDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Item reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the item is not loaded.");

            return ModData.ModsFromInternalName[ModName].ItemDefs[ResourceName];
        }
    }
}
