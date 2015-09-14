using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.API.Defs
{
    public enum WallLargeFrameKind : byte
    {
        None,
        //TODO: what do these do?
        PhlebasMethod,
        LazureMethod
    }
    public struct WallConversionData : IEquatable<WallConversionData>
    {
        public bool IsGrass, IsHardenedSand, IsSandstone, IsStone;

        public WallConversionData(bool grass, bool hSand, bool sStone, bool stone)
        {
            IsGrass = grass;
            IsHardenedSand = hSand;
            IsSandstone = sStone;
            IsStone = stone;
        }

        public bool Equals(WallConversionData other)
        {
            return IsGrass == other.IsGrass && IsHardenedSand == other.IsHardenedSand && IsSandstone == other.IsSandstone && IsStone == other.IsStone;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is WallConversionData)
                return Equals((WallConversionData)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return IsGrass.GetHashCode() | IsHardenedSand.GetHashCode() << 1 | IsSandstone.GetHashCode() << 2 | IsStone.GetHashCode() << 3;
        }
        public override string ToString()
        {
            return "{IsGrass=" + IsGrass + ", IsHardenedSand=" + IsHardenedSand + ", IsSandstone=" + IsSandstone + ", IsStone=" + IsStone + "}";
        }
    }
}
