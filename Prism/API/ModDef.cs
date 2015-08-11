using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods;
using Prism.Mods.Hooks;
using Prism.Mods.Resources;
using Prism.Util;
using Terraria;

namespace Prism.API
{
    /// <summary>
    /// The base class used to define a mod.
    /// Every mod must have exactly one type that inherits from <see cref="ModDef"/>.
    /// </summary>
    public abstract class ModDef : HookContainer
    {
        internal ContentHandler contentHandler;
        internal GameBehaviour gameBehaviour;

        /// <summary>
        /// Gets the <see cref="ModInfo"/> of this mod.
        /// </summary>
        public ModInfo Info
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the <see cref="System.Reflection.Assembly"/> that defines this mod.
        /// </summary>
        public Assembly Assembly
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the mod's BGM entries.
        /// </summary>
        /// <remarks>The key of the dictionary is the BGM's internal name (without mod internal name).</remarks>
        public Dictionary<string, BgmEntry> BgmEntries
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
        /// Gets the mod's NPC definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the NPC's internal name (without mod internal name).</remarks>
        public Dictionary<string, NpcDef> NpcDefs
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the mod's projectile definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the projectile's internal name (without mod internal name).</remarks>
        public Dictionary<string, ProjectileDef> ProjectileDefs
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the mod's tile definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the tile's internal name (without mod internal name).</remarks>
        public Dictionary<string, TileDef> TileDefs
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the mod's recipe definitions.
        /// </summary>
        public List<RecipeDef> RecipeDefs
        {
            get;
            internal set;
        }

        /// <summary>
        /// WARNING: Do not place anything in the ModDef constructor, because the mod is not completely loaded yet (eg. Assembly is null).
        /// Use OnLoad to initialize fields, etc. instead.
        /// </summary>
        protected ModDef() { }

        /// <summary>
        /// Called as soon as the mod is loaded.
        /// </summary>
        //! do not mark as a hook, because this method is called before hooks are created.
        public virtual void OnLoad() { }

        /// <summary>
        /// A hook called when all mods are being unloaded.
        /// </summary>
        [Hook]
        public virtual void OnUnload() { }
        /// <summary>
        /// A hook called when all mods are loaded.
        /// </summary>
        [Hook]
        public virtual void OnAllModsLoaded() { }

        protected abstract ContentHandler CreateContentHandler();
        internal ContentHandler CreateContentHandlerInternally()
        {
            return CreateContentHandler();
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        internal void Unload()
        {
            if (contentHandler != null)
            {
                contentHandler.Unload();
                contentHandler = null;
            }

            if (BgmEntries != null)
            {
                BgmEntries.Clear();
                BgmEntries = null;
            }

            if (ItemDefs != null)
            {
                ItemDefs.Clear();
                ItemDefs = null;
            }

            if (NpcDefs != null)
            {
                NpcDefs.Clear();
                NpcDefs = null;
            }

            if (ProjectileDefs != null)
            {
                ProjectileDefs.Clear();
                ProjectileDefs = null;
            }

            if (TileDefs != null)
            {
                TileDefs.Clear();
                TileDefs = null;
            }

            if (RecipeDefs != null)
            {
                RecipeDefs.Clear();
                RecipeDefs = null;
            }
        }
    }
}
