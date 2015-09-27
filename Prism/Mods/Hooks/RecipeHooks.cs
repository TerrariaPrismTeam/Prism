using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API;
using Prism.API.Defs;
using Prism.Util;
using Terraria;

namespace Prism.Mods.Hooks
{
    using ItemUnion = Either<ItemRef, CraftGroup<ItemDef, ItemRef>>;
    using TileUnion = Either<TileRef, CraftGroup<TileDef, TileRef>>;

    static class RecipeHooks
    {
        static Func<ItemRef, bool> RefEq(int netID)
        {
            return r => r.Resolve().NetID == netID;
        }
        static KeyValuePair<ItemUnion, int>? UsesItem(Recipe r, int netID)
        {
            if (netID == 0)
                return null;

            if (r.P_GroupDef as RecipeDef != null)
            {
                var rd = (RecipeDef)r.P_GroupDef;

                foreach (var id in rd.RequiredItems)
                {
                    var e = RefEq(netID);

                    if (id.Key.Match(e, ig => ig.Any(e)))
                        return id;
                }
            }
            else
                for (int i = 0; i < r.requiredItem.Length && !r.requiredItem[i].IsEmpty(); i++)
                {
                    var it = r.requiredItem[i];

                    if (it.netID == netID ||
                            r.useWood         (netID, it.type) ||
                            r.useSand         (netID, it.type) ||
                            r.useIronBar      (netID, it.type) ||
                            r.useFragment     (netID, it.type) ||
                            r.usePressurePlate(netID, it.type))
                        return new KeyValuePair<ItemUnion, int>(ItemUnion.NewRight(ItemDef.Defs[it.netID]), it.stack);
                }

            return null;
        }
        static void FindGuideRecipes()
        {
            for (int i = 0; i < Recipe.maxRecipes && !Main.recipe[i].createItem.IsEmpty(); i++)
                if (UsesItem(Main.recipe[i], Main.guideItem.netID).HasValue)
                {
                    Main.availableRecipe[Main.numAvailableRecipes] = i;
                    Main.numAvailableRecipes++;
                }
        }

        static Dictionary<int, int> MergeInventory()
        {
            var dict = new Dictionary<int, int>();

            var mp = Main.player[Main.myPlayer];
            Item item = null;
            Item[] inv = mp.inventory;

            for (int k = 0; k < 58; k++)
            {
                item = inv[k];

                if (!item.IsEmpty())
                    if (dict.ContainsKey(item.netID))
                        dict[item.netID] += item.stack;
                    else
                        dict.Add(item.netID, item.stack);
            }

            if (mp.chest != -1)
            {
                if (mp.chest > -1)
                    inv = Main.chest[mp.chest].item;
                else if (mp.chest == -2)
                    inv = mp.bank.item;
                else if (mp.chest == -3)
                    inv = mp.bank2.item;

                for (int l = 0; l < Chest.maxItems; l++)
                {
                    item = inv[l];

                    if (!item.IsEmpty())
                        if (dict.ContainsKey(item.netID))
                            dict[item.netID] += item.stack;
                        else
                            dict.Add(item.netID, item.stack);
                }
            }

            return dict;
        }
        static void FindRecipesInner()
        {
            var items = MergeInventory();

            var mp = Main.player[Main.myPlayer];

            for (int i = 0; i < Recipe.maxRecipes && !Main.recipe[i].createItem.IsEmpty(); i++)
            {
                var r = Main.recipe[i];

                bool isA = true;

                // check proximity of tiles
                if (r.P_GroupDef as RecipeDef != null)
                    foreach (var e in ((RecipeDef)r.P_GroupDef).RequiredTiles)
                    {
                        if (!e.Match(tr => mp.adjTile[tr.Resolve().Type], g => g.Any(tr => mp.adjTile[tr.Resolve().Type])))
                        {
                            isA = false;
                            break;
                        }
                    }
                else
                    for (int j = 0; j < Recipe.maxRequirements && r.requiredTile[j] != -1; j++)
                        if (!mp.adjTile[r.requiredTile[j]])
                        {
                            isA = false;
                            break;
                        }

                if (!isA)
                    continue;

                // check items
                if (r.P_GroupDef as RecipeDef != null)
                    foreach (var e in ((RecipeDef)r.P_GroupDef).RequiredItems)
                    {
                        int stack = e.Value;

                        foreach (var current in items)
                        {
                            var eq = RefEq(current.Key);
                            if (e.Key.Match(eq, g => g.Any(eq)))
                                stack -= current.Value;
                        }

                        if (stack > 0)
                        {
                            isA = false;
                            break;
                        }
                    }
                else
                    for (int m = 0; m < Recipe.maxRequirements && !r.requiredItem[m].IsEmpty(); m++)
                    {
                        var it = r.requiredItem[m];

                        bool found = false;
                        int stack = it.stack;

                        foreach (int current in items.Keys)
                            if (r.useWood         (current, it.type) ||
                                r.useSand         (current, it.type) ||
                                r.useIronBar      (current, it.type) ||
                                r.useFragment     (current, it.type) ||
                                r.usePressurePlate(current, it.type))
                            {
                                stack -= items[current];
                                found = true;
                            }

                        if (!found && items.ContainsKey(it.netID))
                            stack -= items[it.netID];

                        if (stack > 0)
                        {
                            isA = false;
                            break;
                        }
                    }

                if (!isA)
                    continue;

                // check liquids
                bool water = !r.needWater || mp.adjWater || mp.adjTile[172];
                bool honey = !r.needHoney || r.needHoney == mp.adjHoney;
                bool lava  = !r.needLava  || r.needLava  == mp.adjLava ;

                if (!water || !honey || !lava)
                    continue;

                Main.availableRecipe[Main.numAvailableRecipes] = i;
                Main.numAvailableRecipes++;
            }
        }

        internal static void FindRecipes()
        {
            int oldFocusRecipe = Main.availableRecipe [Main.focusRecipe];
            float scrollUiYPos = Main.availableRecipeY[Main.focusRecipe];

            for (int i = 0; i < Recipe.maxRecipes; i++)
                Main.availableRecipe[i] = 0;
            Main.numAvailableRecipes = 0;

            if (Main.guideItem.type > 0 && Main.guideItem.stack > 0 && !String.IsNullOrEmpty(Main.guideItem.name)) // in guide UI
                FindGuideRecipes();
            else
                FindRecipesInner();

            for (int n = 0; n < Main.numAvailableRecipes; n++)
                if (oldFocusRecipe == Main.availableRecipe[n])
                {
                    Main.focusRecipe = n;
                    break;
                }

            Main.focusRecipe = Math.Min(Math.Max(0, Main.focusRecipe), Main.numAvailableRecipes - 1);

            float yDiff = Main.availableRecipeY[Main.focusRecipe] - scrollUiYPos;
            for (int i = 0; i < Recipe.maxRecipes; i++)
                Main.availableRecipeY[i] -= yDiff;
        }
    }
}
