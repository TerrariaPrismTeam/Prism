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
    public class ProjectileRef : EntityRef<ProjectileDef, ProjectileBehaviour, Projectile>
    {
        public ProjectileRef(int resourceId)
            : base(resourceId, id => Handler.ProjDef.DefsByType.ContainsKey(id) ? Handler.ProjDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= ProjectileID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Projectile type.");
        }
        public ProjectileRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override ProjectileDef Resolve()
        {
            if (ResourceID.HasValue && Handler.ProjDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.ProjDef.DefsByType[ResourceID.Value];

            if (IsVanillaRef)
            {
                if (!Handler.ProjDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return Handler.ProjDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.Mods.Keys.Any(mi => mi.InternalName == ModName))
                throw new InvalidOperationException("Projectile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.Mods.First(mi => mi.Key.InternalName == ModName).Value.ProjectileDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Projectile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the Projectile is not loaded.");

            return ProjectileDef.ByName[ResourceName, ModName];
        }
    }
}
