using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.Defs;
using Terraria;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class ItemRef : EntityRef<ItemDef, ItemBehaviour, Item>
    {
        public ItemRef(int resourceId)
            : base(ItemDefHandler.DefFromType.ContainsKey(resourceId) ? ItemDefHandler.DefFromType[resourceId].InternalName : String.Empty)
        {
            if (resourceId >= ItemID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Item type or netID.");
        }
        public ItemRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override ItemDef Resolve()
        {
            if (IsVanillaRef)
            {
                if (!ItemDefHandler.VanillaDefFromName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla item reference '" + ResourceName + "' is not found.");

                return ItemDefHandler.VanillaDefFromName[ResourceName];
            }

            if (!ModData.Mods.Keys.Any(mi => mi.InternalName == ModName))
                throw new InvalidOperationException("Item reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.Mods.First(mi => mi.Key.InternalName == ModName).Value.ItemDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Item reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the item is not loaded.");

            return ItemDef.ByName[ResourceName, ModName];
        }
    }
}
