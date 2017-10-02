using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods;
using Prism.Mods.Hooks;

namespace Prism.API
{
    /// <summary>
    /// The base class used to define a mod.
    /// Every mod must have exactly one type that inherits from <see cref="ModDef"/>.
    /// </summary>
    public abstract class ModDef : HookContainer
    {
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
        /// Gets the mod's content handler.
        /// </summary>
        public ContentHandler ContentHandler
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
        /// Gets the mod's SFX entries.
        /// </summary>
        /// <remarks>The key of the dictionary is the SFX's internal name (without mod internal name).</remarks>
        public Dictionary<string, SfxEntry> SfxEntries
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the mod's buff definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the buff's internal name (without mod internal name).</remarks>
        public Dictionary<string, BuffDef> BuffDefs
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
        /// Gets the mod's mount definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the mount's internal name (without mod internal name).</remarks>
        public Dictionary<string, MountDef> MountDefs
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
        /// Gets the mod's wall definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the wall's internal name (without mod internal name).</remarks>
        public Dictionary<string, WallDef> WallDefs
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

        /// <summary>Sends a message to a mod.</summary>
        public virtual object Call(string id, params object[] args)
        {
            return null;
        }
        /// <summary>Sends a message to a mod identified by its internal name.</summary>
        public static object CallMod(string toCall, string id, params object[] args)
        {
            ModDef m;
            if (ModData.modsFromInternalName.TryGetValue(toCall, out m))
                return m.Call(id, args);
            return null;
        }
        /// <summary>Sends a message to a mod identified by its ModInfo.</summary>
        public static object CallMod(ModInfo toCall, string id, params object[] args)
        {
            ModDef m;
            if (ModData.mods.TryGetValue(toCall, out m))
                return m.Call(id, args);
            return null;
        }
        /// <summary>Sends a message to all mods, EXCEPT THE CALLING MOD.</summary>
        public static object[] CallAll(string id, params object[] args)
        {
            var self = ModData.ModFromAssembly(Assembly.GetCallingAssembly());
            return ModData.mods.Where(kvp => kvp.Value != self).Select(kvp => kvp.Value.Call(id, args)).ToArray();
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        internal void Unload()
        {
            if (ContentHandler != null)
            {
                ContentHandler.Unload();
                ContentHandler = null;
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
