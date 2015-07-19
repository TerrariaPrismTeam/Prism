using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.Hooks;
using Prism.Mods.Resources;

namespace Prism.API
{
    /// <summary>
    /// The base class used to define a mod.
    /// Every mod must have exactly one type that inherits from <see cref="ModDef"/>.
    /// </summary>
    public abstract class ModDef : HookContainer
    {
        internal Dictionary<string, Stream> resources = new Dictionary<string, Stream>();

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
        public virtual void OnLoad  () { }

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

        /// <summary>
        /// Gets all item definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all item definitions.
        /// The key of each key/value pair is the internal name of the item.
        /// </returns>
        protected abstract Dictionary<string, ItemDef> GetItemDefs();

        /// <summary>
        /// Contains resources loaded by the mod.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <returns>The resource</returns>
        public T GetResource<T>(string path)
        {
            path = ResourceLoader.NormalizeResourceFilePath(path);

            if (!resources.ContainsKey(path))
                throw new FileNotFoundException("Resource '" + path + "' not found.");

            if (ResourceLoader.ResourceReaders.ContainsKey(typeof(T)))
                return (T)ResourceLoader.ResourceReaders[typeof(T)].ReadResource(resources[path]);

            throw new InvalidOperationException("No resource reader found for type " + typeof(T) + ".");
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        internal void Unload()
        {
            foreach (var v in resources.Values)
                v.Dispose();

            resources.Clear();
        }

        /// <summary>
        /// Gets the item defs quite by calling the protected version of <see cref="GetItemDefs"/>.
        /// </summary>
        /// <returns><see cref="GetItemDefs"/></returns>
        internal Dictionary<string, ItemDef> GetItemDefsInternally()
        {
            return GetItemDefs();
        }
    }
}
