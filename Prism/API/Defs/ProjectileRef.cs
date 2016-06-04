using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class ProjectileRef : EntityRefWithId<ProjectileDef>
    {
        public ProjectileRef(int resourceId)
            : base(resourceId, id => Handler.ProjDef.DefsByType.ContainsKey(id) ? Handler.ProjDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= ProjectileID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Projectile type.");
        }
        public ProjectileRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public ProjectileRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public ProjectileRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        ProjectileRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.ProjDef.DefsByType.ContainsKey(id) ? Handler.ProjDef.DefsByType[id].InternalName : String.Empty)
        {

        }

        public static ProjectileRef FromIDUnsafe(int resourceId)
        {
            return new ProjectileRef(resourceId, null);
        }

        public override ProjectileDef Resolve()
        {
            if (ResourceID.HasValue && Handler.ProjDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.ProjDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.ProjectileDefs.ContainsKey(ResourceName))
                return Requesting.ProjectileDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.ProjDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return Handler.ProjDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Projectile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].ProjectileDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Projectile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the Projectile is not loaded.");

            return ModData.ModsFromInternalName[ModName].ProjectileDefs[ResourceName];
        }
    }
}
