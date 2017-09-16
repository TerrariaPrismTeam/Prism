using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class TileRef : EntityRefWithId<TileDef>
    {
        static string ToResName(int id)
        {
            TileDef td = null;
            if (Handler.TileDef.DefsByType.TryGetValue(id, out td))
                return td.InternalName;

            string r = null;
            if (Handler.TileDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown Tile ID '" + id + "'.");
        }

        public TileRef(int resourceId)
            : base(resourceId, ToResName)
        {
            if ((uint)resourceId >= unchecked((uint)TileID.Count))
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Tile type.");
        }
        public TileRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public TileRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public TileRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        TileRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.TileDef.DefsByType.ContainsKey(id) ? Handler.TileDef.DefsByType[id].InternalName : String.Empty)
        {

        }

        public static TileRef FromIDUnsafe(int resourceId)
        {
            return new TileRef(resourceId, null);
        }

        public override TileDef Resolve()
        {
            if (ResourceID.HasValue && Handler.TileDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.TileDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.TileDefs.ContainsKey(ResourceName))
                return Requesting.TileDefs[ResourceName];

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

        public static implicit operator Either<TileRef, CraftGroup<TileDef, TileRef>>(TileRef r)
        {
            return Either<TileRef, CraftGroup<TileDef, TileRef>>.NewRight(r);
        }
        public static implicit operator Either<TileRef, TileGroup>(TileRef r)
        {
            return Either<TileRef, TileGroup>.NewRight(r);
        }
    }
}
