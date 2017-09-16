using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class BuffRef : EntityRefWithId<BuffDef>
    {
        static string ToResName(int id)
        {
            BuffDef bd = null;
            if (Handler.BuffDef.DefsByType.TryGetValue(id, out bd))
                return bd.InternalName;

            string r = null;
            if (Handler.BuffDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown Buff ID '" + id + "'.");
        }

        public BuffRef(int resourceId)
            : base(resourceId, ToResName)
        {
            if (resourceId >= BuffID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Buff type.");
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

        BuffRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.BuffDef.DefsByType.ContainsKey(id) ? Handler.BuffDef.DefsByType[id].InternalName : String.Empty)
        {

        }

        public static BuffRef FromIDUnsafe(int resourceId)
        {
            return new BuffRef(resourceId, null);
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
