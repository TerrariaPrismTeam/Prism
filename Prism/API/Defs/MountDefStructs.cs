using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Prism.API.Defs
{
    public struct MountFrameData : IEquatable<MountFrameData>
    {
        public readonly static MountFrameData None = new MountFrameData(0, 0, 0);

        public readonly int Count, Delay, Start;

        public MountFrameData(int count, int delay, int start)
        {
            Count = count;
            Delay = delay;
            Start = start;
        }

        public bool Equals(MountFrameData other)
        {
            return Count == other.Count && Delay == other.Delay && Start == other.Start;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is MountFrameData)
                return Equals((MountFrameData)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Count.GetHashCode() + Delay.GetHashCode() + Start.GetHashCode();
        }
        public override string ToString()
        {
            return "{Count=" + Count + ", Delay=" + Delay + ", Start=" + Start + "}";
        }

        public static bool operator ==(MountFrameData a, MountFrameData b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(MountFrameData a, MountFrameData b)
        {
            return !a.Equals(b);
        }
    }
    public struct MountTextureData
    {
        public readonly Func<Texture2D> Normal, Extra, Glow, ExtraGlow;

        public MountTextureData(Func<Texture2D> normal, Func<Texture2D> extra, Func<Texture2D> glow, Func<Texture2D> expertGlow)
        {
            Normal    = normal    ;
            Extra     = extra     ;
            Glow      = glow      ;
            ExtraGlow = expertGlow;
        }
        public MountTextureData(Func<Texture2D> texture)
            : this(texture, null, null, null)
        {

        }

        public Texture2D GetNormal   ()
        {
            return Normal    == null ? null : Normal   ();
        }
        public Texture2D GetExtra    ()
        {
            return Extra     == null ? null : Extra    ();
        }
        public Texture2D GetGlow     ()
        {
            return Glow      == null ? null : Glow     ();
        }
        public Texture2D GetExtraGlow()
        {
            return ExtraGlow == null ? null : ExtraGlow();
        }
    }
}
