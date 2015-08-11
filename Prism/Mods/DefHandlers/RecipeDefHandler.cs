using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    //TODO: we might need to rethink this
    sealed class RecipeDefHandler
    {
        internal static bool SettingUpRecipes = false;

        internal List<RecipeDef> recipes = new List<RecipeDef>(Recipe.maxRecipes);
        static int
            DefMaxRecipes = Recipe.maxRecipes,
            DefNumRecipes = -1;

        internal static List<RecipeDef> SetRecipeModDefs(ModDef mod, IEnumerable<RecipeDef> defs)
        {
            return defs.Select(d =>
            {
                d.Mod = mod.Info;
                return d;
            }).ToList();
        }

        static void CopyDefToVanilla(RecipeDef def, Recipe r)
        {
            SettingUpRecipes = true;

            r.createItem.netDefaults(def.CreateItem.Resolve().NetID);
            r.createItem.stack = def.CreateStack;

            int i = 0;
            foreach (var kvp in def.RequiredItems)
            {
                if (i >= Recipe.maxRequirements)
                    break;

                r.requiredItem[i] = new Item();
                r.requiredItem[i].netDefaults(kvp.Key.Resolve().NetID);
                r.requiredItem[i].stack = kvp.Value;

                i++;
            }

            i = 0;
            foreach (var t in def.RequiredTiles)
            {
                if (i >= Recipe.maxRequirements)
                    break;

                r.requiredTile[i] = t.Resolve().Type;

                i++;
            }

            r.needWater = (def.RequiredLiquids & RecipeLiquids.Water) != 0;
            r.needLava  = (def.RequiredLiquids & RecipeLiquids.Lava ) != 0;
            r.needHoney = (def.RequiredLiquids & RecipeLiquids.Honey) != 0;

            r.alchemy = def.RequiredTiles.Any(t => t.Resolve().Type == TileID.Bottles);

            //TODO: set any* to true when TileGroups are defined & implemented

            SettingUpRecipes = false;
        }

        static void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Recipe.numRecipes + amt : DefMaxRecipes;

            // vanilla allocates 2000 elements, but only 1806 are used, so only resize when needed
            if (newLen < Recipe.maxRecipes && amt > 0)
                return;

            Recipe.maxRecipes = newLen;

            Array.Resize(ref Main.recipe          , newLen);
            Array.Resize(ref Main.availableRecipe , newLen);
            Array.Resize(ref Main.availableRecipeY, newLen);
        }

        internal void Reset()
        {
            recipes.Clear();

            Recipe.maxRecipes = DefMaxRecipes;

            ExtendVanillaArrays(-1);

            Recipe.numRecipes = 0;

            SettingUpRecipes = true ;
            Recipe.SetupRecipes();
            SettingUpRecipes = false;

            DefNumRecipes = Recipe.numRecipes;
        }

        internal void FillVanilla()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                var r = Main.recipe[i];

                //TODO: add ItemGroups & TileGroups when they're defined & implemented
                recipes.Add(new RecipeDef(
                    new ItemRef(r.createItem.netID),
                    r.createItem.stack,
                    r.requiredItem.TakeWhile(it => it.type != 0).Select(it => new KeyValuePair<ItemRef, int>(new ItemRef(it.netID), it.stack)).ToDictionary(),
                    r.requiredTile.TakeWhile(t => t >= 0).Select(t => new TileRef(t)).ToArray(),
                    (r.needWater ? RecipeLiquids.Water : 0) |
                    (r.needLava  ? RecipeLiquids.Lava  : 0) |
                    (r.needHoney ? RecipeLiquids.Honey : 0)));
            }

            DefNumRecipes = Recipe.numRecipes;
        }

        internal List<LoaderError> Load(IEnumerable<RecipeDef> defs)
        {
            var ret = new List<LoaderError>();

            var c = defs.Count();

            // this will make the internal array resize when needed, and it won't happen multiple times when the defs collection is huge (will cause memory issues due to GC)
            // i.e. this ensures that enough memory is preallocated
            if (recipes.Count + c > recipes.Capacity)
                recipes.Capacity += c;

            ExtendVanillaArrays(c);

            Recipe.newRecipe = new Recipe(); // just to make sure

            foreach (var d in defs)
            {
                recipes.Add(d);

                try
                {
                    CopyDefToVanilla(d, Recipe.newRecipe);
                }
                catch (Exception e)
                {
                    ret.Add(new LoaderError(d.Mod, "Could not load a RecipeDef that creates item " + d.CreateItem + ".", e));
                }

                Recipe.AddRecipe();
            }

            Recipe.numRecipes += c;

            return ret;
        }
    }
}
