using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;

namespace Prism.API.Defs
{

    public partial class TileDef : EntityDef<TileBehaviour, Tile>
    {
        /* TODO:
         * - Add TileAdj
         * - Add TileMerge
         * - Improve Tile Drop JSON read
         */

        /// <summary>
        /// Gets or sets the tile's texture function.
        /// </summary>
        public virtual Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets this tile's housing configuration.
        /// </summary>
        public virtual TileHousingConfig HousingConfig
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this tile's mining configuration.
        /// </summary>
        public virtual TileMineConfig MineConfig
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this tile's placement configuration.
        /// </summary>
        public virtual TilePlaceConfig PlaceConfig
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this tile's lighting configuration.
        /// </summary>
        public virtual TileLightingConfig LightingConfig
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this tile's lighting configuration.
        /// </summary>
        public virtual TileFrameConfig FrameConfig
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not a tile has collision.
        /// </summary>
        public virtual bool Solid
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not the player can walk on this tile if it's non-solid. (such as tables, bookcases, etc.)
        /// </summary>
        public virtual bool SolidTop
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile acts as a storage container
        /// </summary>
        public virtual bool Chest
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should behave similarly to a rope.
        /// </summary>
        public virtual bool Rope
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered part of the dungeon.
        /// </summary>
        public virtual bool TileDungeon
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered a viable spawn point.
        /// </summary>
        public virtual bool Spawn
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile is unaffected by explosions
        /// </summary>
        public virtual bool ExplosionResistant
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile can be sloped
        /// </summary>
        public virtual bool Slopable
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        public virtual bool NoFail
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>obsidianKill</remarks>
        public virtual bool ObsidianKill
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should behave similarly to a platform
        /// </summary>
        public virtual bool Platform
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should behave similarly to a pile
        /// </summary>
        public virtual bool Pile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered bricks.
        /// </summary>
        public virtual bool Brick
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered moss.
        /// </summary>
        public virtual bool Moss
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered stone.
        /// </summary>
        public virtual bool Stone
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not the tile should be considered grass frame-wise.
        /// </summary>
        public virtual bool Grass
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered sand. (does not grant gravity to the tile)
        /// </summary>
        public virtual bool TileSand
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile has a vanilla 'flame texture'. Almost all custom tiles will have this as false.
        /// </summary>
        public virtual bool TileFlame
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile is considered a potion herb. Does nothing except change the default sound the tile makes.
        /// </summary>
        public virtual bool AlchemyFlower
        {
            get;
            set;
        }

        /// <summary>
        /// The color the tile uses on the map.
        /// </summary>
        public virtual Color MapColor
        {
            get;
            set;
        }

        /// <summary>
        /// The text used when the tile is hovered over.
        /// </summary>
        public virtual string MapHoverText
        {
            get;
            set;
        }

