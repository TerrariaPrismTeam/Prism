using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;

namespace Prism.API.Defs
{
    public class MountRef : EntityRefWithId<MountDef>
    {
        public MountRef(int resourceId)
            : base(resourceId, id => Handler.MountDef.DefsByType.ContainsKey(id) ? Handler.MountDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= MountID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Mount type.");
        }
        public MountRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public MountRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public MountRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        MountRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.MountDef.DefsByType.ContainsKey(id) ? Handler.MountDef.DefsByType[id].InternalName : String.Empty)
        {

        }

        public static MountRef FromIDUnsafe(int resourceId)
        {
            return new MountRef(resourceId, null);
        }

        public override MountDef Resolve()
        {
            if (ResourceID.HasValue && Handler.MountDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.MountDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.MountDefs.ContainsKey(ResourceName))
                return Requesting.MountDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.MountDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla mount reference '" + ResourceName + "' is not found.");

                return Handler.MountDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Mount reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].MountDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Mount reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mount is not loaded.");

            return ModData.ModsFromInternalName[ModName].MountDefs[ResourceName];
        }
    }
}
