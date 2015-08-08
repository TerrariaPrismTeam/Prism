using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class NpcRef : EntityRef<NpcDef, NpcBehaviour, NPC>
    {
        public NpcRef(int resourceId)
            : base(resourceId, id => Handler.NpcDef.DefsByType.ContainsKey(id) ? Handler.NpcDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= NPCID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla NPC type or netID.");
        }
        public NpcRef(ObjectRef objRef)
            : base(objRef)
        {

        }
        public NpcRef(string resourceName, ModInfo mod)
            : base(resourceName, mod)
        {

        }
        public NpcRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override NpcDef Resolve()
        {
            if (ResourceID.HasValue && Handler.NpcDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.NpcDef.DefsByType[ResourceID.Value];

            if (IsVanillaRef)
            {
                if (!Handler.NpcDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return Handler.NpcDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].NpcDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the NPC is not loaded.");

            return ModData.ModsFromInternalName[ModName].NpcDefs[ResourceName];
        }
    }
}
