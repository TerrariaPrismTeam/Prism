using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Prism.Debugging;
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

            var netid = def.CreateItem.Resolve().NetID;
            r.createItem.netDefaults(netid);
            r.createItem.stack = def.CreateStack;

            r.P_GroupDef = def;

            // for groups: display first the first, it's handled by RecipeHooks.FindRecipes
            int i = 0;
            foreach (var t in def.RequiredTiles)
            {
                if (i >= Recipe.maxRequirements)
                {
                    Logging.LogWarning("Recipe '" + def + "' requires more tiles than vanilla can handle.");
                    break;
                }

                r.requiredTile[i] = t.Match(MiscExtensions.Identity, g => g[0]).Resolve().Type;

                i++;
            }

            i = 0;
            foreach (var kvp in def.RequiredItems)
            {
                if (i >= Recipe.maxRequirements)
                {
                    Logging.LogWarning("Recipe '" + def + "' requires more items than vanilla can handle.");
                    break;
                }

                r.requiredItem[i] = new Item();
                r.requiredItem[i].netDefaults(kvp.Key.Match(MiscExtensions.Identity, g => g[0]).Resolve().NetID);
                r.requiredItem[i].stack = kvp.Value;

                i++;
            }

            r.alchemy = def.AlchemyReduction;

            r.needWater = (def.RequiredLiquids & RecipeLiquids.Water) != 0;
            r.needLava  = (def.RequiredLiquids & RecipeLiquids.Lava ) != 0;
            r.needHoney = (def.RequiredLiquids & RecipeLiquids.Honey) != 0;

            r.needSnowBiome = def.RequiresSnowBiome;

            foreach (var kvp in def.RequiredItems)
            {
                if (!kvp.Key.IsLeft)
                    continue;

                if (kvp.Key.Left == ItemGroup.Fragment)
                {
                    r.anyFragment = true;
                    r.requiredItem[i] = new Item();
                    r.requiredItem[i].netDefaults(ItemID.FragmentVortex);
                    r.requiredItem[i].stack = kvp.Value;
                    i++;
                }
                else if (kvp.Key.Left == ItemGroup.Tier2Bar)
                {
                    r.anyIronBar = true;
                    r.requiredItem[i] = new Item();
                    r.requiredItem[i].netDefaults(ItemID.IronBar);
                    r.requiredItem[i].stack = kvp.Value;
                    i++;
                }
                else if (kvp.Key.Left == ItemGroup.PressurePlate)
                {
                    r.anySand = true;
                    r.requiredItem[i] = new Item();
                    r.requiredItem[i].netDefaults(ItemID.GrayPressurePlate);
                    r.requiredItem[i].stack = kvp.Value;
                    i++;
                }
                else if (kvp.Key.Left == ItemGroup.Sand)
                {
                    r.anySand = true;
                    r.requiredItem[i] = new Item();
                    r.requiredItem[i].netDefaults(ItemID.SandBlock);
                    r.requiredItem[i].stack = kvp.Value;
                    i++;
                }
                else if (kvp.Key.Left == ItemGroup.Wood)
                {
                    r.anyWood = true;
                    r.requiredItem[i] = new Item();
                    r.requiredItem[i].netDefaults(ItemID.Wood);
                    r.requiredItem[i].stack = kvp.Value;
                    i++;
                }
            }

            SettingUpRecipes = or;
        }
        static void CopyVanillaToDef(Recipe r, RecipeDef def)
        {
            def.CreateItem  = new ItemRef(r.createItem.netID);
            def.CreateStack = r.createItem.stack;

            var items = new Dictionary<ItemUnion, int>();
            var tiles = new List<TileUnion>();

            // * craftgroups are added from the vanilla recipegroups
            // * all items that aren't recipegroup "iconic item"s are added to the item list
            foreach (var it in r.requiredItem)
            {
                if (it.IsEmpty())
                    break;

                foreach (var gi in r.acceptedGroups)
                {
                    var g = RecipeGroup.recipeGroups[gi];

                    if (g.ValidItems[g.IconicItemIndex] == it.netID)
                        goto CONTINUE_OUTER;
                }

                items.Add(new ItemRef(it.netID), it.stack);
            CONTINUE_OUTER:;
            }

            foreach (var gi in r.acceptedGroups)
            {
                var g = RecipeGroup.recipeGroups[gi];

                var icon = r.requiredItem.First(it => it.netID == g.ValidItems[g.IconicItemIndex]);

                items.Add(ItemGroupFromVanilla(g), icon.stack);
            }

            foreach (var ti in r.requiredTile)
            {
                if (ti < 0)
                    break;

                tiles.Add(new TileRef(ti));
            }

            if (r.needWater) { def.RequiredLiquids |= RecipeLiquids.Water; }
            if (r.needLava ) { def.RequiredLiquids |= RecipeLiquids.Lava ; }
            if (r.needHoney) { def.RequiredLiquids |= RecipeLiquids.Honey; }

            def.RequiresSnowBiome = r.needSnowBiome;

            if (r.anyFragment     )
                foreach (var it in r.requiredItem)
                    if (ItemGroup.Fragment.List.Any(fi => fi.Resolve().NetID == it.netID))
                    {
                        items.Add(ItemGroup.Fragment     , it.stack);
                        break;
                    }
            if (r.anyIronBar      )
                foreach (var it in r.requiredItem)
                    if (ItemGroup.Fragment.List.Any(fi => fi.Resolve().NetID == it.netID))
                    {
                        items.Add(ItemGroup.Tier2Bar     , it.stack);
                        break;
                    }
            if (r.anyPressurePlate)
                foreach (var it in r.requiredItem)
                    if (ItemGroup.Fragment.List.Any(fi => fi.Resolve().NetID == it.netID))
                    {
                        items.Add(ItemGroup.PressurePlate, it.stack);
                        break;
                    }
            if (r.anySand         )
                foreach (var it in r.requiredItem)
                    if (ItemGroup.Fragment.List.Any(fi => fi.Resolve().NetID == it.netID))
                    {
                        items.Add(ItemGroup.Sand         , it.stack);
                        break;
                    }
            if (r.anyWood         )
                foreach (var it in r.requiredItem)
                    if (ItemGroup.Fragment.List.Any(fi => fi.Resolve().NetID == it.netID))
                    {
                        items.Add(ItemGroup.Wood         , it.stack);
                        break;
                    }

            def.RequiredItems = items;
            def.RequiredTiles = tiles;
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
            Recipe.SetupRecipes(); // calls SetupRecipeGroups
            CheckRecipes();
            SettingUpRecipes = false;

            DefNumRecipes = Recipe.numRecipes;
        }

        internal void FillVanilla()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                var r = Main.recipe[i];
                var d = new RecipeDef(
                        new ItemRef(r.createItem.netID), r.createItem.stack,
                        Empty<ItemUnion, int>.Dictionary, Empty<TileUnion>.Array);

                CopyVanillaToDef(r, d);

                recipes.Add(d);

                r.P_GroupDef = d;
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

            return ret;
        }
    }
}
