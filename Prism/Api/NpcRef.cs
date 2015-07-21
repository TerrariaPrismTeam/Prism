using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.Defs;

namespace Prism.API
{
    public class NpcRef : EntityRef<NpcDef>
    {
        public NpcRef(int resourceId)
            : base(resourceId)
        {

        }
        public NpcRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override NpcDef Resolve()
        {
            if (Mod == PrismApi.VanillaInfo)
            {
                if (!NpcDefHandler.VanillaDefFromName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return NpcDefHandler.VanillaDefFromName[ResourceName];
            }

            if (!ModData.Mods.Keys.Any(mi => mi.InternalName == ModName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.Mods.First(mi => mi.Key.InternalName == ModName).Value.ItemDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the item is not loaded.");

            return NpcDef.ByName[ResourceName, ModName];
        }
    }
}
