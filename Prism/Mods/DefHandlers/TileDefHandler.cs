using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods.Behaviours;
using Prism.Mods;

namespace Prism.Mods.DefHandlers {

    public class TileDefHandler : EntityDefHandler<TileDef, TileBehaviour, Tile> {
        
        protected override Type IDContainerType {
            get {
                return typeof(TileID);
            }
        }

        internal static void OnSetDefaults(Tile tile, int type) {}

        protected override void ExtendVanillaArrays(int amt = 1) {}

        //protected override Tile GetVanillaEntityFromID(int id) { }
        
        //protected override TileDef NewDeffFromVanilla(Tile tile) { }

        public override void CopyEntityToDef(Tile entity, TileDef def) { }

        public override void CopyDefToEntity(TileDef def, Tile entity) { }
        
        protected override List<LoaderError> CheckTextures(TileDef def) {
            var ret = new List<LoaderError>();
            
            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of TileDef " + def + " is null."));
            
            return ret;
        }

        protected override List<LoaderError> LoadTextures(TileDef def) {
            
            var ret = new List<LoaderError>();
            var t = def.GetTexture();
            
            if (t == null) {
                ret.Add(new LoaderError(def.Mod, "GetTexture return value is null for TileDef " + def + ".");
                return ret;
            }
            
            Main.tileTexture[def.Type] = def.GetTexture();
            Main.tileSetsLoaded[def.Type] = true;
            
            return ret;
            
        }
        
        protected override int GetRegularType(Tile tile) {
            return tile.type;
        }
        
        protected override void CopySetProperties(TileDef def) { }

    }

}
