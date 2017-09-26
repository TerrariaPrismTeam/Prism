using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.Map;

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
        static Dictionary<string, TEntityDef> SetEntityModDefs<TEntityDef>(ModDef def, Dictionary<string, TEntityDef> dict)
            where TEntityDef : ObjectDef
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

            Handler.BuffDef .Reset();
            Handler.ItemDef .Reset();
            Handler.MountDef.Reset();
            Handler.NpcDef  .Reset();
            Handler.ProjDef .Reset();
            Handler.TileDef .Reset();
            Handler.WallDef .Reset();

            AmmoGroup.Reset();

            Handler.DefaultColourLookupLength = MapHelper.colorLookup.Length;
        }
        /// <summary>
        /// Sets up this EntityDefLoader for loading mods, creating/adding all of the vanilla content defs, etc.
        /// </summary>
        internal static void SetupEntityHandlers()
        {
            Handler.BuffDef .FillVanilla();
            Handler.ItemDef .FillVanilla();
            Handler.MountDef.FillVanilla();
            Handler.NpcDef  .FillVanilla();
            Handler.ProjDef .FillVanilla();
            Handler.TileDef .FillVanilla();
            Handler.WallDef .FillVanilla();

            Handler.RecipeDef.FillVanilla();

            AmmoGroup.FillVanilla();

            Handler.RecipeDef.CheckRecipes();
        }

        /// <summary>
        /// Loads a mod and returns all <see cref="LoaderError"/>s encountered.
        /// </summary>
        /// <param name="mod">The mod to load.</param>
        /// <returns>Enumerable list of LoaderErrors encountered while loading the mod.</returns>
        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            var ret = new List<LoaderError>();

            var ch = mod.ContentHandler;

            mod.BuffDefs       = SetEntityModDefs(mod, ch.GetBuffDefsInternally ());
            mod.ItemDefs       = SetEntityModDefs(mod, ch.GetItemDefsInternally ());
            mod.MountDefs      = SetEntityModDefs(mod, ch.GetMountDefsInternally());
            mod.NpcDefs        = SetEntityModDefs(mod, ch.GetNpcDefsInternally  ());
            mod.ProjectileDefs = SetEntityModDefs(mod, ch.GetProjDefsInternally ());
            mod.TileDefs       = SetEntityModDefs(mod, ch.GetTileDefsInternally ());
            mod.WallDefs       = SetEntityModDefs(mod, ch.GetWallDefsInternally ());

            mod.RecipeDefs = RecipeDefHandler.SetRecipeModDefs(mod, ch.GetRecipeDefsInternally());

            ret.AddRange(Handler.BuffDef .Load(mod.BuffDefs      ));
            ret.AddRange(Handler.ItemDef .Load(mod.ItemDefs      ));
            ret.AddRange(Handler.MountDef.Load(mod.MountDefs     ));
            ret.AddRange(Handler.NpcDef  .Load(mod.NpcDefs       ));
            ret.AddRange(Handler.ProjDef .Load(mod.ProjectileDefs));
            ret.AddRange(Handler.TileDef .Load(mod.TileDefs      ));
            ret.AddRange(Handler.WallDef .Load(mod.WallDefs      ));

            ret.AddRange(Handler.RecipeDef.Load(mod.RecipeDefs));

            return ret;
        }
    }
}
