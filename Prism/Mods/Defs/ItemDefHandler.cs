using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    /// <summary>
    /// Handles all the <see cref="ItemDef"/>s.
    /// </summary>
    static class ItemDefHandler
    {
        static int nextType = ItemID.Count;
        internal static Dictionary<int   , ItemDef> DefFromType = new Dictionary<int, ItemDef>();
        internal static Dictionary<string, ItemDef> VanillaDefFromName = new Dictionary<string, ItemDef>();

        /// <summary>
        /// Extends the vanilla arrays through which the game iterates for various type checks.
        /// </summary>
        /// <param name="amt">The amount by which to extend the arrays.</param>
        static void ExtendArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.itemAnimations.Length + amt : ItemID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.itemTexture, newLen);

            Array.Resize(ref Main.itemAnimations  , newLen);
            Array.Resize(ref Main.itemFlameLoaded , newLen);
            Array.Resize(ref Main.itemFlameTexture, newLen);
            Array.Resize(ref Main.itemFrame       , newLen);
            Array.Resize(ref Main.itemFrameCounter, newLen);

            Array.Resize(ref Item.bodyType, newLen);
            Array.Resize(ref Item.claw    , newLen);
            Array.Resize(ref Item.headType, newLen);
            Array.Resize(ref Item.legType , newLen);
            Array.Resize(ref Item.staff   , newLen);

            Array.Resize(ref ItemID.Sets.AnimatesAsSoul          , newLen);
            Array.Resize(ref ItemID.Sets.Deprecated              , newLen);
            Array.Resize(ref ItemID.Sets.ExoticPlantsForDyeTrade , newLen);
            Array.Resize(ref ItemID.Sets.ExtractinatorMode       , newLen);
            Array.Resize(ref ItemID.Sets.gunProj                 , newLen);
            Array.Resize(ref ItemID.Sets.ItemIconPulse           , newLen);
            Array.Resize(ref ItemID.Sets.ItemNoGravity           , newLen);
            Array.Resize(ref ItemID.Sets.NebulaPickup            , newLen);
            Array.Resize(ref ItemID.Sets.NeverShiny              , newLen);
            Array.Resize(ref ItemID.Sets.StaffMinionSlotsRequired, newLen);
        }

        /// <summary>
        /// Resets the loaded items.
        /// </summary>
        internal static void Reset()
        {
            nextType = ItemID.Count;
            DefFromType.Clear();
            ExtendArrays(0);
        }
        /// <summary>
        /// Loads the items into the specified Dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="Dictionary{TKey, TValue}"/> to load the items into.</param>
        internal static void Load(Dictionary<string, ItemDef> dict)
        {
            ExtendArrays(dict.Count);

            foreach (var v in dict.Values)
            {
                int type = nextType++;

                v.Type = type;
                DefFromType.Add(type, v);
            }
        }

        /// <summary>
        /// Adds all the original vanilla items.
        /// </summary>
        internal static void FillVanilla()
        {
            for (int i = -24 /* some phasesabre */; i < ItemID.Count; i++)
            {
                //if (i > -19 /* phasesabres stop at -19 because Redigit */ && i <= 0)
                // copper etc items, using <=1.2-style netids instead of the new types (backwards compatibility needed for terraria code that still uses those netids)
                if (i == 0)
                    continue;

                Item it = new Item();
                it.RealSetDefaults(i, true);

                ItemDef def = new ItemDef();

                def.DisplayName = Lang.itemName(it.type, true);
                def.InternalName = it.name;
                def.Type = it.type;
                def.NetID = i;

                // copy to ItemDef

                DefFromType.Add(i, def);
                VanillaDefFromName.Add(it.name, def);
            }
        }
    }
}
