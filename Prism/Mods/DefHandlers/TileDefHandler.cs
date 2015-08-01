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
        /*
            Search done for any arrays created with tile count. There should be a lot more. Also the sets...:

              Terraria\Main.cs(1005):		public static bool[] tileLighted = new bool[419];
              Terraria\Main.cs(1006):		public static bool[] tileMergeDirt = new bool[419];
              Terraria\Main.cs(1007):		public static bool[] tileCut = new bool[419];
              Terraria\Main.cs(1008):		public static bool[] tileAlch = new bool[419];
              Terraria\Main.cs(1009):		public static int[] tileShine = new int[419];
              Terraria\Main.cs(1010):		public static bool[] tileShine2 = new bool[419];
              Terraria\Main.cs(1015):		public static bool[] tileStone = new bool[419];
              Terraria\Main.cs(1016):		public static bool[] tileAxe = new bool[419];
              Terraria\Main.cs(1017):		public static bool[] tileHammer = new bool[419];
              Terraria\Main.cs(1018):		public static bool[] tileWaterDeath = new bool[419];
              Terraria\Main.cs(1019):		public static bool[] tileLavaDeath = new bool[419];
              Terraria\Main.cs(1020):		public static bool[] tileTable = new bool[419];
              Terraria\Main.cs(1021):		public static bool[] tileBlockLight = new bool[419];
              Terraria\Main.cs(1022):		public static bool[] tileNoSunLight = new bool[419];
              Terraria\Main.cs(1023):		public static bool[] tileDungeon = new bool[419];
              Terraria\Main.cs(1024):		public static bool[] tileSpelunker = new bool[419];
              Terraria\Main.cs(1025):		public static bool[] tileSolidTop = new bool[419];
              Terraria\Main.cs(1026):		public static bool[] tileSolid = new bool[419];
              Terraria\Main.cs(1027):		public static bool[] tileBouncy = new bool[419];
              Terraria\Main.cs(1028):		public static short[] tileValue = new short[419];
              Terraria\Main.cs(1029):		public static byte[] tileLargeFrames = new byte[419];
              Terraria\Main.cs(1031):		public static bool[] tileRope = new bool[419];
              Terraria\Main.cs(1032):		public static bool[] tileBrick = new bool[419];
              Terraria\Main.cs(1033):		public static bool[] tileMoss = new bool[419];
              Terraria\Main.cs(1034):		public static bool[] tileNoAttach = new bool[419];
              Terraria\Main.cs(1035):		public static bool[] tileNoFail = new bool[419];
              Terraria\Main.cs(1036):		public static bool[] tileObsidianKill = new bool[419];
              Terraria\Main.cs(1037):		public static bool[] tileFrameImportant = new bool[419];
              Terraria\Main.cs(1038):		public static bool[] tilePile = new bool[419];
              Terraria\Main.cs(1039):		public static bool[] tileBlendAll = new bool[419];
              Terraria\Main.cs(1040):		public static short[] tileGlowMask = new short[419];
              Terraria\Main.cs(1041):		public static bool[] tileContainer = new bool[419];
              Terraria\Main.cs(1042):		public static bool[] tileSign = new bool[419];
              Terraria\Main.cs(1043):		public static bool[][] tileMerge = new bool[419][];
              Terraria\Player.cs(580):		public bool[] adjTile = new bool[419];
              Terraria\Player.cs(581):		public bool[] oldAdjTile = new bool[419];
              Terraria\WorldGen.cs(567):		public static int[] tileCounts = new int[419];
              Terraria\WorldGen.cs(636):		public static bool[] houseTile = new bool[419];
              Terraria.Map\MapHelper.cs(118):            Color[][] array = new Color[419][];
              Terraria.Map\MapHelper.cs(953):            MapHelper.tileOptionCounts = new int[419];
              Terraria.Map\MapHelper.cs(982):            MapHelper.tileLookup = new ushort[419];
        */

        protected override Type IDContainerType
        {
            get
            {
                return typeof(TileID);
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.itemAnimations.Length + amt : ItemID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.itemTexture, newLen);

            Array.Resize(ref Main.tileLighted               , newLen);      
            Array.Resize(ref Main.tileMergeDirt             , newLen);    
            Array.Resize(ref Main.tileCut                   , newLen);          
            Array.Resize(ref Main.tileAlch                  , newLen);         
            Array.Resize(ref Main.tileShine                 , newLen);        
            Array.Resize(ref Main.tileShine2                , newLen);       
            Array.Resize(ref Main.tileStone                 , newLen);        
            Array.Resize(ref Main.tileAxe                   , newLen);          
            Array.Resize(ref Main.tileHammer                , newLen);       
            Array.Resize(ref Main.tileWaterDeath            , newLen);   
            Array.Resize(ref Main.tileLavaDeath             , newLen);    
            Array.Resize(ref Main.tileTable                 , newLen);        
            Array.Resize(ref Main.tileBlockLight            , newLen);   
            Array.Resize(ref Main.tileNoSunLight            , newLen);   
            Array.Resize(ref Main.tileDungeon               , newLen);      
            Array.Resize(ref Main.tileSpelunker             , newLen);    
            Array.Resize(ref Main.tileSolidTop              , newLen);     
            Array.Resize(ref Main.tileSolid                 , newLen);        
            Array.Resize(ref Main.tileBouncy                , newLen);       
            Array.Resize(ref Main.tileValue                 , newLen);        
            Array.Resize(ref Main.tileLargeFrames           , newLen);  
            Array.Resize(ref Main.tileRope                  , newLen);         
            Array.Resize(ref Main.tileBrick                 , newLen);        
            Array.Resize(ref Main.tileMoss                  , newLen);         
            Array.Resize(ref Main.tileNoAttach              , newLen);     
            Array.Resize(ref Main.tileNoFail                , newLen);       
            Array.Resize(ref Main.tileObsidianKill          , newLen); 
            Array.Resize(ref Main.tileFrameImportant        , newLen);
            Array.Resize(ref Main.tilePile                  , newLen);         
            Array.Resize(ref Main.tileBlendAll              , newLen);     
            Array.Resize(ref Main.tileGlowMask              , newLen);     
            Array.Resize(ref Main.tileContainer             , newLen);    
            Array.Resize(ref Main.tileSign                  , newLen);         
            Array.Resize(ref Main.tileMerge                 , newLen);                  
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
            //def.Type                                    = tile.type;
            //def.HousingConfig.IsChair                   = null;
            //def.HousingConfig.IsDoor                    = null;
            //def.HousingConfig.IsTable                   = null;
            //def.HousingConfig.IsTorch                   = null;
            //def.PlaceConfig.CheckWalls                  = tile.;
            //def.PlaceConfig.Directional                 = null;
            //def.PlaceConfig.MergesWithDirt              = null;
            //def.PlaceConfig.NoAttach                    = null;
            //def.PlaceConfig.PlacementConditions         = null;
            //def.PlaceConfig.PlacementFrame              = null;
            //def.PlaceConfig.PlacementOrigin             = null;
            //def.MineConfig.BreakDust                    = null;
            //def.MineConfig.BreaksByAxe                  = null;
            //def.MineConfig.BreaksByCut                  = null;
            //def.MineConfig.BreaksByHammer               = null;
            //def.MineConfig.BreaksByLava                 = null;
            //def.MineConfig.BreaksByPick                 = null;
            //def.MineConfig.BreaksByWater                = null;
            //def.MineConfig.BreaksInstantly              = null;
            //def.MineConfig.ItemDrop                     = null;
            //def.MineConfig.MinAxe                       = null;
            //def.MineConfig.MinHammer                    = null;
            //def.MineConfig.MinPick                      = null;
            //def.MineConfig.RateAxe                      = null;
            //def.MineConfig.RateHammer                   = null;
            //def.MineConfig.RatePick                     = null;
            //def.MineConfig.Sound                        = null;
            //def.MineConfig.SoundGroup                   = null;
            //def.LightingConfig.BlocksLight              = null;
            //def.LightingConfig.BlocksSun                = null;
            //def.LightingConfig.DangersenseGlow          = null;
            //def.LightingConfig.Glows                    = null;
            //def.LightingConfig.ShineChance              = null;
            //def.LightingConfig.Shines                   = null;
            //def.LightingConfig.SpelunkerGlow            = null;
            //def.FrameConfig.DrawOffsetY                 = null;
            //def.FrameConfig.FrameCounterMax             = null;
            //def.FrameConfig.FrameHeight                 = null;
            //def.FrameConfig.FrameImportant              = null;
            //def.FrameConfig.FrameWidth                  = null;
            //def.FrameConfig.InitFrame                   = null;
            //def.FrameConfig.LargeFrames                 = null;
            //def.FrameConfig.MaxFrame                    = null;
            //def.FrameConfig.SheetYAligned               = null;
            //def.FrameConfig.TileHeight                  = null;
            //def.FrameConfig.TileWidth                   = null;
            //def.AlchemyFlower                           = null;
            //def.Brick                                   = null;
            //def.Chest                                   = null;
            //def.ExplosionResistant                      = null;
            //def.Grass                                   = null;
            //def.MapColor                                = null;
            //def.MapHoverText                            = null;
            //def.Moss                                    = null;
            //def.NoFail                                  = null;
            //def.ObsidianKill                            = null;
            //def.Pile                                    = null;
            //def.PlaceConfig                             = null;
            //def.Platform                                = null;
            //def.Rope                                    = null;
            //def.Slopable                                = null;
            //def.Solid                                   = null;
            //def.SolidTop                                = null;
            //def.Spawn                                   = null;
            //def.SpawnAt                                 = null;
            //def.Stone                                   = null;
            //def.TileDungeon                             = null;
            //def.TileFlame                               = null;
            //def.TileSand                                = null;
        }
        protected override void CopyDefToEntity(TileDef def, Tile tile)
        {
            
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
