using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.DefHandlers;
using Terraria;

namespace Prism.Mods
{
    /// <summary>
    /// Controls the loading of entities.
    /// </summary>
    static class EntityDefLoader
    {
        /// <summary>
        /// Sets each entity def's <see cref="EntityDef{TBehaviour, TEntity}.InternalName"/>
        /// and <see cref="EntityDef{TBehaviour, TEntity}.Mod"/> fields to the def's key
        /// in the dictionary and this <see cref="ModDef"/>, respectively.
        /// </summary>
        /// <typeparam name="TEntityDef"></typeparam>
        /// <param name="def"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        static Dictionary<string, TEntityDef> SetEntityModDefs<TEntityDef, TBehaviour, TEntity>(ModDef def, Dictionary<string, TEntityDef> dict)
            where TEntity : class
            where TBehaviour : EntityBehaviour<TEntity>
            where TEntityDef : EntityDef<TBehaviour, TEntity>
        {
            foreach (var kvp in dict)
            {
                kvp.Value.InternalName = kvp.Key;
                kvp.Value.Mod = def.Info;
            }

            return dict;
        }

        /// <summary>
        /// Resets all the item/NPC/tile/projectile/etc def handlers.
        /// </summary>
        internal static void ResetEntityHandlers()
        {
            Handler.RecipeDef.Reset();

            Handler.ItemDef.Reset();
            Handler.NpcDef .Reset();
            Handler.ProjDef.Reset();
            Handler.TileDef.Reset();
        }
        /// <summary>
        /// Sets up this EntityDefLoader for loading mods, creating/adding all of the vanilla content defs, etc.
        /// </summary>
        internal static void SetupEntityHandlers()
        {
            Handler.ItemDef.FillVanilla();
            Handler.NpcDef .FillVanilla();
            Handler.ProjDef.FillVanilla();
            Handler.TileDef.FillVanilla();

            Handler.RecipeDef.FillVanilla();
        }

        /// <summary>
        /// Loads a mod and returns all <see cref="LoaderError"/>s encountered.
        /// </summary>
        /// <param name="mod">The mod to load.</param>
        /// <returns>Enumerable list of LoaderErrors encountered while loading the mod.</returns>
        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            var ret = new List<LoaderError>();

            var ch = mod.contentHandler;

            mod.ItemDefs = SetEntityModDefs<ItemDef, ItemBehaviour, Item>(mod, ch.GetItemDefsInternally());
            mod.NpcDefs = SetEntityModDefs<NpcDef, NpcBehaviour, NPC>(mod, ch.GetNpcDefsInternally());
            mod.ProjectileDefs = SetEntityModDefs<ProjectileDef, ProjectileBehaviour, Projectile>(mod, ch.GetProjDefsInternally());
            mod.TileDefs = SetEntityModDefs<TileDef, TileBehaviour, Tile>(mod, ch.GetTileDefsInternally());

            mod.RecipeDefs = RecipeDefHandler.SetRecipeModDefs(mod, ch.GetRecipeDefsInternally());

            ret.AddRange(Handler.ItemDef.Load(mod.ItemDefs));
            ret.AddRange(Handler.NpcDef.Load(mod.NpcDefs));
            ret.AddRange(Handler.ProjDef.Load(mod.ProjectileDefs));
            ret.AddRange(Handler.TileDef.Load(mod.TileDefs));

            ret.AddRange(Handler.RecipeDef.Load(mod.RecipeDefs));

            return ret;
        }
    }
}
