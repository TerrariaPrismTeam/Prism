using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods;
using Prism.Mods.Hooks;
using Prism.Mods.Resources;
using Prism.Util;

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
        public Dictionary<string, ItemDef      > ItemDefs
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the mod's NPC definitions.
        /// </summary>
        /// <remarks>The key of the dictionary is the NPC's internal name (without mod internal name).</remarks>
        public Dictionary<string, NpcDef       > NpcDefs
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
        /// Gets the mod's recipe definitions.
        /// </summary>
        public IEnumerable<RecipeDef> RecipeDefs
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

        //TODO: move this to a separate class containing game- or world-related hooks... later
        /// <summary>
        /// A hook called at the end of the game's Update method.
        /// </summary>
        [Hook]
        public virtual void PostUpdate() { }

        //TODO: move these somewhere else? (it might get crowded with these soon)
        /// <summary>
        /// Gets all item definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all item definitions.
        /// The key of each key/value pair is the internal name of the item.
        /// </returns>
        protected virtual Dictionary<string, ItemDef      > GetItemDefs      ()
        {
            return Empty<string, ItemDef      >.Dictionary;
        }
        /// <summary>
        /// Gets all NPC definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all NPC definitions.
        /// The key of each key/value pair is the internal name of the NPC.
        /// </returns>
        protected virtual Dictionary<string, NpcDef       > GetNpcDefs       ()
        {
            return Empty<string, NpcDef       >.Dictionary;
        }
        /// <summary>
        /// Gets all projectile definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all projectile definitions.
        /// The key of each key/value pair is the internal name of the projectile.
        /// </returns>
        protected virtual Dictionary<string, ProjectileDef> GetProjectileDefs()
        {
            return Empty<string, ProjectileDef>.Dictionary;
        }

        /// <summary>
        /// Gets all recipe definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A collection containing all recipe definitions.
        /// </returns>
        protected virtual IEnumerable<RecipeDef> GetRecipeDefs()
        {
            return Empty<RecipeDef>.Array;
        }

        protected virtual ItemBehaviour       CreateGlobalItemBehaviour      ()
        {
            return null;
        }
        protected virtual NpcBehaviour        CreateGlobalNpcBehaviour       ()
        {
            return null;
        }
        protected virtual ProjectileBehaviour CreateGlobalProjectileBehaviour()
        {
            return null;
        }

        T GetResourceInternal<T>(Func<Stream> getStream)
        {
            if (ResourceLoader.ResourceReaders.ContainsKey(typeof(T)))
                return (T)ResourceLoader.ResourceReaders[typeof(T)].ReadResource(getStream());

            throw new InvalidOperationException("No resource reader found for type " + typeof(T) + ".");
        }

        /// <summary>
        /// Gets the specified resource loaded by the mod.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <returns>The resource</returns>
        public T GetResource<T>(string path)
        {
            path = ResourceLoader.NormalizeResourceFilePath(path);

            if (!resources.ContainsKey(path))
                throw new FileNotFoundException("Resource '" + path + "' not found.");

            return GetResourceInternal<T>(() => resources[path]);
        }
        /// <summary>
        /// Returns the specified resource embedded in the mod's assembly.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <param name="containing">The assembly that contains the embedded resource. Leave it to null for the calling assembly.</param>
        /// <returns>The resource</returns>
        public T GetEmbeddedResource<T>(string path, Assembly containing = null)
        {
            var c = containing ?? Assembly.GetCallingAssembly();

            var asmNamePfix = c.GetName().Name + ".";
            var path_ = ResourceLoader.NormalizeResourceFilePath(path, asmNamePfix);

            var fromFilePath  = Path.GetDirectoryName(path ).Replace('/', '.').Replace('\\', '.') + "." + Path.GetFileName(path );
            var fromFilePath_ = Path.GetDirectoryName(path_).Replace('/', '.').Replace('\\', '.') + "." + Path.GetFileName(path_);

            var tries = new[]
            {
                path,
                asmNamePfix + path,
                fromFilePath,
                asmNamePfix + fromFilePath,

                path_,
                asmNamePfix + path_,
                fromFilePath_,
                asmNamePfix + fromFilePath_
            };

            for (int i = 0; i < tries.Length; i++)
                if (Array.IndexOf(c.GetManifestResourceNames(), tries[i]) != -1)
                    return GetResourceInternal<T>(() => c.GetManifestResourceStream(tries[i])); // passing 'i' directly here is ok, because the function is called before GetResourceInternal returns (and i increases)

            throw new FileNotFoundException("Embedded resource '" + path + "' not found.");
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
        /// Gets the item defs by calling the protected version of <see cref="GetItemDefs" />.
        /// </summary>
        /// <returns><see cref="GetItemDefs"/></returns>
        internal Dictionary<string, ItemDef      > GetItemDefsInternally()
        {
            return GetItemDefs();
        }
        /// <summary>
        /// Gets the NPC defs by calling the protected version of <see cref="GetNpcDefs" />.
        /// </summary>
        /// <returns><see cref="GetNpcDefs"/></returns>
        internal Dictionary<string, NpcDef       > GetNpcDefsInternally ()
        {
            return GetNpcDefs();
        }
        /// <summary>
        /// Gets the projectile defs by calling the protected version of <see cref="GetProjectileDefs" />.
        /// </summary>
        /// <returns><see cref="GetProjectileDefs"/></returns>
        internal Dictionary<string, ProjectileDef> GetProjDefsInternally()
        {
            return GetProjectileDefs();
        }

        /// <summary>
        /// Gets the recipe defs by calling the protected version of <see cref="GetRecipeDefsInternally" />.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<RecipeDef> GetRecipeDefsInternally()
        {
            return GetRecipeDefs();
        }

        internal virtual ItemBehaviour CreateGlobalItemBInternally      ()
        {
            return CreateGlobalItemBehaviour      ();
        }
        internal virtual NpcBehaviour CreateGlobalNpcBInternally        ()
        {
            return CreateGlobalNpcBehaviour       ();
        }
        internal virtual ProjectileBehaviour CreateGlobalProjBInternally()
        {
            return CreateGlobalProjectileBehaviour();
        }
    }
}
