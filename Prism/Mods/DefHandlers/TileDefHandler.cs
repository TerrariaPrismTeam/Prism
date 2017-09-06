using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Util;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Map;
using Terraria.ObjectData;

namespace Prism.Mods.DefHandlers
{
    //TODO: we might have to retink this, because tiles aren't quite like the other defs (except for RecipeDef, which is even more different and can be redone, too)
    //TODO: fill arrays in Terraria.Map.MapHelper
    sealed class TileDefHandler : GEntityDefHandler<TileDef, TileBehaviour, Tile>
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
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Main.tileSetsLoaded.Length + amt : TileID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.tileTexture, newLen);

            Array.Resize(ref Main.tileSetsLoaded    , newLen);
            Array.Resize(ref Main.tileAlch          , newLen);
            Array.Resize(ref Main.tileAxe           , newLen);
            Array.Resize(ref Main.tileBlendAll      , newLen);
            Array.Resize(ref Main.tileBlockLight    , newLen);
            Array.Resize(ref Main.tileBouncy        , newLen);
            Array.Resize(ref Main.tileBrick         , newLen);
            Array.Resize(ref Main.tileContainer     , newLen);
            Array.Resize(ref Main.tileCut           , newLen);
            Array.Resize(ref Main.tileDungeon       , newLen);
            Array.Resize(ref Main.tileFlame         , newLen);
            Array.Resize(ref Main.tileFrame         , newLen);
            Array.Resize(ref Main.tileFrameCounter  , newLen);
            Array.Resize(ref Main.tileFrameImportant, newLen);
            Array.Resize(ref Main.tileGlowMask      , newLen);
            Array.Resize(ref Main.tileHammer        , newLen);
            Array.Resize(ref Main.tileLargeFrames   , newLen);
            Array.Resize(ref Main.tileLavaDeath     , newLen);
            Array.Resize(ref Main.tileLighted       , newLen);
            Array.Resize(ref Main.tileMergeDirt     , newLen);
            Array.Resize(ref Main.tileMoss          , newLen);
            Array.Resize(ref Main.tileNoAttach      , newLen);
            Array.Resize(ref Main.tileNoFail        , newLen);
            Array.Resize(ref Main.tileNoSunLight    , newLen);
            Array.Resize(ref Main.tileObsidianKill  , newLen);
            Array.Resize(ref Main.tilePile          , newLen);
            Array.Resize(ref Main.tileRope          , newLen);
            Array.Resize(ref Main.tileSand          , newLen);
            Array.Resize(ref Main.tileShine         , newLen);
            Array.Resize(ref Main.tileShine2        , newLen);
            Array.Resize(ref Main.tileSign          , newLen);
            Array.Resize(ref Main.tileSolid         , newLen);
            Array.Resize(ref Main.tileSolidTop      , newLen);
            Array.Resize(ref Main.tileSpelunker     , newLen);
            Array.Resize(ref Main.tileStone         , newLen);
            Array.Resize(ref Main.tileTable         , newLen);
            Array.Resize(ref Main.tileValue         , newLen);
            Array.Resize(ref Main.tileWaterDeath    , newLen);

            //TODO: add map colour support
            Array.Resize(ref MapHelper.tileLookup      , newLen);
            Array.Resize(ref MapHelper.tileOptionCounts, newLen);
          //Array.Resize(ref Lang.mapLegend            , newLen);

            Array.Resize(ref TileID.Sets.AllTiles                    , newLen);
            Array.Resize(ref TileID.Sets.AvoidedByNPCs               , newLen);
            Array.Resize(ref TileID.Sets.BlocksStairs                , newLen);
            Array.Resize(ref TileID.Sets.BlocksStairsAbove           , newLen);
            Array.Resize(ref TileID.Sets.BreakableWhenPlacing        , newLen);
            Array.Resize(ref TileID.Sets.CanBeClearedDuringGeneration, newLen);
            Array.Resize(ref TileID.Sets.ChecksForMerge              , newLen);
            Array.Resize(ref TileID.Sets.Corrupt                     , newLen);
            Array.Resize(ref TileID.Sets.Crimson                     , newLen);
            Array.Resize(ref TileID.Sets.Falling                     , newLen);
            Array.Resize(ref TileID.Sets.FramesOnKillWall            , newLen);
            Array.Resize(ref TileID.Sets.GeneralPlacementTiles       , newLen);
            Array.Resize(ref TileID.Sets.GrassSpecial                , newLen);
            Array.Resize(ref TileID.Sets.Hallow                      , newLen);
            Array.Resize(ref TileID.Sets.HellSpecial                 , newLen);
            Array.Resize(ref TileID.Sets.HousingWalls                , newLen);
            Array.Resize(ref TileID.Sets.Ices                        , newLen);
            Array.Resize(ref TileID.Sets.IcesSlush                   , newLen);
            Array.Resize(ref TileID.Sets.IcesSnow                    , newLen);
            Array.Resize(ref TileID.Sets.InteractibleByNPCs          , newLen);
            Array.Resize(ref TileID.Sets.JungleSpecial               , newLen);
            Array.Resize(ref TileID.Sets.Leaves                      , newLen);
            Array.Resize(ref TileID.Sets.Mud                         , newLen);
            Array.Resize(ref TileID.Sets.NotReallySolid              , newLen);
            Array.Resize(ref TileID.Sets.Ore                         , newLen);
            Array.Resize(ref TileID.Sets.Snow                        , newLen);
            Array.Resize(ref TileID.Sets.TouchDamageHot              , newLen);
            Array.Resize(ref TileID.Sets.TouchDamageOther            , newLen);
            Array.Resize(ref TileID.Sets.TouchDamageSands            , newLen);
            Array.Resize(ref TileID.Sets.TouchDamageVines            , newLen);

            Array.Resize(ref TileID.Sets.Conversion.Grass       , newLen);
            Array.Resize(ref TileID.Sets.Conversion.HardenedSand, newLen);
            Array.Resize(ref TileID.Sets.Conversion.Ice         , newLen);
            Array.Resize(ref TileID.Sets.Conversion.Moss        , newLen);
            Array.Resize(ref TileID.Sets.Conversion.Sand        , newLen);
            Array.Resize(ref TileID.Sets.Conversion.Sandstone   , newLen);
            Array.Resize(ref TileID.Sets.Conversion.Stone       , newLen);
            Array.Resize(ref TileID.Sets.Conversion.Thorn       , newLen);

          //Array.Resize(ref TileID.Sets.RoomNeeds.CountsAsChair, newLen);
          //Array.Resize(ref TileID.Sets.RoomNeeds.CountsAsDoor , newLen);
          //Array.Resize(ref TileID.Sets.RoomNeeds.CountsAsTable, newLen);
          //Array.Resize(ref TileID.Sets.RoomNeeds.CountsAsTorch, newLen);

            Array.Resize(ref Main.tileMerge, newLen);
            for (int i = 0; i < Main.tileMerge.Length; i++)
                Array.Resize(ref Main.tileMerge[i], newLen);

            if (TileObjectData._data == null)
                TileObjectData._data = new List<TileObjectData>(newLen);
            else if (TileObjectData._data.Count > newLen)
                TileObjectData._data.RemoveRange(newLen, TileObjectData._data.Count - newLen);
            else if (TileObjectData._data.Count != newLen)
                TileObjectData._data.Capacity = newLen;
        }

        protected override Tile GetVanillaEntityFromID(int id)
        {
            // Main.tile_ & TileID.Sets.* arrays must be used to get the properties
            return new Tile()
            {
                type = (ushort)id
            };
        }
        protected override TileDef NewDefFromVanilla(Tile tile)
        {
            return new TileDef(ObjectName.Empty, getTexture: () => Main.tileTexture[tile.type])
            {
                Type  = tile.type,
                NetID = tile.type
            };
        }
        protected override string GetNameVanillaMethod(Tile tile)
        {
            return Lang.GetMapObjectName(tile.type) ?? String.Empty; //! might return empty string (if arr entry is null or empty)
        }

        protected override void CopyEntityToDef(Tile tile, TileDef def)
        {
            var t = tile.type;

            def.AllTiles  = TileID.Sets.AllTiles[t];
            def.IsFalling = TileID.Sets.Falling [t];
            def.NoFail    = Main.tileNoFail     [t];
            def.Value     = Main.tileValue      [t];

            def.AestheticData = new TileAestheticData()
            {
                BlendAll       = Main.tileBlendAll         [t],
                ChecksForMerge = TileID.Sets.ChecksForMerge[t],
                MergesWithDirt = Main.tileMergeDirt        [t],
                MergeWith      = Main.tileMerge            [t].SelectIndex((i, v) => v ? new TileRef(i) : null).Where(r => r != null).ToList()
            };

            def.CollisionData = new TileCollisionData()
            {
                DamageData = new TileDamageData()
                {
                    Hot   = TileID.Sets.TouchDamageHot  [t],
                    Sand  = TileID.Sets.TouchDamageSands[t],
                    Vines = TileID.Sets.TouchDamageVines[t],
                    Other = TileID.Sets.TouchDamageOther[t]
                },

                IsBouncy       = Main.tileBouncy           [t],
                IsSolid        = Main.tileSolid            [t],
                IsSolidTop     = Main.tileSolidTop         [t],
                NotReallySolid = TileID.Sets.NotReallySolid[t]
            };

            def.ConversionData = new TileConversionData()
            {
                IsCorrupt       = TileID.Sets.Corrupt      [t],
                IsCrimson       = TileID.Sets.Crimson      [t],
                IsHallow        = TileID.Sets.Hallow       [t],
                IsSpecialGrass  = TileID.Sets.GrassSpecial [t],
                IsSpecialHell   = TileID.Sets.HellSpecial  [t],
                IsSpecialJungle = TileID.Sets.JungleSpecial[t],

                IsGrass        = TileID.Sets.Conversion.Grass       [t],
                IsHardenedSand = TileID.Sets.Conversion.HardenedSand[t],
                IsIce          = TileID.Sets.Conversion.Ice         [t],
                IsMoss         = TileID.Sets.Conversion.Moss        [t],
                IsSand         = TileID.Sets.Conversion.Sand        [t],
                IsSandstone    = TileID.Sets.Conversion.Sandstone   [t],
                IsStone        = TileID.Sets.Conversion.Stone       [t],
                IsThorn        = TileID.Sets.Conversion.Thorn       [t]
            };

            def.FrameData = new TileFrameData()
            {
                FrameImportant   = Main.tileFrameImportant     [t],
                LargeFrames      = Main.tileLargeFrames        [t],
                FramesOnKillWall = TileID.Sets.FramesOnKillWall[t]
            };

            def.HousingData = new TileHousingData()
            {
                IsWall        = TileID.Sets.HousingWalls[t],
                IsChair       = Array.IndexOf(TileID.Sets.RoomNeeds.CountsAsChair, t) != -1,
                IsDoor        = Array.IndexOf(TileID.Sets.RoomNeeds.CountsAsDoor , t) != -1,
                IsTable       = Array.IndexOf(TileID.Sets.RoomNeeds.CountsAsTable, t) != -1,
                IsLightSource = Array.IndexOf(TileID.Sets.RoomNeeds.CountsAsTorch, t) != -1,
            };

            def.LightingData = new TileLightingData()
            {
                BlocksLight    = Main.tileBlockLight[t],
                BlocksSunlight = Main.tileNoSunLight[t],
                GlowMask       = Main.tileGlowMask  [t],
                ShineChance    = Main.tileShine     [t],
                Shines         = Main.tileShine2    [t],
                SpelunkerGlow  = Main.tileSpelunker [t],
                Glows          = Main.tileLighted   [t]
            };

            def.MineData = new TileMineData()
            {
                MineTool = Main.tileHammer[t] ? TileMineTool.Hammer : Main.tileAxe[t] ? TileMineTool.Axe : TileMineTool.Pickaxe,

                BreaksByCut   = Main.tileCut       [t],
                BreaksByWater = Main.tileWaterDeath[t],
                BreaksByLava  = Main.tileLavaDeath [t]
            };

            def.NpcData = new TileNpcData()
            {
                IsAvoided      = TileID.Sets.AvoidedByNPCs     [t],
                IsInteractible = TileID.Sets.InteractibleByNPCs[t]
            };

            def.PlacementData = new TilePlacementData()
            {
                DisallowAttachingOtherTiles = Main.tileNoAttach                       [t],
                BlocksStairs                = TileID.Sets.BlocksStairs                [t],
                BlocksStairsAbove           = TileID.Sets.BlocksStairsAbove           [t],
                BreaksWhenPlacing           = TileID.Sets.BreakableWhenPlacing        [t],
                CanBeClearedDuringWorldGen  = TileID.Sets.CanBeClearedDuringGeneration[t],
                IsGeneralPlacementTile      = TileID.Sets.GeneralPlacementTiles       [t]
            };

            def.SubtypeData = new TileSubtypeData()
            {
                IsAlchemyTable  = Main.tileAlch     [t],
                IsBrick         = Main.tileBrick    [t],
                IsContainer     = Main.tileContainer[t],
                IsCraftingTable = Main.tileTable    [t],
                IsDungeonTile   = Main.tileDungeon  [t],
                IsFlame         = Main.tileFlame    [t],
                IsMoss          = Main.tileMoss     [t],
                IsPile          = Main.tilePile     [t],
                IsRope          = Main.tileRope     [t],
                IsSand          = Main.tileSand     [t],
                IsSign          = Main.tileSign     [t],
                IsStone         = Main.tileStone    [t],

                IsIce      = TileID.Sets.Ices     [t],
                IsIceSlush = TileID.Sets.IcesSlush[t],
                IsIceSnow  = TileID.Sets.IcesSnow [t],
                IsLeaves   = TileID.Sets.Leaves   [t],
                IsMud      = TileID.Sets.Mud      [t],
                IsOre      = TileID.Sets.Ore      [t],
                IsSnow     = TileID.Sets.Snow     [t]
            };

            def.OtherData = TileObjectData._data.Count <= t || TileObjectData._data[t] == null ? GetDefaultObjData(def) : null;
        }
        protected override void CopyDefToEntity(TileDef def, Tile tile)
        {
            tile.type = (ushort)def.Type;
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

        static TileObjectData GetDefaultObjData(TileDef def)
        {
            switch (def.Width)
            {
                case 1:
                    switch (def.Height)
                    {
                        case 1:
                            if (def.SubtypeData.IsAlchemyTable)
                                return TileObjectData.StyleOnTable1x1;
                            if (def.LightingData.Glows || def.HousingData.IsLightSource)
                                return TileObjectData.StyleTorch;

                            return TileObjectData.Style1x1;
                        case 2:
                            return def.CollisionData.IsSolidTop ? TileObjectData.Style1x2Top : TileObjectData.Style1x2;
                        default:
                            return TileObjectData.Style1xX;
                    }
                case 2:
                    switch (def.Height)
                    {
                        case 1:
                            return TileObjectData.Style2x1;
                        case 2:
                            return TileObjectData.Style2x2;
                        default:
                            return TileObjectData.Style2xX;
                    }
                case 3:
                    switch (def.Height)
                    {
                        case 2:
                            return TileObjectData.Style3x2;
                        case 3:
                            return def.HousingData.IsWall ? TileObjectData.Style3x3Wall : TileObjectData.Style3x3;
                        case 4:
                            return TileObjectData.Style3x4;
                    }
                    break;
                case 4:
                    switch (def.Height)
                    {
                        case 2:
                            return TileObjectData.Style4x2;
                    }
                    break;
                case 6:
                    switch (def.Height)
                    {
                        case 3:
                            return TileObjectData.Style6x3;
                    }
                    break;
            }

            return def.SubtypeData.IsAlchemyTable ? TileObjectData.StyleAlch : null;
        }

        protected override void CopySetProperties(TileDef def)
        {
            var t = def.Type;

            Main.tileAlch          [t] = def.SubtypeData.IsAlchemyTable               ;
            Main.tileAxe           [t] = def.MineData.MineTool == TileMineTool.Axe    ;
            Main.tileBlendAll      [t] = def.AestheticData.BlendAll                   ;
            Main.tileBlockLight    [t] = def.LightingData.BlocksLight                 ;
            Main.tileBouncy        [t] = def.CollisionData.IsBouncy                   ;
            Main.tileBrick         [t] = def.SubtypeData.IsBrick                      ;
            Main.tileContainer     [t] = def.SubtypeData.IsContainer                  ;
            Main.tileCut           [t] = def.MineData.BreaksByCut                     ;
            Main.tileDungeon       [t] = def.SubtypeData.IsDungeonTile                ;
            Main.tileFlame         [t] = def.SubtypeData.IsFlame                      ;
            Main.tileFrameImportant[t] = def.FrameData.FrameImportant                 ;
            Main.tileGlowMask      [t] = def.LightingData.GlowMask                    ;
            Main.tileHammer        [t] = def.MineData.MineTool == TileMineTool.Hammer ;
            Main.tileLargeFrames   [t] = def.FrameData.LargeFrames                    ;
            Main.tileLavaDeath     [t] = def.MineData.BreaksByLava                    ;
            Main.tileLighted       [t] = def.LightingData.Glows                       ;
            Main.tileMergeDirt     [t] = def.AestheticData.MergesWithDirt             ;
            Main.tileMoss          [t] = def.SubtypeData.IsMoss                       ;
            Main.tileNoAttach      [t] = def.PlacementData.DisallowAttachingOtherTiles;
            Main.tileNoFail        [t] = def.NoFail                                   ;
            Main.tileNoSunLight    [t] = def.LightingData.BlocksSunlight              ;
            Main.tileObsidianKill  [t] = def.MineData.BreaksByLava                    ;
            Main.tilePile          [t] = def.SubtypeData.IsPile                       ;
            Main.tileRope          [t] = def.SubtypeData.IsRope                       ;
            Main.tileSand          [t] = def.SubtypeData.IsSand                       ;
            Main.tileShine         [t] = def.LightingData.ShineChance                 ;
            Main.tileShine2        [t] = def.LightingData.Shines                      ;
            Main.tileSign          [t] = def.SubtypeData.IsSign                       ;
            Main.tileSolid         [t] = def.CollisionData.IsSolid                    ;
            Main.tileSolidTop      [t] = def.CollisionData.IsSolidTop                 ;
            Main.tileSpelunker     [t] = def.LightingData.SpelunkerGlow               ;
            Main.tileStone         [t] = def.SubtypeData.IsStone                      ;
            Main.tileTable         [t] = def.SubtypeData.IsCraftingTable              ;
            Main.tileValue         [t] = def.Value                                    ;
            Main.tileWaterDeath    [t] = def.MineData.BreaksByWater                   ;

            //Lang.mapLegend[MapHelper.TileToLookup(t, 0)] = def.MapTooltip ?? String.Empty;

            TileID.Sets.AllTiles                    [t] = def.AllTiles                                ;
            TileID.Sets.AvoidedByNPCs               [t] = def.NpcData.IsAvoided                       ;
            TileID.Sets.BlocksStairs                [t] = def.PlacementData.BlocksStairs              ;
            TileID.Sets.BlocksStairsAbove           [t] = def.PlacementData.BlocksStairsAbove         ;
            TileID.Sets.BreakableWhenPlacing        [t] = def.PlacementData.BreaksWhenPlacing         ;
            TileID.Sets.CanBeClearedDuringGeneration[t] = def.PlacementData.CanBeClearedDuringWorldGen;
            TileID.Sets.ChecksForMerge              [t] = def.AestheticData.ChecksForMerge            ;
            TileID.Sets.Corrupt                     [t] = def.ConversionData.IsCorrupt                ;
            TileID.Sets.Crimson                     [t] = def.ConversionData.IsCrimson                ;
            TileID.Sets.Falling                     [t] = def.IsFalling                               ;
            TileID.Sets.FramesOnKillWall            [t] = def.FrameData.FramesOnKillWall              ;
            TileID.Sets.GeneralPlacementTiles       [t] = def.PlacementData.IsGeneralPlacementTile    ;
            TileID.Sets.GrassSpecial                [t] = def.ConversionData.IsSpecialGrass           ;
            TileID.Sets.Hallow                      [t] = def.ConversionData.IsHallow                 ;
            TileID.Sets.HellSpecial                 [t] = def.ConversionData.IsSpecialHell            ;
            TileID.Sets.HousingWalls                [t] = def.HousingData.IsWall                      ;
            TileID.Sets.Ices                        [t] = def.SubtypeData.IsIce                       ;
            TileID.Sets.IcesSlush                   [t] = def.SubtypeData.IsIceSlush                  ;
            TileID.Sets.IcesSnow                    [t] = def.SubtypeData.IsIceSnow                   ;
            TileID.Sets.InteractibleByNPCs          [t] = def.NpcData.IsInteractible                  ;
            TileID.Sets.JungleSpecial               [t] = def.ConversionData.IsSpecialJungle          ;
            TileID.Sets.Leaves                      [t] = def.SubtypeData.IsLeaves                    ;
            TileID.Sets.Mud                         [t] = def.SubtypeData.IsMud                       ;
            TileID.Sets.NotReallySolid              [t] = def.CollisionData.NotReallySolid            ;
            TileID.Sets.Ore                         [t] = def.SubtypeData.IsOre                       ;
            TileID.Sets.Snow                        [t] = def.SubtypeData.IsSnow                      ;
            TileID.Sets.TouchDamageHot              [t] = def.CollisionData.DamageData.Hot            ;
            TileID.Sets.TouchDamageOther            [t] = def.CollisionData.DamageData.Other          ;
            TileID.Sets.TouchDamageSands            [t] = def.CollisionData.DamageData.Sand           ;
            TileID.Sets.TouchDamageVines            [t] = def.CollisionData.DamageData.Vines          ;

            TileID.Sets.Conversion.Grass       [t] = def.ConversionData.IsGrass       ;
            TileID.Sets.Conversion.HardenedSand[t] = def.ConversionData.IsHardenedSand;
            TileID.Sets.Conversion.Ice         [t] = def.ConversionData.IsIce         ;
            TileID.Sets.Conversion.Moss        [t] = def.ConversionData.IsMoss        ;
            TileID.Sets.Conversion.Sand        [t] = def.ConversionData.IsSand        ;
            TileID.Sets.Conversion.Sandstone   [t] = def.ConversionData.IsSandstone   ;
            TileID.Sets.Conversion.Stone       [t] = def.ConversionData.IsStone       ;
            TileID.Sets.Conversion.Thorn       [t] = def.ConversionData.IsThorn       ;

            if (def.HousingData.IsChair)
                LinqExt.Add(ref TileID.Sets.RoomNeeds.CountsAsChair, t);
            if (def.HousingData.IsDoor )
                LinqExt.Add(ref TileID.Sets.RoomNeeds.CountsAsDoor , t);
            if (def.HousingData.IsTable)
                LinqExt.Add(ref TileID.Sets.RoomNeeds.CountsAsTable, t);
            if (def.HousingData.IsLightSource)
                LinqExt.Add(ref TileID.Sets.RoomNeeds.CountsAsTorch, t);

            if ((def.OtherData = def.OtherData ?? GetDefaultObjData(def)) != null)
                TileObjectData._data.Add(def.OtherData);
        }

        protected override void PostLoad(Dictionary<string, TileDef> dict)
        {
            foreach (var def in dict.Values)
            {
                var arr = new bool[Main.tileSetsLoaded.Length];

                for (int i = 0; i < def.AestheticData.MergeWith.Count; i++)
                    arr[def.AestheticData.MergeWith[i].Resolve().Type] = true;

                Main.tileMerge[def.Type] = arr;
            }
        }
    }
}
