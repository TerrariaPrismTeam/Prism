using System;
using System.Collections.Generic;
using System.Linq;
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

        //TODO: figure out what Main.wallBlend does

        public WallDef(string displayName, Func<Texture2D> getTexture = null)
            : base(displayName)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;
        }
    }
}
