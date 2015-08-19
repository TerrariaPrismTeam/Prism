using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class BuffRef : EntityRefWithId<BuffDef>
    {
        public BuffRef(int resourceId)
            : base(resourceId, id => Handler.ItemDef.DefsByType.ContainsKey(id) ? Handler.ItemDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= BuffID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Buff type or netID.");
        }
        public BuffRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public BuffRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public BuffRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        public override BuffDef Resolve()
        {
            if (ResourceID.HasValue && Handler.BuffDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.BuffDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.BuffDefs.ContainsKey(ResourceName))
                return Requesting.BuffDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.BuffDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla buff reference '" + ResourceName + "' is not found.");

                return Handler.BuffDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Buff reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].BuffDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Buff reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the buff is not loaded.");

            return ModData.ModsFromInternalName[ModName].BuffDefs[ResourceName];
        }
    }
}
