using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Prism.Util;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.Localization;

namespace Prism.Mods.Hooks
{
    using ItemUnion = Either<ItemRef, CraftGroup<ItemDef, ItemRef>>;
    using TileUnion = Either<TileRef, CraftGroup<TileDef, TileRef>>;

    using ItemSlot = Item; // clearer difference between 'abstract' item type & item in inv

    static class RecipeHooks
    {
        static Func<ItemRef, bool> RefEq(int netID)
        {
            return r => r.ResourceID.HasValue ? netID == r.ResourceID.Value : r.Resolve().NetID == netID;
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

        static void Merge(Dictionary<int, int> dict, Item item)
        {
            int stack;
            if (dict.TryGetValue(item.netID, out stack))
                dict[item.netID] = stack + item.stack;
            else
                dict.Add(item.netID, item.stack);
        }
        static Dictionary<int, int> MergeInventory()
        {
            var dict = new Dictionary<int, int>();

            var mp = Main.player[Main.myPlayer];
            Item item = null;
            var inv = mp.inventory;

            for (int k = 0; k < Main.maxInventory; k++)
            {
                item = inv[k];

                if (!item.IsEmpty())
                    Merge(dict, item);
            }

            if (mp.chest != -1 && mp.chest > -5)
            {
                if (mp.chest == -2)
                    inv = mp.bank.item;
                else if (mp.chest == -3)
                    inv = mp.bank2.item;
                else if (mp.chest == -4)
                    inv = mp.bank3.item;
                else
                    inv = Main.chest[mp.chest].item;

                for (int l = 0; l < Chest.maxItems; l++)
                {
                    item = inv[l];

                    if (!item.IsEmpty())
                        Merge(dict, item);
                }
            }

            return dict;
        }
        static void FindRecipesInner()
        {
            var mp = Main.player[Main.myPlayer];
            var items = MergeInventory();

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

            if (Main.guideItem.type > 0 && Main.guideItem.stack > 0 && !String.IsNullOrEmpty(Main.guideItem.Name)) // in guide UI
                FindGuideRecipes();
            else
                FindRecipesInner();

            for (int n = 0; n < Main.numAvailableRecipes; n++)
                if (oldFocusRecipe == Main.availableRecipe[n])
                {
                    Main.focusRecipe = n;
                    break;
                }

            Main.focusRecipe = Math.Max(0, Math.Min(Main.focusRecipe, Main.numAvailableRecipes - 1));

            float yDiff = Main.availableRecipeY[Main.focusRecipe] - scrollUiYPos;
            for (int i = 0; i < Recipe.maxRecipes; i++)
                Main.availableRecipeY[i] -= yDiff;
        }

        static int AlchReduction(Recipe r, int stack)
        {
            if (r.alchemy && Main.player[Main.myPlayer].alchemyTable)
            {
                if (stack > 1)
                {
                    int reduction = 0;
                    for (int i = 0; i < stack; i++)
                        if (Main.rand.Next(3) == 0)
                            reduction++;

                    stack -= reduction;
                }
                else if (Main.rand.Next(3) == 0)
                    stack = 0;
            }

            return stack;
        }

        static int ConsumeItem(ItemDef id, int stack)
        {
            var mp = Main.player[Main.myPlayer];

            var inv = mp.inventory;
            Item item = null;

            for (int i = 0; i < Main.maxInventory && stack > 0; i++)
            {
                item = inv[i];

                if (item.netID != id.NetID)
                    continue;

                if (item.stack > stack)
                {
                    item.stack -= stack;
                    stack = 0;
                }
                else
                {
                    stack -= item.stack;

                    item.SetDefaults(0);
                    item.stack = 0;
                }
            }

            if (mp.chest != -1 && mp.chest > -5)
            {
                if (mp.chest == -2)
                    inv = mp.bank.item;
                else if (mp.chest == -3)
                    inv = mp.bank2.item;
                else if (mp.chest == -4)
                    inv = mp.bank3.item;
                else
                    inv = Main.chest[mp.chest].item;

                for (int i = 0; i < Chest.maxItems && stack > 0; i++)
                {
                    item = inv[i];

                    if (item.netID != id.NetID)
                        continue;

                    if (item.stack > stack)
                    {
                        item.stack -= stack;

                        if (Main.netMode == 1 && mp.chest >= 0)
                            NetMessage.SendData(MessageID.SyncChestItem, -1, -1, NetworkText.Empty, mp.chest, i, 0f, 0f, 0, 0, 0);

                        stack = 0;
                    }
                    else
                    {
                        stack -= item.stack;

                        item.SetDefaults(0);
                        item.stack = 0;

                        if (Main.netMode == 1 && mp.chest >= 0)
                            NetMessage.SendData(MessageID.SyncChestItem, -1, -1, NetworkText.Empty, mp.chest, i, 0f, 0f, 0, 0, 0);
                    }
                }
            }

            return stack;
        }
        static int ConsumeGroup(ItemGroup g, int stack)
        {
            for (var i = 0; i < g.Count && stack > 0; i++)
                stack = ConsumeItem(g[i].Resolve(), stack);

            return stack;
        }

        static void CreateBasic(this Recipe r)
        {
            for (int i = 0; i < Recipe.maxRequirements && !r.requiredItem[i].IsEmpty(); i++)
            {
                Item item = r.requiredItem[i];

                int stack = AlchReduction(r, item.stack);

                if (stack <= 0)
                    continue;

                var mp = Main.player[Main.myPlayer];

                var inv = mp.inventory;
                Item invItem = null;

                for (int j = 0; j < Main.maxInventory && stack > 0; j++)
                {
                    invItem = inv[j];

                    if (invItem.IsTheSameAs(item) ||
                            r.useWood(invItem.type, item.type) ||
                            r.useSand(invItem.type, item.type) ||
                            r.useFragment(invItem.type, item.type) ||
                            r.useIronBar(invItem.type, item.type) ||
                            r.usePressurePlate(invItem.type, item.type))
                        if (invItem.stack > stack)
                        {
                            invItem.stack -= stack;
                            stack = 0;
                        }
                        else
                        {
                            stack -= invItem.stack;

                            invItem.SetDefaults(0);
                            invItem.stack = 0;
                        }
                }
                if (mp.chest != -1 && mp.chest > -5)
                {
                    if (mp.chest == -2)
                        inv = mp.bank.item;
                    else if (mp.chest == -3)
                        inv = mp.bank2.item;
                    else if (mp.chest == -4)
                        inv = mp.bank3.item;
                    else
                        inv = Main.chest[mp.chest].item;

                    for (int k = 0; k < Chest.maxItems && stack > 0; k++)
                    {
                        invItem = inv[k];

                        if (invItem.IsTheSameAs(item) ||
                                r.useWood(invItem.type, item.type) ||
                                r.useSand(invItem.type, item.type) ||
                                r.useIronBar(invItem.type, item.type) ||
                                r.usePressurePlate(invItem.type, item.type) ||
                                r.useFragment(invItem.type, item.type))
                            if (invItem.stack > stack)
                            {
                                invItem.stack -= stack;

                                if (Main.netMode == 1 && mp.chest >= 0)
                                    NetMessage.SendData(MessageID.SyncChestItem, -1, -1, NetworkText.Empty, mp.chest, k, 0f, 0f, 0, 0, 0);

                                stack = 0;
                            }
                            else
                            {
                                stack -= invItem.stack;

                                invItem.SetDefaults(0);
                                invItem.stack = 0;

                                if (Main.netMode == 1 && mp.chest >= 0)
                                    NetMessage.SendData(MessageID.SyncChestItem, -1, -1, NetworkText.Empty, mp.chest, k, 0f, 0f, 0, 0, 0);
                            }
                    }
                }
            }
        }
        static void CreateUnion(this Recipe r)
        {
            var rd = (RecipeDef)r.P_GroupDef;

            foreach (var iu in rd.RequiredItems)
            {
                int stack = AlchReduction(r, iu.Value);

                if (stack <= 0)
                    continue;

                iu.Key.Match(ir => ConsumeItem(ir.Resolve(), stack), g => ConsumeGroup((ItemGroup)g, stack));
            }
        }
        internal static void Create(Recipe r)
        {
            if (r.P_GroupDef as RecipeDef == null)
                r.CreateBasic();
            else
                r.CreateUnion();

            AchievementsHelper.NotifyItemCraft(r);
            AchievementsHelper.NotifyItemPickup(Main.player[Main.myPlayer], r.createItem);
            Recipe.FindRecipes();
        }
    }
}

