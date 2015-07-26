using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;

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
        /// Gets the internal name used to reference this entity from any mod in Prism which uses it.
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
        /// Gets the type of entity [need to elaborate on this...]
        /// </summary>
        public int Type
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the name of the entity which will show up in-game (e.g. on item in inventory, on NPC mouse hover, etc).
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
    }
}
