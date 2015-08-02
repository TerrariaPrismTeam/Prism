using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Util;

namespace Prism.API.Defs
{
    /// <summary>
    /// The class from which the definitions for items, NPCs, projectiles, tiles, etc. are derived.
    /// </summary>
    public abstract class EntityDef<TBehaviour, TEntity>
        where TEntity : class
        where TBehaviour : EntityBehaviour<TEntity>
    {
        /// <summary>
        /// Gets the internal name used to reference this entity from within any Prism mod.
        /// </summary>
        public string InternalName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets Information about the mod to which this entity belongs.
        /// </summary>
        public ModInfo Mod
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the internal type index of this entity.
        /// </summary>
        /// <remarks>Only use this after all mods are loaded.</remarks>
        public int Type
        {
            get;
            internal set;
        }

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
        /// Gets or sets the name of the entity which will show up in-game.
        /// <para/>
        /// E.g.: An item's name in inventory, an NPC's name displayed on mouse hover, an NPC's name displayed in player death messages, etc.
        /// <para/>
        /// Note: Although there exists one way to see a Projectile's name (in player death messages), they don't have a display name property in the vanilla game
        /// </summary>
        public virtual string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameterless constructor that instantiates the matching EntityBehaviour class of the EntityRef.
        /// </summary>
        public virtual Func<TBehaviour> CreateBehaviour
        {
            get;
            set;
        }

        protected EntityDef(string displayName, Func<TBehaviour> newBehaviour = null)
        {
            InternalName = String.Empty;

            DisplayName = displayName;
            CreateBehaviour = newBehaviour ?? Empty<TBehaviour>.Func;
        }

        public override string ToString()
        {
            return "{" + (String.IsNullOrEmpty(InternalName) ? DisplayName : InternalName) + ", Mod=" + Mod + "}";
        }
    }
}
