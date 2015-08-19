using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Util;
using Prism.Mods;
using System.IO;
using Prism.Mods.Resources;

namespace Prism.API
{
    public abstract class ContentHandler
    {
        internal ModInfo? Mod;
        internal Dictionary<string, Stream> resources = new Dictionary<string, Stream>();

        public void Adopt(ModInfo owner)
        {
            Mod = Mod ?? owner;
        }

        internal void Unload()
        {
            Mod = null;
            if (resources != null)
            {
                foreach (var v in resources.Values)
                    v.Dispose();

                resources.Clear();
                resources = null;
            }
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

            var fromFilePath = Path.GetDirectoryName(path).Replace('/', '.').Replace('\\', '.') + "." + Path.GetFileName(path);
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
        /// Gets all BGM entries created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all BGM entry definitions.
        /// The key of each key/value pair is the internal name of the entry.
        /// </returns>
        protected virtual Dictionary<string, BgmEntry> GetBgms()
        {
            return Empty<string, BgmEntry>.Dictionary;
        }
        /// <summary>
        /// Gets all SFX entries created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all SFX entry definitions.
        /// The key of each key/value pair is the internal name of the entry.
        /// </returns>
        protected virtual Dictionary<string, SfxEntry> GetSfxes()
        {
            return Empty<string, SfxEntry>.Dictionary;
        }

        /// <summary>
        /// Gets all buff definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all buff definitions.
        /// The key of each key/value pair is the internal name of the buff.
        /// </returns>
        protected virtual Dictionary<string, BuffDef> GetBuffDefs()
        {
            return Empty<string, BuffDef>.Dictionary;
        }
        /// <summary>
        /// Gets all item definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all item definitions.
        /// The key of each key/value pair is the internal name of the item.
        /// </returns>
        protected virtual Dictionary<string, ItemDef> GetItemDefs()
        {
            return Empty<string, ItemDef>.Dictionary;
        }
        /// <summary>
        /// Gets all NPC definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all NPC definitions.
        /// The key of each key/value pair is the internal name of the NPC.
        /// </returns>
        protected virtual Dictionary<string, NpcDef> GetNpcDefs()
        {
            return Empty<string, NpcDef>.Dictionary;
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
        /// Gets all tile definitions created by the mod.
        /// </summary>
        /// <returns>
        /// A dictionary containing all tile definitions.
        /// The key of each key/value pair is the internal name of the tile.
        /// </returns>
        protected virtual Dictionary<string, TileDef> GetTileDefs()
        {
            return Empty<string, TileDef>.Dictionary;
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

        protected virtual GameBehaviour CreateGameBehaviour()
        {
            return null;
        }
        protected virtual PlayerBehaviour CreatePlayerBehaviour()
        {
            return null;
        }

        protected virtual BuffBehaviour CreateGlobalBuffBehaviour()
        {
            return null;
        }
        protected virtual ItemBehaviour CreateGlobalItemBehaviour()
        {
            return null;
        }
        protected virtual NpcBehaviour CreateGlobalNpcBehaviour()
        {
            return null;
        }
        protected virtual ProjectileBehaviour CreateGlobalProjectileBehaviour()
        {
            return null;
        }
        protected virtual TileBehaviour CreateGlobalTileBehaviour()
        {
            return null;
        }

        // ---

        internal Dictionary<string, BgmEntry> GetBgmsInternally()
        {
            return GetBgms();
        }
        internal Dictionary<string, SfxEntry> GetSfxesInternally()
        {
            return GetSfxes();
        }

        internal Dictionary<string, BuffDef> GetBuffDefsInternally()
        {
            return GetBuffDefs();
        }
        internal Dictionary<string, ItemDef> GetItemDefsInternally()
        {
            return GetItemDefs();
        }
        internal Dictionary<string, NpcDef> GetNpcDefsInternally()
        {
            return GetNpcDefs();
        }
        internal Dictionary<string, ProjectileDef> GetProjDefsInternally()
        {
            return GetProjectileDefs();
        }
        internal Dictionary<string, TileDef> GetTileDefsInternally()
        {
            return GetTileDefs();
        }

        internal IEnumerable<RecipeDef> GetRecipeDefsInternally()
        {
            return GetRecipeDefs();
        }

        internal GameBehaviour CreateGameBInternally()
        {
            return CreateGameBehaviour();
        }
        internal PlayerBehaviour CreatePlayerBInternally()
        {
            return CreatePlayerBehaviour();
        }

        internal BuffBehaviour CreateGlobalBuffBInternally()
        {
            return CreateGlobalBuffBehaviour();
        }
        internal ItemBehaviour CreateGlobalItemBInternally()
        {
            return CreateGlobalItemBehaviour();
        }
        internal NpcBehaviour CreateGlobalNpcBInternally()
        {
            return CreateGlobalNpcBehaviour();
        }
        internal ProjectileBehaviour CreateGlobalProjBInternally()
        {
            return CreateGlobalProjectileBehaviour();
        }
        internal TileBehaviour CreateGlobalTileBInternally()
        {
            return CreateGlobalTileBehaviour();
        }
    }
}
