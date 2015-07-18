using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;

namespace Prism.API
{
    /// <summary>
    /// The base class used to define a mod.
    /// Every mod must have exactly one type that inherits from <see cref="ModDef" />.
    /// </summary>
    public abstract class ModDef
    {
        /// <summary>
        /// Gets the <see cref="ModInfo" /> of this mod.
        /// </summary>
        public ModInfo Info
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the <see cref="System.Reflection.Assembly" /> that defines this mod.
        /// </summary>
        public Assembly Assembly
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the mod's item definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the item's internal name (without mod internal name).</remarks>
        public Dictionary<string, ItemDef> ItemDefs
        {
            get;
            internal set;
        }

        /// <summary>
        /// WARNING: Do not place anything in the ModDef constructor, because the mod is not completely loaded yet (eg. Assembly is null).
        /// Use OnLoad to initialize fields, etc. instead.
        /// </summary>
        public ModDef() { }

        /// <summary>
        /// Called as soon as the mod is loaded.
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Gets all item definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all item definitions.
        /// The key of each key/value pair is the internal name of the item.
        /// </returns>
        protected abstract Dictionary<string, ItemDef> GetItemDefs();

        internal Dictionary<string, ItemDef> GetItemDefsI()
        {
            return GetItemDefs();
        }
    }
}
