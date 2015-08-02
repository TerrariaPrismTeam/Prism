using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.API.Defs
{
    public class TileFrameConfig
    {
        /// <summary>
        /// The tile's per-tile frame width.
        /// </summary>
        public virtual int FrameWidth
        {
            get;
            set;
        }
        /// <summary>
        /// The tile's per-tile frame height.
        /// </summary>
        public virtual int FrameHeight
        {
            get;
            set;
        }
        /// <summary>
        /// The amount of tiles on the X axis this tile takes up.
        /// </summary>
        public virtual int TileWidth
        {
            get;
            set;
        }
        /// <summary>
        /// The amount of tiles on the Y axis this tile takes up.
        /// </summary>
        public virtual int TileHeight
        {
            get;
            set;
        }
        /// <summary>
        /// The draw offset of the tile on the Y axis. (Negative moves up, Positive moves down)
        /// </summary>
        public virtual int DrawOffsetY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not to save the tile's frames.
        /// </summary>
        public virtual bool FrameImportant
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the tile's spritesheet has frames along the X or Y axis.
        /// </summary>
        public virtual bool SheetYAligned
        {
            get;
            set;
        }

        /// <summary>
        /// The start frame for the tile's animation.
        /// </summary>
        public virtual int InitFrame
        {
            get;
            set;
        }
        /// <summary>
        /// The maximum frame of the tile.
        /// </summary>
        public virtual int MaxFrame
        {
            get;
            set;
        }
        /// <summary>
        /// The maximum for the frame counter of the tile's animation.
        /// </summary>
        public virtual int FrameCounterMax
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile is considered as using large frames
        /// </summary>
        public virtual bool LargeFrames
        {
            get;
            set;
        }
    }

    public class TileLightingConfig
    {
        /// <summary>
        /// Gets or sets whether or not this tile blocks light from passing through it.
        /// </summary>
        public virtual bool BlocksLight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile blocks sunlight from passing through it.
        /// </summary>
        public virtual bool BlocksSun
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile glows in the dark. Make this true if this tile uses the hook ModifyLight.
        /// </summary>
        public virtual bool Glows
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile spawns sparkle dust if it is properly lit.
        /// </summary>
        public virtual bool Shines
        {
            get;
            set;
        }
        /// <summary>
        /// The chance of the sparkle. (Higher numbers == less chance)
        /// </summary>
        public virtual int ShineChance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should glow if the player has Spelunker.
        /// </summary>
        public virtual bool SpelunkerGlow
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should glow if the player has Dangersense.
        /// </summary>
        public virtual bool DangersenseGlow
        {
            get;
            set;
        }
    }

    public class TileHousingConfig
    {
        /// <summary>
        /// Gets or sets whether or not this tile should be considered a table. (Used in NPC Housing)
        /// </summary>
        public virtual bool IsTable
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered a chair. (Used in NPC Housing)
        /// </summary>
        public virtual bool IsChair
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered a torch. (Used in NPC Housing)
        /// </summary>
        public virtual bool IsTorch
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should be considered a door. (Used in NPC Housing)
        /// </summary>
        public virtual bool IsDoor
        {
            get;
            set;
        }
    }

    public class TileMineConfig
    {
        /// <summary>
        /// The sound ID that this tile uses when it is killed.
        /// </summary>
        public virtual int Sound
        {
            get;
            set;
        }
        /// <summary>
        /// The soundGroup ID that this tile uses when it is killed.
        /// </summary>
        public virtual int SoundGroup
        {
            get;
            set;
        }

        /// <summary>
        /// The dust ID that this tile uses when it is mined or killed.
        /// </summary>
        public virtual int BreakDust
        {
            get;
            set;
        }

        /// <summary>
        /// The item this tile will drop when killed.
        /// </summary>
        public virtual ItemRef ItemDrop
        {
            get;
            set;
        }

        /// <summary>
        /// A multiplier for how fast the tile is mined by a pickaxe.
        /// </summary>
        public virtual float RatePick
        {
            get;
            set;
        }
        /// <summary>
        /// A multiplier for how fast the tile is mined by an axe.
        /// </summary>
        public virtual float RateAxe
        {
            get;
            set;
        }
        /// <summary>
        /// A multiplier for how fast the tile is mined by a hammer.
        /// </summary>
        public virtual float RateHammer
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum pick value required to mine this tile.
        /// </summary>
        public virtual int MinPick
        {
            get;
            set;
        }
        /// <summary>
        /// The minimum axe value required to mine this tile.
        /// </summary>
        public virtual int MinAxe
        {
            get;
            set;
        }
        /// <summary>
        /// The minimum hammer value required to mine this tile.
        /// </summary>
        public virtual int MinHammer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not a tile breaks immediately when hit.
        /// </summary>
        public virtual bool BreaksInstantly
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not a tile breaks from a pickaxe.
        /// </summary>
        public virtual bool BreaksByPick
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not a tile breaks from an axe.
        /// </summary>
        public virtual bool BreaksByAxe
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not a tile breaks from a hammer.
        /// </summary>
        public virtual bool BreaksByHammer
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not a tile breaks when hit by a melee weapon or projectile.
        /// </summary>
        public virtual bool BreaksByCut
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not a tile breaks when submerged in water or honey.
        /// </summary>
        public virtual bool BreaksByWater
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not a tile breaks when submerged in lava.
        /// </summary>
        public virtual bool BreaksByLava
        {
            get;
            set;
        }
    }

    public class TilePlaceConfig
    {
        /// <summary>
        /// Gets or sets whether or not this tile should place the first or second frame based on player direction.
        /// </summary>
        public virtual bool Directional
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this tile should check tile placement if walls behind it are broken.
        /// </summary>
        public virtual bool CheckWalls
        {
            get;
            set;
        }

        /// <summary>
        /// The specific X & Y frame of the tile to use when it is placed.
        /// </summary>
        public virtual Point PlacementFrame
        {
            get;
            set;
        }
        /// <summary>
        /// A preset condition used to determine if a tile can be placed or can stay in place.
        /// </summary>
        public virtual PlacementConditions PlacementConditions
        {
            get;
            set;
        }
        /// <summary>
        /// The tile within the tile that is considered the placement tile. (The tile the mouse is over when placing)
        /// </summary>
        public virtual Point PlacementOrigin
        {
            get;
            set;
        }
        /// <summary>
        /// Causes this tile to not be 'attachable' by other tiles. In other words, a tile that needs a placement condition checking this tile will always return false for this tile.
        /// </summary>
        public virtual bool NoAttach
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not this tile should merge with dirt.
        /// </summary>
        public virtual bool MergesWithDirt
        {
            get;
            set;
        }
    }

    public enum PlacementConditions
    {
        Air,
        FlatCeiling,
        FlatCeilingSolid,
        FlatGround,
        FlatGroundSolid,
        PlaceTouching,
        PlaceTouchingSolid,
        Side,
        Wall
    }
}
