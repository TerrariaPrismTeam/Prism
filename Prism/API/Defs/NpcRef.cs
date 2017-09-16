using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class NpcRef : EntityRefWithId<NpcDef>
    {
        static string ToResName(int id)
        {
            NpcDef nd = null;
            if (Handler.NpcDef.DefsByType.TryGetValue(id, out nd))
                return nd.InternalName;

            string r = null;
            if (Handler.NpcDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown NPC ID '" + id + "'.");
        }

        public NpcRef(int resourceId)
            : base(resourceId, ToResName)
        {
            if (resourceId >= NPCID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla NPC type or netID.");
        }
        public NpcRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public NpcRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public NpcRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        NpcRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.NpcDef.DefsByType.ContainsKey(id) ? Handler.NpcDef.DefsByType[id].InternalName : String.Empty)
        {

        }

        public static NpcRef FromIDUnsafe(int resourceId)
        {
            return new NpcRef(resourceId, null);
        }

        public override NpcDef Resolve()
        {
            if (ResourceID.HasValue && Handler.NpcDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.NpcDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.NpcDefs.ContainsKey(ResourceName))
                return Requesting.NpcDefs[ResourceName];

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