        public TileDef(string displayName, Func<TileBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? (() => null);
        }
        public TileDef(string displayName, JsonData json, Func<TileBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            DisplayName = displayName;
            GetTexture = getTexture ?? (() => null);

            FrameConfig.TileWidth = json.Has("width") ? (int)json["width"] : 1;
            FrameConfig.TileHeight = json.Has("height") ? (int)json["height"] : 1;

            if (json.Has("size") && json["size"].IsArray && json["size"].Count >= 2 && json["size"][0].IsInt && json["size"][1].IsInt)
            {
                FrameConfig.TileWidth = (int)json["size"][0];
                FrameConfig.TileHeight = (int)json["size"][1];
            }

            FrameConfig.FrameWidth = json.Has("frameWidth") ? (int)json["frameWidth"] : 16;
            FrameConfig.FrameHeight = json.Has("frameHeight") ? (int)json["frameHeight"] : 16;

            FrameConfig.DrawOffsetY = (int)json["drawOffsetY"];

            Solid = (bool)json["solid"];
            SolidTop = (bool)json["solidTop"];
            FrameConfig.FrameImportant = (bool)json["frameImportant"];
            PlaceConfig.Directional = (bool)json["directional"];

            if (FrameConfig.FrameImportant)
                PlaceConfig.PlacementFrame = new Point((int)json["placementFrameX"], (int)json["placementFrameY"]);

            if (json.Has("placementConditions"))
            {
                switch (((string)json["placementConditions"]).ToLower())
                {
                    case "air":
                        PlaceConfig.PlacementConditions = PlacementConditions.Air;
                        break;
                    case "wall":
                        PlaceConfig.PlacementConditions = PlacementConditions.Wall;
                        break;
                    case "placetouching":
                        PlaceConfig.PlacementConditions = PlacementConditions.PlaceTouching;
                        break;
                    case "placetouchingsolid":
                        PlaceConfig.PlacementConditions = PlacementConditions.PlaceTouchingSolid;
                        break;
                    case "side":
                        PlaceConfig.PlacementConditions = PlacementConditions.Side;
                        break;
                    case "flatground":
                        PlaceConfig.PlacementConditions = PlacementConditions.FlatGround;
                        break;
                    case "flatgroundsolid":
                        PlaceConfig.PlacementConditions = PlacementConditions.FlatGroundSolid;
                        break;
                    case "flatceiling":
                        PlaceConfig.PlacementConditions = PlacementConditions.FlatCeiling;
                        break;
                    case "flatceilingsolid":
                        PlaceConfig.PlacementConditions = PlacementConditions.FlatCeilingSolid;
                        break;
                }
            }
            else if (FrameConfig.TileWidth == 1 && FrameConfig.TileHeight == 1)
                PlaceConfig.PlacementConditions = PlacementConditions.PlaceTouchingSolid;
            else
                PlaceConfig.PlacementConditions = PlacementConditions.FlatGroundSolid;

            PlaceConfig.CheckWalls = PlaceConfig.PlacementConditions == PlacementConditions.Wall;

            // Only update checkWalls if JSON property exists
            // so as to not overwrite assignment from PlacementConditions
            // check above.
            if (json.Has("checkWalls"))
                PlaceConfig.CheckWalls = (bool)json["checkWalls"];

            if (json.Has("placementOrigin") && json["placementOrigin"].IsArray && json["placementOrigin"].Count >= 2 && json["placementOrigin"][0].IsInt && json["placementOrigin"][1].IsInt)
            {

                Point value = new Point((int)json["placementOrigin"][0], (int)json["placementOrigin"][1]);

                if (value.X < 0)
                    value.X = 0;

                if (value.Y < 0)
                    value.Y = 0;

                if (value.X >= FrameConfig.TileWidth)
                    value.X = FrameConfig.TileWidth - 1;

                if (value.Y >= FrameConfig.TileHeight)
                    value.Y = FrameConfig.TileHeight - 1;

                PlaceConfig.PlacementOrigin = value;

            }

            FrameConfig.SheetYAligned = (bool)json["sheetYAligned"];
            MineConfig.BreaksInstantly = (bool)json["breaksFast"];
            MineConfig.BreaksByPick = (bool)json["breaksByPick"];
            MineConfig.BreaksByAxe = (bool)json["breaksByAxe"];
            MineConfig.BreaksByHammer = (bool)json["breaksByHammer"];
            MineConfig.BreaksByCut = (bool)json["breaksByCut"];
            MineConfig.BreaksByWater = (bool)json["breaksByWater"];
            MineConfig.BreaksByLava = (bool)json["breaksByLava"];

            if (MineConfig.BreaksByLava)
                ObsidianKill = true;

            MineConfig.MinPick = (int)json["minPick"];
            MineConfig.MinAxe = (int)json["minAxe"];
            MineConfig.MinHammer = (int)json["minHammer"];

            MineConfig.RatePick = (float)json["ratePick"];
            MineConfig.RateAxe = (float)json["rateAxe"];
            MineConfig.RateHammer = (float)json["rateHammer"];

            HousingConfig.IsTable = (bool)json["table"];
            HousingConfig.IsChair = (bool)json["chair"];
            HousingConfig.IsTorch = (bool)json["torch"];
            HousingConfig.IsDoor = (bool)json["door"];
            Chest = (bool)json["chest"];
            Rope = (bool)json["rope"];
            PlaceConfig.NoAttach = (bool)json["noAttach"];
            TileDungeon = (bool)json["tileDungeon"];
            Spawn = (bool)json["spawn"];
            ExplosionResistant = (bool)json["explosionResistant"];

            //TODO: AdjTile Resolver

            Slopable = (bool)json["slopable"];
            NoFail = (bool)json["noFail"];
            ObsidianKill = (bool)json["obsidianKill"];
            LightingConfig.BlocksLight = (bool)json["blocksLight"];
            LightingConfig.BlocksSun = (bool)json["blocksSun"];
            LightingConfig.Glows = (bool)json["glows"];
            LightingConfig.Shines = (bool)json["shines"];

            LightingConfig.ShineChance = (int)json["shineChance"];
            FrameConfig.InitFrame = (int)json["frame"];
            FrameConfig.MaxFrame = (int)json["frameMax"];
            FrameConfig.FrameCounterMax = (int)json["frameCounterMax"];

            LightingConfig.SpelunkerGlow = (bool)json["treasure"];
            LightingConfig.DangersenseGlow = (bool)json["danger"];
            Platform = (bool)json["platform"];
            Pile = (bool)json["pile"];
            Brick = (bool)json["brick"];
            Moss = (bool)json["moss"];
            Stone = (bool)json["stone"];
            Grass = (bool)json["grass"];
            PlaceConfig.MergesWithDirt = (bool)json["mergeDirt"];
            FrameConfig.LargeFrames = (bool)json["largeFrames"];
            TileSand = (bool)json["tileSand"];
            TileFlame = (bool)json["tileFlame"];
            AlchemyFlower = (bool)json["alchemyFlower"];

            MineConfig.Sound = (int)json["sound"];
            MineConfig.SoundGroup = (int)json["soundGroup"];
            MineConfig.BreakDust = (int)json["dust"];

            MapColor = json.Has("mapColor") && json["mapColor"].IsArray && json["mapColor"].Count >= 3 ?
                new Color(((byte)((int)json["mapColor"][0])), ((byte)((int)json["mapColor"][1])), ((byte)((int)json["mapColor"][2])), ((json["mapColor"].Count <= 3) ? 255 : ((byte)((int)json["mapColor"][3])))) :
                Color.Pink;

            MapHoverText = json.Has("mapHoverText") ? (string)json["mapHoverText"] : DisplayName;

            if (json["drop"].IsInt)
                MineConfig.ItemDrop = new ItemRef((int)json["drop"]);
            else
                MineConfig.ItemDrop = new ItemRef((string)json["drop"] /* mod name? */);

            //TODO: Tile Merge Resolver

        }

        public static implicit operator TileRef(TileDef  def)
        {
            return new TileRef(def.InternalName, def.Mod.InternalName);
        }
        public static explicit operator TileDef(TileRef @ref)
        {
            return @ref.Resolve();
        }
    }
}
