using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Prism.Debugging;
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

    using MergedInv = Dictionary<ItemRef, List<Item>>;

    static class RecipeHooks
    {
        // create a collection with all item slots currently available
        static IEnumerable<ItemSlot> GetInv(Player p)
        {
            foreach (var it in p.inventory) yield return it;

            switch (p.chest)
            {
                case -1: break;
                case -4:
                    foreach (var it in p.bank3.item) yield return it;
                    break;
                case -3:
                    foreach (var it in p.bank2.item) yield return it;
                    break;
                case -2:
                    foreach (var it in p.bank .item) yield return it;
                    break;
                default:
                    if (p.chest < -4) break;

                    foreach (var it in Main.chest[p.chest].item) yield return it;
                    break;
            }

            yield break;
        }

        // transform the above into something more useful
        static MergedInv MergeInv(this IEnumerable<ItemSlot> inv)
        {
            var ret = new MergedInv();

            foreach (var it in inv)
            {
                if (it.IsEmpty())
                    continue;

                List<ItemSlot> r;
                if (ret.TryGetValue(it, out r))
                    r.Add(it);
                else
                {
                    r = new List<ItemSlot>();
                    r.Add(it);
                    ret.Add(it, r);
                }
            }

            return ret;
        }
        static IEnumerable<ItemSlot> GetUsedSlots(MergedInv inv, ItemUnion ri)
        {
            List<ItemSlot> sl = null;

            return ri.IsLeft
                // if it's a craft group, we return the slots of all items that belong to it
                ? ri.Left.SelectMany(ir => !inv.TryGetValue(ir, out sl)
                    ? Empty<ItemSlot>.List : sl)
                // otherwise, just return the slot of the single item
                : !inv.TryGetValue(ri.Right, out sl) ? Empty<ItemSlot>.List : sl;
        }

        static bool HasItems(MergedInv inv, RecipeDef r)
        {
            return r.RequiredItems.All(kvp => GetUsedSlots(inv, kvp.Key).Sum(it => it.stack) >= kvp.Value);
        }
        static bool HasTiles(Player p, RecipeDef r)
        {
            return r.RequiredTiles.All(tr =>
                tr.IsLeft
                    ? tr.Left.Any(t => p.adjTile[t.Resolve().Type])
                    : p.adjTile[tr.Right.Resolve().Type]
            );
        }
        static bool HasMisc(Player p, RecipeDef rd)
        {
            // TODO: turn this into 1 big expr as well?
            // ( if (foo) r &= p; -> r = p || !foo; /*etc*/ )
            bool r = true;

            if ((rd.RequiredLiquids & RecipeLiquids.Water) != 0)
                r &= p.adjWater || p.adjTile[TileID.Sinks];
            if ((rd.RequiredLiquids & RecipeLiquids.Lava ) != 0)
                r &= p.adjLava ;
            if ((rd.RequiredLiquids & RecipeLiquids.Honey) != 0)
                r &= p.adjHoney;

            if (rd.RequiresSnowBiome) r &= p.ZoneSnow;

            return r;
        }

        static bool IsAvailable(Player p, MergedInv inv, RecipeDef r)
        {
            // TODO: add a hook here?
            return HasItems(inv, r) && HasTiles(p, r) && HasMisc(p, r);
        }
        static bool IsGuideAvailable(ItemRef it, RecipeDef rd)
        {
            return rd.RequiredItems.Keys.Any(iu => iu.IsLeft ? iu.Left.Any(ri => ri == it) : iu.Right == it);
        }

        internal static void FindRecipes()
        {
            int oldFocusRecipe = Main.availableRecipe [Main.focusRecipe];
            float scrollUiYPos = Main.availableRecipeY[Main.focusRecipe];

            Array.Clear(Main.availableRecipe, 0, Main.availableRecipe.Length);
            Main.numAvailableRecipes = 0;

            var p = Main.player[Main.myPlayer];
            var guide = !Main.guideItem.IsEmpty();
            var minv = guide ? EmptyClass<MergedInv>.Default : GetInv(p).MergeInv();
            var ig = guide ? (ItemRef)Main.guideItem : null;

            for (int i = 0; i < Main.recipe.Length; i++)
            {
                if (Main.recipe[i].createItem.IsEmpty())
                    continue;

                var r = Main.recipe[i];

                if (r.P_GroupDef as RecipeDef == null)
                {
                    Logging.LogWarning("FindRecipes(): Recipe " + i + " doesn't have a RecipeDef attached!");
                    continue;
                }

                var rd = (RecipeDef)r.P_GroupDef;

                if ((guide && IsGuideAvailable(ig, rd)) || (!guide && IsAvailable(p, minv, rd)))
                    Main.availableRecipe[Main.numAvailableRecipes++] = i;
            }

            for (int i = 0; i < Main.numAvailableRecipes; i++)
                if (oldFocusRecipe == Main.availableRecipe[i])
                {
                    Main.focusRecipe = i;
                    break;
                }

            Main.focusRecipe = Math.Max(0, Math.Min(Main.focusRecipe, Main.numAvailableRecipes - 1));

            float yDiff = Main.availableRecipeY[Main.focusRecipe] - scrollUiYPos;
            for (int i = 0; i < Main.availableRecipeY.Length; i++)
                Main.availableRecipeY[i] -= yDiff;
        }

        // assumes HasItems returned true
        static void Consume(IEnumerable<ItemSlot> slots, int stack)
        {
            foreach (var slot in slots)
            {
                // we have enough to craft the whole bunch
                if (slot.stack > stack)
                {
                    slot.stack -= stack;
                    return;
                }

                // deplete this slot
                stack -= slot.stack;
                slot.TurnToAir();
            }
        }

        internal static void Create(Recipe r)
        {
            var p = Main.player[Main.myPlayer];

            if (r.P_GroupDef as RecipeDef == null)
            {
                Logging.LogWarning("Create(): Recipe.P_GroupDef was null!");
                r.RealCreate();
                return;
            }

            var rd = (RecipeDef)r.P_GroupDef;

            var minv = GetInv(p).MergeInv();

            foreach (var kvp in rd.RequiredItems)
            {
                var slots = GetUsedSlots(minv, kvp.Key);
                Consume(slots, kvp.Value);
            }

            AchievementsHelper.NotifyItemCraft(r);
            AchievementsHelper.NotifyItemPickup(p, r.createItem);
            Recipe.FindRecipes();
        }
    }
}

