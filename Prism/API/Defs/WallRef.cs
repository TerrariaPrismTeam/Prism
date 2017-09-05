using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class WallRef : EntityRefWithId<WallDef>
    {
        public WallRef(int resourceId)
            : base(resourceId, id => Handler.WallDef.DefsByType.ContainsKey(id) ? Handler.WallDef.DefsByType[id].DisplayName : WallDefHandler.WALL + id)
        {
            // FIXME: this somehow borks with resourceId == 1 and WallID.Count == 231, WTF?
            if (resourceId >= WallID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla wall type, but is " + resourceId + "/" + WallID.Count + ".");
        }
        public WallRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public WallRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public WallRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        WallRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.WallDef.DefsByType.ContainsKey(id) ? Handler.WallDef.DefsByType[id].InternalName : WallDefHandler.WALL + id)
        {

        }

        public static WallRef FromIDUnsafe(int resourceId)
        {
            return new WallRef(resourceId, null);
        }

        public override WallDef Resolve()
        {
            if (ResourceID.HasValue && Handler.WallDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.WallDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.WallDefs.ContainsKey(ResourceName))
                return Requesting.WallDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.WallDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla wall reference '" + ResourceName + "' is not found.");

                return Handler.WallDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Wall reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].WallDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Wall reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the wall is not loaded.");

            return ModData.ModsFromInternalName[ModName].WallDefs[ResourceName];
        }
    }
}
