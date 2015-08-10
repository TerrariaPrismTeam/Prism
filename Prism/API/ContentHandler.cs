using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Util;

namespace Prism.API
{
    public abstract class ContentHandler
    {
        internal WeakReference modDef_wr;

        protected ModDef ModDef
        {
            get
            {
                if (modDef_wr == null || !modDef_wr.IsAlive)
                    throw new ObjectDisposedException("Mod");

                return (ModDef)modDef_wr.Target;
            }
        }

        protected T GetResource        <T>(string path)
        {
            return ModDef.GetResource<T>(path);
        }
        protected T GetEmbeddedResource<T>(string path, Assembly containing = null)
        {
            return ModDef.GetEmbeddedResource<T>(path, containing ?? Assembly.GetCallingAssembly());
        }

        // ---

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
