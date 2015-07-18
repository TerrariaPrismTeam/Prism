using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;

namespace Prism.API
{
    public abstract class EntityDef
    {
        public readonly static string
            VanillaString  = "Vanilla" ,
            TerrariaString = "Terraria";

        /// <summary>
        /// The internal name used to reference this entity from any mod in Prism which uses it.
        /// </summary>
        public string InternalName
        {
            get;
            internal set;
        }
        /// <summary>
        /// Information about the mod to which this entity belongs.
        /// </summary>
        public ModInfo Mod
        {
            get;
            internal set;
        }
        /// <summary>
        /// The type of entity [need to elaborate on this...]
        /// </summary>
        public int Type
        {
            get;
            internal set;
        }

        /// <summary>
        /// The name of the entity which will show up in-game (e.g. on item in inventory, on NPC mouse hover, etc).
        /// </summary>
        public virtual string DisplayName
        {
            get;
            set;
        }
    }
}
