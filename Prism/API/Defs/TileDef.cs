using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;
using LitJson;


namespace Prism.API.Defs {

    public class TileDef : EntityDef<TileBehaviour, Tile> {

        // TODO
        // Add TileAdj
        // Add TileMerge
        // Improve Tile Drop JSON read

        /// <summary>
        /// Gets TileDefs by their type number.
        /// </summary>
        public struct ByTypeIndexer {
            public TileDef this[int type] {
                get {
                    return Handler.TileDef.DefsByType[type];
                }
            }
        }

        /// <summary>
        /// Gets TileDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameIndexer {
            public TileDef this[string tileInternalName, string modInternalName = null] {
                get {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == PrismApi.VanillaString || modInternalName == PrismApi.TerrariaString)
                        return Handler.TileDef.VanillaDefsByName[tileInternalName];

                    return ModData.ModsFromInternalName[modInternalName].TileDefs[tileInternalName];
                }
            }
        }

        /// <summary>
        /// Gets TileDefs by their type number.
        /// </summary>
        public static ByTypeIndexer ByType {
            get {
                return new ByTypeIndexer();
            }
        }
        /// <summary>
        /// Gets TileDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameIndexer ByName {
            get {
                return new ByNameIndexer();
            }
        }

        /// <summary>
        /// Gets or sets the tile's texture function.
        /// </summary>
        public virtual Func<Texture2D> GetTexture {
            get;
            set;
        }

        #region Fields

        /// <summary>
        /// A multiplier for how fast the tile is mined by a pickaxe.
        /// </summary>
        public virtual float RatePick {
            get;
            set;
        }

        /// <summary>
        /// A multiplier for how fast the tile is mined by an axe.
        /// </summary>
        public virtual float RateAxe {
            get;
            set;
        }

        /// <summary>
        /// A multiplier for how fast the tile is mined by a hammer.
        /// </summary>
        public virtual float RateHammer {
            get;
            set;
        }

        /// <summary>
        /// The minimum pick value required to mine this tile.
        /// </summary>
        public virtual int MinPick {
            get;
            set;
        }

        /// <summary>
        /// The minimum axe value required to mine this tile.
        /// </summary>
        public virtual int MinAxe {
            get;
            set;
        }

        /// <summary>
        /// The minimum hammer value required to mine this tile.
        /// </summary>
        public virtual int MinHammer {
            get;
            set;
        }

        /// <summary>
        /// The tile's per-tile frame width.
        /// </summary>
        public virtual int FrameWidth {
            get;
            set;
        }

        /// <summary>
        /// The tile's per-tile frame height.
        /// </summary>
        public virtual int FrameHeight {
            get;
            set;
        }

        /// <summary>
        /// The amount of tiles on the X axis this tile takes up.
        /// </summary>
        public virtual int Width {
            get;
            set;
        }

        /// <summary>
        /// The amount of tiles on the Y axis this tile takes up.
        /// </summary>
        public virtual int Height {
            get;
            set;
        }

        /// <summary>
        /// The draw offset of the tile on the Y axis. (Negative moves up, Positive moves down)
        /// </summary>
        public virtual int DrawOffsetY {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile has collision.
        /// </summary>
        public virtual bool Solid {
            get;
            set;
        }

        /// <summary>
        /// Whether or not the player can walk on this tile if it's non-solid. (such as tables, bookcases, etc.)
        /// </summary>
        public virtual bool SolidTop {
            get;
            set;
        }

        /// <summary>
        /// Whether or not to save the tile's frames.
        /// </summary>
        public virtual bool FrameImportant {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should place the first or second frame based on player direction.
        /// </summary>
        public virtual bool Directional {
            get;
            set;
        }

        /// <summary>
        /// The specific X & Y frame of the tile to use when it is placed.
        /// </summary>
        public virtual Point PlacementFrame {
            get;
            set;
        }

        /// <summary>
        /// A preset condition used to determine if a tile can be placed or can stay in place.
        /// </summary>
        public virtual PlacementConditions PlacementConditions {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should check tile placement if walls behind it are broken.
        /// </summary>
        public virtual bool CheckWalls {
            get;
            set;
        }

        /// <summary>
        /// The tile within the tile that is considered the placement tile. (The tile the mouse is over when placing)
        /// </summary>
        public virtual Point PlacementOrigin {
            get;
            set;
        }

        /// <summary>
        /// Whether the tile's spritesheet has frames along the X or Y axis.
        /// </summary>
        public virtual bool SheetYAligned {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks immediately when hit.
        /// </summary>
        public virtual bool BreaksFast {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks from a pickaxe.
        /// </summary>
        public virtual bool BreaksByPick {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks from an axe.
        /// </summary>
        public virtual bool BreaksByAxe {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks from a hammer.
        /// </summary>
        public virtual bool BreaksByHammer {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks when hit by a melee weapon or projectile.
        /// </summary>
        public virtual bool BreaksByCut {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks when submerged in water or honey.
        /// </summary>
        public virtual bool BreaksByWater {
            get;
            set;
        }

        /// <summary>
        /// Whether or not a tile breaks when submerged in lava.
        /// </summary>
        public virtual bool BreaksByLava {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered a table. (Used in NPC Housing)
        /// </summary>
        public virtual bool Table {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered a chair. (Used in NPC Housing)
        /// </summary>
        public virtual bool Chair {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered a torch. (Used in NPC Housing)
        /// </summary>
        public virtual bool Torch {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered a door. (Used in NPC Housing)
        /// </summary>
        public virtual bool Door {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile acts as a storage container
        /// </summary>
        public virtual bool Chest {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should behave similarly to a rope.
        /// </summary>
        public virtual bool Rope {
            get;
            set;
        }

        /// <summary>
        /// Causes this tile to not be 'attachable' by other tiles. In other words, a tile that needs a placement condition checking this tile will always return false for this tile.
        /// </summary>
        public virtual bool NoAttach {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered part of the dungeon.
        /// </summary>
        public virtual bool TileDungeon {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered a viable spawn point.
        /// </summary>
        public virtual bool Spawn {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile is unaffected by explosions
        /// </summary>
        public virtual bool ExplosionResistant {
            get;
            set;
        }

        /// <summary>
        /// The tile position within this tile the player should respawn at.
        /// </summary>
        public virtual Point SpawnAt {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile can be sloped
        /// </summary>
        public virtual bool Slopable {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool NoFail {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool ObsidianKill {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile blocks light from passing through it.
        /// </summary>
        public virtual bool BlocksLight {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile blocks sunlight from passing through it.
        /// </summary>
        public virtual bool BlocksSun {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile glows in the dark. Make this true if this tile uses the hook ModifyLight.
        /// </summary>
        public virtual bool Glows {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile spawns sparkle dust if it is properly lit.
        /// </summary>
        public virtual bool Shines {
            get;
            set;
        }

        /// <summary>
        /// The chance of the sparkle. (Higher numbers == less chance)
        /// </summary>
        public virtual int ShineChance {
            get;
            set;
        }

        /// <summary>
        /// The start frame for the tile's animation.
        /// </summary>
        public virtual int Frame {
            get;
            set;
        }

        /// <summary>
        /// The maximum frame of the tile.
        /// </summary>
        public virtual int FrameMax {
            get;
            set;
        }

        /// <summary>
        /// The maximum for the frame counter of the tile's animation.
        /// </summary>
        public virtual int FrameCounterMax {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should glow if the player has Spelunker.
        /// </summary>
        public virtual bool Treasure {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should glow if the player has Dangersense.
        /// </summary>
        public virtual bool Danger {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should behave similarly to a platform
        /// </summary>
        public virtual bool Platform {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should behave similarly to a pile
        /// </summary>
        public virtual bool Pile {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered bricks.
        /// </summary>
        public virtual bool Brick {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered moss.
        /// </summary>
        public virtual bool Moss {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered stone.
        /// </summary>
        public virtual bool Stone {
            get;
            set;
        }

        /// <summary>
        /// Whether or not the tile should be considered grass frame-wise.
        /// </summary>
        public virtual bool Grass {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should merge with dirt.
        /// </summary>
        public virtual bool MergeDirt {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile is considered as using large frames
        /// </summary>
        public virtual bool LargeFrames {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile should be considered sand. (does not grant gravity to the tile)
        /// </summary>
        public virtual bool TileSand {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile has a vanilla 'flame texture'. Almost all custom tiles will have this as false.
        /// </summary>
        public virtual bool TileFlame {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this tile is considered a potion herb. Does nothing except change the default sound the tile makes.
        /// </summary>
        public virtual bool AlchemyFlower {
            get;
            set;
        }

        /// <summary>
        /// The sound ID that this tile uses when it is killed.
        /// </summary>
        public virtual int Sound {
            get;
            set;
        }

        /// <summary>
        /// The soundGroup ID that this tile uses when it is killed.
        /// </summary>
        public virtual int SoundGroup {
            get;
            set;
        }

        /// <summary>
        /// The dust ID that this tile uses when it is mined or killed.
        /// </summary>
        public virtual int Dust {
            get;
            set;
        }

        /// <summary>
        /// The color the tile uses on the map.
        /// </summary>
        public virtual Color MapColor {
            get;
            set;
        }

        /// <summary>
        /// The text used when the tile is hovered over.
        /// </summary>
        public virtual string MapHoverText {
            get;
            set;
        }

        /// <summary>
        /// The item this tile will drop when killed.
        /// </summary>
        public virtual int Drop {
            get;
            set;
        }

        #endregion Fields
        
        //Don't use this constructor to create modded tiles pls thx
        public TileDef() : this("?TileName?") { }        

        public TileDef(
            #region arguments
            string displayName,

            int width               = 1,
            int height              = 1,

            bool alchemyFlower      = false,
            bool blocksLight        = false,
            bool blocksSun          = false,
            bool breaksByAxe        = false,
            bool breaksByCut        = false,
            bool breaksByHammer     = false,
            bool breaksByLava       = false,
            bool breaksByPick       = false,
            bool breaksByWater      = false,
            bool breaksFast         = false,
            bool brick              = false,
            bool chair              = false,
            bool checkWalls         = false,
            bool chest              = false,
            bool danger             = false,
            bool directional        = false,
            bool door               = false,
            int drawOffsetY         = 0,
            int drop                = 0,
            int dust                = 0,
            bool explosionResistant = false,
            int frame               = 0,
            int frameCounterMax     = 0,
            int frameHeight         = 16,
            bool frameImportant     = false,
            int frameMax            = 0,
            int frameWidth          = 16,
            Func<Texture2D> getTex  = null,
            bool glows              = false,
            bool grass              = false,
            bool largeFrames        = false,
            Color mapColor          = default(Color),
            string mapHoverText     = "",
            bool mergeDirt          = false,
            int minAxe              = 0,
            int minHammer           = 0,
            int minPick             = 0,
            bool moss               = false,
            bool noAttach           = false,
            bool noFail             = false,
            bool obsidianKill       = false,
            bool pile               = false,
            PlacementConditions placementConditions = PlacementConditions.Air,
            Point placementFrame    = default(Point),
            Point placementOrigin   = default(Point),
            bool platform           = false,
            float rateAxe           = 1f,
            float rateHammer        = 1f,
            float ratePick          = 1f,
            bool rope               = false,
            bool sheetYAligned      = false,
            int shineChance         = 0,
            bool shines             = false,
            bool slopable           = false,
            bool solid              = true,
            bool solidTop           = false,
            int sound               = 0,
            int soundGroup          = 0,
            bool spawn              = false,
            Point spawnAt           = default(Point),
            bool stone              = false,
            bool table              = false,
            bool tileDungeon        = false,
            bool tileFlame          = false,
            bool tileSand           = false,
            bool torch              = false,
            bool treasure           = false
            #endregion
        ) {
            DisplayName         = displayName;
            Width               = width;
            Height              = height;

            AlchemyFlower       = alchemyFlower;
            BlocksLight         = blocksLight;
            BlocksSun           = blocksSun;
            BreaksByAxe         = breaksByAxe;
            BreaksByCut         = breaksByCut;
            BreaksByHammer      = breaksByHammer;
            BreaksByLava        = breaksByLava;
            BreaksByPick        = breaksByPick;
            BreaksByWater       = breaksByWater;
            BreaksFast          = breaksFast;
            Brick               = brick;
            Chair               = chair;
            Chest               = chest;
            CheckWalls          = checkWalls;
            Danger              = danger;
            Directional         = directional;
            Door                = door;
            DrawOffsetY         = drawOffsetY;
            Drop                = drop;
            Dust                = dust;
            ExplosionResistant  = explosionResistant;
            Frame               = frame;
            FrameCounterMax     = frameCounterMax;
            FrameHeight         = frameHeight;
            FrameImportant      = frameImportant;
            FrameMax            = frameMax;
            FrameWidth          = frameWidth;
            GetTexture          = getTex ?? (() => null);
            Glows               = glows;
            Grass               = grass;
            LargeFrames         = largeFrames;
            MapColor            = mapColor;
            MapHoverText        = mapHoverText;
            MergeDirt           = mergeDirt;
            MinAxe              = minAxe;
            MinHammer           = minHammer;
            MinPick             = minPick;
            Moss                = moss;
            NoAttach            = noAttach;
            NoFail              = noFail;
            ObsidianKill        = obsidianKill;
            Pile                = pile;
            PlacementConditions = placementConditions;
            PlacementFrame      = placementFrame;
            PlacementOrigin     = placementOrigin;
            Platform            = platform;
            RateAxe             = rateAxe;
            RateHammer          = rateHammer;
            RatePick            = ratePick;
            Rope                = rope;
            SheetYAligned       = sheetYAligned;
            ShineChance         = shineChance;
            Shines              = shines;
            Slopable            = slopable;
            Solid               = solid;
            SolidTop            = solidTop;
            Sound               = sound;
            SoundGroup          = soundGroup;
            Spawn               = spawn;
            SpawnAt             = spawnAt;
            Stone               = stone;
            Table               = table;
            TileDungeon         = tileDungeon;
            TileFlame           = tileFlame;
            TileSand            = tileSand;
            Torch               = torch;
            Treasure            = treasure;

        }

        public TileDef(string displayName, JsonData json, Func<Texture2D> getTex = null) {

            DisplayName = displayName;
            GetTexture  = getTex ?? (() => null);

            Width  = json.Has("width")  ? (int)json["width"]  : 1;
            Height = json.Has("height") ? (int)json["height"] : 1;

            if (json.Has("size") && json["size"].IsArray && json["size"].Count >= 2 && json["size"][0].IsInt && json["size"][1].IsInt) {
                Width  = (int)json["size"][0];
                Height = (int)json["size"][1];
            }

            FrameWidth     = json.Has("frameWidth")  ? (int)json["frameWidth"]  : 16;
            FrameHeight    = json.Has("frameHeight") ? (int)json["frameHeight"] : 16;

            DrawOffsetY    = (int)json["drawOffsetY"];

            Solid          = (bool)json["solid"];
            SolidTop       = (bool)json["solidTop"];
            FrameImportant = (bool)json["frameImportant"];
            Directional    = (bool)json["directional"];

            if (FrameImportant)
                PlacementFrame  = new Point((int)json["placementFrameX"], (int)json["placementFrameY"]);

            if (json.Has("placementConditions")) {

                switch (((string)json["placementConditions"]).ToLower()) {
                    
                    case "air"                : PlacementConditions = PlacementConditions.Air;                break;
                    case "wall"               : PlacementConditions = PlacementConditions.Wall;               break;
                    case "placetouching"      : PlacementConditions = PlacementConditions.PlaceTouching;      break;
                    case "placetouchingsolid" : PlacementConditions = PlacementConditions.PlaceTouchingSolid; break;
                    case "side"               : PlacementConditions = PlacementConditions.Side;               break;
                    case "flatground"         : PlacementConditions = PlacementConditions.FlatGround;         break;
                    case "flatgroundsolid"    : PlacementConditions = PlacementConditions.FlatGroundSolid;    break;
                    case "flatceiling"        : PlacementConditions = PlacementConditions.FlatCeiling;        break;
                    case "flatceilingsolid"   : PlacementConditions = PlacementConditions.FlatCeilingSolid;   break;

                }

            } else {

                if (Width == 1 && Height == 1) {
                    PlacementConditions = PlacementConditions.PlaceTouchingSolid;
                } else {
                    PlacementConditions = PlacementConditions.FlatGroundSolid;
                }

            }

            if (PlacementConditions == PlacementConditions.Wall)
                CheckWalls = true;
            else
                CheckWalls = false;

            // Only update checkWalls if JSON property exists
            // so as to not overwrite assignment from PlacementConditions
            // check above.
            if (json.Has("checkWalls"))
                CheckWalls = (bool)json["checkWalls"];

            if (json.Has("placementOrigin") && json["placementOrigin"].IsArray && json["placementOrigin"].Count >= 2 && json["placementOrigin"][0].IsInt && json["placementOrigin"][1].IsInt) {

                Point value = new Point((int)json["placementOrigin"][0], (int)json["placementOrigin"][1]);

                if (value.X < 0)
                    value.X = 0;

                if (value.Y < 0)
                    value.Y = 0;

                if (value.X >= Width)
                    value.X = Width - 1;

                if (value.Y >= Height)
                    value.Y = Height - 1;

                PlacementOrigin = value;

            }

            SheetYAligned  = (bool)json["sheetYAligned"];
            BreaksFast     = (bool)json["breaksFast"];
            BreaksByPick   = (bool)json["breaksByPick"];
            BreaksByAxe    = (bool)json["breaksByAxe"];
            BreaksByHammer = (bool)json["breaksByHammer"];
            BreaksByCut    = (bool)json["breaksByCut"];
            BreaksByWater  = (bool)json["breaksByWater"];
            BreaksByLava   = (bool)json["breaksByLava"];

            if (BreaksByLava)
                ObsidianKill = true;

            MinPick     = (int)json["minPick"];
            MinAxe      = (int)json["minAxe"];
            MinHammer   = (int)json["minHammer"];

            RatePick    = (float)json["ratePick"];
            RateAxe     = (float)json["rateAxe"];
            RateHammer  = (float)json["rateHammer"];

            Table       = (bool)json["table"];
            Chair       = (bool)json["chair"];
            Torch       = (bool)json["torch"];
            Door        = (bool)json["door"];
            Chest       = (bool)json["chest"];
            Rope        = (bool)json["rope"];
            NoAttach    = (bool)json["noAttach"];
            TileDungeon = (bool)json["tileDungeon"];
            Spawn       = (bool)json["spawn"];
            ExplosionResistant = (bool)json["explosionResistant"];

            if (json.Has("spawnAt") && json["spawnAt"].IsArray && json["spawnAt"].Count >= 2 && json["spawnAt"][0].IsInt && json["spawnAt"][1].IsInt)
                SpawnAt = new Point((int)json["spawnAt"][0], (int)json["spawnAt"][1]);

            // TODO AdjTile Resolver
            //if (json.Has("adjTile"))
            //    ResolverQueue.Add(new AdjTileResolver(modBase, tileID, TileDef.byType[tileID], json["adjTile"]));

            Slopable        = (bool)json["slopable"];
            NoFail          = (bool)json["noFail"];
            ObsidianKill    = (bool)json["obsidianKill"];
            BlocksLight     = (bool)json["blocksLight"];
            BlocksSun       = (bool)json["blocksSun"];
            Glows           = (bool)json["glows"];
            Shines          = (bool)json["shines"];

            ShineChance     = (int)json["shineChance"];
            Frame           = (int)json["frame"];
            FrameMax        = (int)json["frameMax"];
            FrameCounterMax = (int)json["frameCounterMax"];

            Treasure        = (bool)json["treasure"];
            Danger          = (bool)json["danger"];
            Platform        = (bool)json["platform"];
            Pile            = (bool)json["pile"];
            Brick           = (bool)json["brick"];
            Moss            = (bool)json["moss"];
            Stone           = (bool)json["stone"];
            Grass           = (bool)json["grass"];
            MergeDirt       = (bool)json["mergeDirt"];
            LargeFrames     = (bool)json["largeFrames"];
            TileSand        = (bool)json["tileSand"];
            TileFlame       = (bool)json["tileFlame"];
            AlchemyFlower   = (bool)json["alchemyFlower"];

            Sound           = (int)json["sound"];
            SoundGroup      = (int)json["soundGroup"];
            Dust            = (int)json["dust"];

            MapColor = json.Has("mapColor") && json["mapColor"].IsArray && json["mapColor"].Count >= 3 ?
                new Color(((byte)((int)json["mapColor"][0])), ((byte)((int)json["mapColor"][1])), ((byte)((int)json["mapColor"][2])), ((json["mapColor"].Count <= 3) ? 255 : ((byte)((int)json["mapColor"][3])))) :
                Color.Pink;
            
            MapHoverText = json.Has("mapHoverText") ? (string)json["mapHoverText"] : DisplayName;

            // TODO Fix Tile Drop to get type from strings as well
            Drop = (int)json["drop"]; 
            
            // TODO Tile Merge Resolver
            //if (json.Has("tileMerge"))
            //    ResolverQueue.Add(new TileMergeResolver(modBase, TileDef.byType[tileID], tileID, jsonData4));

        }
        
        public static implicit operator TileRef(TileDef def) {
            return new TileRef(def.InternalName, def.Mod.InternalName);
        }
        
        public static explicit operator TileDef(TileRef @ref) {
            return @ref.Resolve();
        }

    }

}