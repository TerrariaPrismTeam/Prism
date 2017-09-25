using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Prism.API.Defs
{
    public enum TileMineTool : byte
    {
        Pickaxe,
        Axe,
        Hammer
    }

    // sorry for the laziness
    static class AutoToString
    {
        internal static string ToStringMe(object o)
        {
            var t = o.GetType();
            var ps = t.GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

            return "{" + String.Join(",", ps.Select(p => p.Name + "=" + p.GetValue(o, null))) + "}";
        }
    }

    public class TileSubtypeData
    {
        public bool IsAlchemyTable
        {
            get;
            set;
        }
        public bool IsBrick
        {
            get;
            set;
        }
        public bool IsContainer
        {
            get;
            set;
        }
        public bool IsCraftingTable
        {
            get;
            set;
        }
        public bool IsDungeonTile
        {
            get;
            set;
        }
        public bool IsFlame
        {
            get;
            set;
        }
        public bool IsMoss
        {
            get;
            set;
        }
        public bool IsPile
        {
            get;
            set;
        }
        public bool IsRope
        {
            get;
            set;
        }
        public bool IsSand
        {
            get;
            set;
        }
        public bool IsSign
        {
            get;
            set;
        }
        public bool IsStone
        {
            get;
            set;
        }
        public bool IsIce
        {
            get;
            set;
        }
        public bool IsIceSlush
        {
            get;
            set;
        }
        public bool IsIceSnow
        {
            get;
            set;
        }
        public bool IsLeaves
        {
            get;
            set;
        }
        public bool IsMud
        {
            get;
            set;
        }
        public bool IsOre
        {
            get;
            set;
        }
        public bool IsSnow
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public class TileAestheticData
    {
        public bool BlendAll
        {
            get;
            set;
        }
        public bool MergesWithDirt
        {
            get;
            set;
        }
        public bool ChecksForMerge
        {
            get;
            set;
        }

        public List<TileRef> MergeWith
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileMineData
    {
        public TileMineTool MineTool
        {
            get;
            set;
        }

        public bool BreaksByCut
        {
            get;
            set;
        }
        public bool BreaksByWater
        {
            get;
            set;
        }
        public bool BreaksByLava
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public class TileConversionData
    {
        public bool IsCorrupt
        {
            get;
            set;
        }
        public bool IsCrimson
        {
            get;
            set;
        }
        public bool IsHallow
        {
            get;
            set;
        }
        public bool IsGrass
        {
            get;
            set;
        }
        public bool IsHardenedSand
        {
            get;
            set;
        }
        public bool IsIce
        {
            get;
            set;
        }
        public bool IsMoss
        {
            get;
            set;
        }
        public bool IsSand
        {
            get;
            set;
        }
        public bool IsSandstone
        {
            get;
            set;
        }
        public bool IsSpecialGrass
        {
            get;
            set;
        }
        public bool IsSpecialHell
        {
            get;
            set;
        }
        public bool IsSpecialJungle
        {
            get;
            set;
        }
        public bool IsStone
        {
            get;
            set;
        }
        public bool IsThorn
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileLightingData
    {
        public bool BlocksLight
        {
            get;
            set;
        }
        public bool BlocksSunlight
        {
            get;
            set;
        }
        public short GlowMask
        {
            get;
            set;
        }
        public int ShineChance
        {
            get;
            set;
        }
        public bool Shines
        {
            get;
            set;
        }
        public bool SpelunkerGlow
        {
            get;
            set;
        }
        public bool Glows
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileDamageData
    {
        public int Hot
        {
            get;
            set;
        }
        public int Sand
        {
            get;
            set;
        }
        public int Vines
        {
            get;
            set;
        }
        public int Other
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileCollisionData
    {
        public TileDamageData DamageData
        {
            get;
            set;
        }

        public bool IsBouncy
        {
            get;
            set;
        }
        public bool IsSolid
        {
            get;
            set;
        }
        public bool IsSolidTop
        {
            get;
            set;
        }
        public bool NotReallySolid
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TilePlacementData
    {
        public bool DisallowAttachingOtherTiles
        {
            get;
            set;
        }
        public bool BlocksStairs
        {
            get;
            set;
        }
        public bool BlocksStairsAbove
        {
            get;
            set;
        }
        public bool BreaksWhenPlacing
        {
            get;
            set;
        }
        public bool CanBeClearedDuringWorldGen
        {
            get;
            set;
        }
        public bool IsGeneralPlacementTile
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileFrameData
    {
        public bool FrameImportant
        {
            get;
            set;
        }
        public byte LargeFrames
        {
            get;
            set;
        }
        public bool FramesOnKillWall
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileHousingData
    {
        public bool IsWall
        {
            get;
            set;
        }
        public bool IsChair
        {
            get;
            set;
        }
        public bool IsDoor
        {
            get;
            set;
        }
        public bool IsTable
        {
            get;
            set;
        }
        public bool IsLightSource
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
    public struct TileNpcData
    {
        public bool IsAvoided
        {
            get;
            set;
        }
        public bool IsInteractible
        {
            get;
            set;
        }

        public override string ToString() { return AutoToString.ToStringMe(this); }
    }
}
