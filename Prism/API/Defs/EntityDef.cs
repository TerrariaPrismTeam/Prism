using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Util;

namespace Prism.API.Defs
{
    /// <summary>
    /// The class from which the definitions for items, NPCs, projectiles, tiles, etc. are derived.
    /// </summary>
    public abstract class EntityDef<TBehaviour, TEntity> : ObjectDef<TBehaviour>
        where TEntity : class
        where TBehaviour : EntityBehaviour<TEntity>
    {
        int setNetID = 0;
        /// <summary>
        /// Gets this item's NetID.
        /// </summary>
        public int NetID
        {
            get
            {
                return setNetID == 0 ? Type : setNetID;
            }
            internal set
            {
                setNetID = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of this entity.
        /// </summary>
        public Point Size
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width of this entity.
        /// </summary>
        public int Width
        {
            get
            {
                return Size.X;
            }
            set
            {
                Size = new Point(value, Size.Y);
            }
        }
        /// <summary>
        /// Gets or sets the height of this entity.
        /// </summary>
        public int Height
        {
            get
            {
                return Size.Y;
            }
            set
            {
                Size = new Point(Size.X, value);
            }
        }

        protected EntityDef(string displayName, Func<TBehaviour> newBehaviour = null)
            : base(displayName, newBehaviour)
        {

        }
    }
}
