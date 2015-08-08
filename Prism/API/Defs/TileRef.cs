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
    public class TileRef : EntityRef<TileDef, TileBehaviour, Tile>
    {
        public TileRef(int resourceId)
            : base(resourceId, id => Handler.TileDef.DefsByType.ContainsKey(id) ? Handler.TileDef.DefsByType[id].InternalName : String.Empty)
        {
            if (resourceId >= TileID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Tile type.");
        }
        public TileRef(ObjectRef objRef)
            : base(objRef)
        {

        }
        public TileRef(string resourceName, ModInfo mod)
            : base(resourceName, mod)
        {

        }
        public TileRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override TileDef Resolve()
        {
            if (ResourceID.HasValue && Handler.TileDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.TileDef.DefsByType[ResourceID.Value];

            if (IsVanillaRef)
            {
                if (!Handler.TileDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla tile reference '" + ResourceName + "' is not found.");

                return Handler.TileDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Tile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].TileDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Tile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the tile is not loaded.");

            return ModData.ModsFromInternalName[ModName].TileDefs[ResourceName];
        }
    }
}
