using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    //TODO: we might have to retink this, because tiles aren't quite like the other defs (except for RecipeDef, which is even more different and can be redone, too)
    sealed class TileDefHandler : EntityDefHandler<TileDef, TileBehaviour, Tile>
    {
        protected override Type IDContainerType
        {
            get
            {
                return typeof(TileID);
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            //TODO: finish this
        }

        protected override Tile GetVanillaEntityFromID(int id)
        {
            // Main.tile_ arrays must be used to get the properties
            return new Tile()
            {
                type = (ushort)id
            };
        }

        protected override TileDef NewDefFromVanilla(Tile tile)
        {
            return new TileDef(String.Empty, getTexture: () => Main.tileTexture[tile.type]);
        }

        protected override void CopyEntityToDef(Tile tile, TileDef def)
        {
            // see GetVanillaEntityFromID
            tile.type = (ushort)def.Type;
        }
        protected override void CopyDefToEntity(TileDef def, Tile tile)
        {
            //TODO: finish this
        }

        protected override List<LoaderError> CheckTextures(TileDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of TileDef " + def + " is null."));

            return ret;
        }
        protected override List<LoaderError> LoadTextures (TileDef def)
        {
            var ret = new List<LoaderError>();
            var t = def.GetTexture();

            if (t == null)
            {
                ret.Add(new LoaderError(def.Mod, "GetTexture return value is null for TileDef " + def + "."));
                return ret;
            }

            Main.tileTexture[def.Type] = def.GetTexture();
            Main.tileSetsLoaded[def.Type] = true;

            return ret;
        }

        protected override int GetRegularType(Tile tile)
        {
            return tile.type;
        }

        protected override void CopySetProperties(TileDef def)
        {
            //TODO: finish this method
        }
    }
}
