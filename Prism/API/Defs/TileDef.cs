using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Util;
using Terraria;
using Terraria.ObjectData;

namespace Prism.API.Defs
{

    public partial class TileDef : EntityDef<TileBehaviour, Tile>
    {
        /// <summary>
        /// Gets or sets the tile's texture getter.
        /// </summary>
        public Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tile's instance behaviour constructor.
        /// </summary>
        public Func<TileBehaviour> CreateInstanceBehaviour
        {
            get;
            set;
        }

        public TileSubtypeData SubtypeData
        {
            get;
            set;
        }
        public TileAestheticData AestheticData
        {
            get;
            set;
        }
        public TileMineData MineData
        {
            get;
            set;
        }
        public TileConversionData ConversionData
        {
            get;
            set;
        }
        public TileLightingData LightingData
        {
            get;
            set;
        }
        public TileCollisionData CollisionData
        {
            get;
            set;
        }
        public TilePlacementData PlacementData
        {
            get;
            set;
        }
        public TileFrameData FrameData
        {
            get;
            set;
        }
        public TileHousingData HousingData
        {
            get;
            set;
        }
        public TileNpcData NpcData
        {
            get;
            set;
        }

        //TODO: hide the objectdata & use custom API stuff?
        public TileObjectData OtherData
        {
            get;
            set;
        }

        public bool NoFail
        {
            get;
            set;
        }
        public short Value
        {
            get;
            set;
        }

        [Obsolete("Not implemented yet.")]
        public string MapTooltip
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

        public bool AllTiles
        {
            get;
            set;
        }
        public bool IsFalling
        {
            get;
            set;
        }

        //TODO: drop item

        public TileDef(string displayName, Func<TileBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null, Func<TileBehaviour> newInstBehaviour = null)
            : base(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;

            CreateInstanceBehaviour = newInstBehaviour ?? Empty<TileBehaviour>.Func;

#pragma warning disable 618
            MapTooltip = String.Empty;
            MapColour = Color.Transparent;
#pragma warning restore 618

            SubtypeData    = new TileSubtypeData   ();
            AestheticData  = new TileAestheticData () { MergeWith = Empty<TileRef>.List };
            ConversionData = new TileConversionData();
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
