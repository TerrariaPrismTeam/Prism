using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Util;

namespace Prism.API.Defs
{
    public partial class WallDef : ObjectDef
    {
        /// <summary>
        /// Gets or sets the wall's texture function.
        /// </summary>
        public Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        public bool IsSuitableForHousing
        {
            get;
            set;
        }
        public bool Light
        {
            get;
            set;
        }
        public bool IsDungeonWall
        {
            get;
            set;
        }

        public bool IsCorruption
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
        public bool IsTransparent
        {
            get;
            set;
        }

        public WallConversionData ConversionData
        {
            get;
            set;
        }

        [Obsolete("Not implemented yet.")]
        public Color MapColour
        {
            get;
            set;
        }

        //TODO: figure out what these do
        public int Blend
        {
            get;
            set;
        }
        public WallLargeFrameKind LargeFrameKind
        {
            get;
            set;
        }

        //TODO: drop item

        public WallDef(string displayName, Func<Texture2D> getTexture = null)
            : base(displayName)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;

#pragma warning disable 618
            MapColour = Color.Transparent;
#pragma warning restore 618
        }

        public static implicit operator WallRef(WallDef  def)
        {
            return new WallRef(def.InternalName, def.Mod.InternalName);
        }
        public static explicit operator WallDef(WallRef @ref)
        {
            return @ref.Resolve();
        }
    }
}
