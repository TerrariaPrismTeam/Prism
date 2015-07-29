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
            : base(Handler.ProjDef.DefsByType.ContainsKey(resourceId) ? Handler.ProjDef.DefsByType[resourceId].InternalName : String.Empty)
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
            if (IsVanillaRef)
            {
                if (!Handler.ProjDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return Handler.ProjDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.Mods.Keys.Any(mi => mi.InternalName == ModName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.Mods.First(mi => mi.Key.InternalName == ModName).Value.ItemDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the NPC is not loaded.");

            return ProjectileDef.ByName[ResourceName, ModName];
        }
    }
}
