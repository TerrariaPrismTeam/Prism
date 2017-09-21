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
    using ItemUnion = Either<ItemRef, CraftGroup<ItemDef, ItemRef>>;
    using TileUnion = Either<TileRef, CraftGroup<TileDef, TileRef>>;

    ////TODO: we might need to rethink this
    // seems to be OK now?
    ////TODO: switch to RecipeGroups
    // vanilla RecipeGroups are pretty much an ugly hack to work around the limitations in
    // Recipe.FindRecipes and Recipe.Create, and they aren't typesafe etc.
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

        static CraftGroup<ItemDef, ItemRef> ItemGroupFromVanilla(RecipeGroup g)
        {
            return new CraftGroup<ItemDef, ItemRef>(
                g.ValidItems.Select(t => new ItemRef(t)),
                new ObjectName(g.GetText()), new ItemRef(g.IconicItemIndex));
        }
        static void CopyDefToVanilla(RecipeDef def, Recipe r)
        {
            var or = SettingUpRecipes;
            SettingUpRecipes = true;

            r.createItem.netDefaults(def.CreateItem.Resolve().NetID);
            r.createItem.stack = def.CreateStack;

            //if (def.RequiredItems.Keys.Any(e => (e.Kind & EitherKind.Left) != 0) ||
            //        def.RequiredTiles .Any(e => (e.Kind & EitherKind.Left) != 0))
            //    r.P_GroupDef = def; // handled by RecipeHooks.FindRecipes

            // for groups: display first the first, it's handled by RecipeHooks.FindRecipes
            int i = 0;
            foreach (var kvp in def.RequiredItems)
            {
                if (i >= Recipe.maxRequirements)
                    break;

                r.requiredItem[i] = new Item();
                r.requiredItem[i].netDefaults(kvp.Key.Match(MiscExtensions.Identity, g => g[0]).Resolve().NetID);
                r.requiredItem[i].stack = kvp.Value;

                i++;
            }

            i = 0;
            foreach (var t in def.RequiredTiles)
            {
                if (i >= Recipe.maxRequirements)
                    break;

                r.requiredTile[i] = t.Match(MiscExtensions.Identity, g => g[0]).Resolve().Type;

                i++;
            }

            r.alchemy = def.AlchemyReduction;

            r.needWater = (def.RequiredLiquids & RecipeLiquids.Water) != 0;
            r.needLava  = (def.RequiredLiquids & RecipeLiquids.Lava ) != 0;
            r.needHoney = (def.RequiredLiquids & RecipeLiquids.Honey) != 0;

            ////TODO: set any* to true when TileGroups are defined & implemented
            // RecipeHooks.FindRecipes handles this

            SettingUpRecipes = or;
        }

        static void ExtendVanillaArrays(int amt = 1)
        {
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Recipe.numRecipes + amt : DefMaxRecipes;

            // vanilla allocates 2000 elements, but only 1806 are used, so only resize when needed
            if (newLen < Recipe.maxRecipes && amt > 0)
                return;

            Recipe.maxRecipes = newLen;

            Array.Resize(ref Main.recipe          , newLen);
            Array.Resize(ref Main.availableRecipe , newLen);
            Array.Resize(ref Main.availableRecipeY, newLen);
        }

        internal void CheckRecipes()
        {
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                var r = Main.recipe[i];

                r.createItem.checkMat();

                for (int j = 0; j < r.requiredItem.Length; j++)
                {
                    var it = r.requiredItem[j];

                    if (it.type == 0)
                        break;

                    it.material = true;
                }
            }
        }

        internal void Reset()
        {
            recipes.Clear();
            RecipeGroup.recipeGroupIDs.Clear();
            RecipeGroup.recipeGroups  .Clear();

            Recipe.maxRecipes = DefMaxRecipes;
            RecipeGroup.nextRecipeGroupIndex = 0;

            ExtendVanillaArrays(-1);

            Recipe.numRecipes = 0;

            SettingUpRecipes = true ;
            Recipe.SetupRecipes();
            CheckRecipes();
            SettingUpRecipes = false;

            DefNumRecipes = Recipe.numRecipes;
        }

        internal void FillVanilla()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                var r = Main.recipe[i];

                recipes.Add(new RecipeDef(
                    new ItemRef(r.createItem.netID),
                    r.createItem.stack,
                    r.requiredItem.TakeWhile(it => it.type != 0)
                        // do not include RecipeGroup items
                        .Where (it => r.acceptedGroups.Any(gid =>
                        {
                            var g = RecipeGroup.recipeGroups[gid];

                            return g.ValidItems[g.IconicItemIndex] != it.netID;
                        }))
                        // add regular items
                        .Select(it => new KeyValuePair<ItemUnion, int>(new ItemRef(it.netID), it.stack))
                        // add craft groups
                        .Concat(r.acceptedGroups.Select(it =>
                        {
                            var g = RecipeGroup.recipeGroups[it];
                            var sind = Array.FindIndex(r.requiredItem,
                                        item => item.netID == g.ValidItems[g.IconicItemIndex]);

                            return new KeyValuePair<ItemUnion, int>(
                                ItemGroupFromVanilla(g),
                                r.requiredItem[sind].stack);
                        }))
                        .ToDictionary(),
                    r.requiredTile.TakeWhile( t =>  t      >= 0)
                        .Select( t => new TileRef(t)),
                    (r.needWater ? RecipeLiquids.Water : 0) |
                    (r.needLava  ? RecipeLiquids.Lava  : 0) |
                    (r.needHoney ? RecipeLiquids.Honey : 0)  )
                {
                    Mod              = PrismApi.VanillaInfo,
                    AlchemyReduction = r.alchemy
                });
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
