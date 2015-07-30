using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.ID;

namespace Prism.API.Defs {
    public class TileRef : EntityRef<TileDef, TileBehaviour, Tile> {
        public TileRef(int resourceId)
            : base(Handler.TileDef.DefsByType.ContainsKey(resourceId) ? Handler.TileDef.DefsByType[resourceId].InternalName : String.Empty) {
            if (resourceId >= TileID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Tile type.");
        }
        public TileRef(string resourceName, string modName = null)
            : base(resourceName, modName) {

        }

        public override TileDef Resolve() {
            if (IsVanillaRef) {
                if (!Handler.TileDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla tile reference '" + ResourceName + "' is not found.");

                return Handler.TileDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.Mods.Keys.Any(mi => mi.InternalName == ModName))
                throw new InvalidOperationException("Tile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.Mods.First(mi => mi.Key.InternalName == ModName).Value.TileDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Tile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the tile is not loaded.");

            return TileDef.ByName[ResourceName, ModName];
        }
    }
}