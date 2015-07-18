using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    static class ItemDefHandler
    {
        static int nextType = ItemID.Count;
        internal static Dictionary<int, ItemDef> DefFromType = new Dictionary<int, ItemDef>();
        internal static Dictionary<string, ItemDef> VanillaDefFromName = new Dictionary<string, ItemDef>();

        /// <summary>
        /// Resizes the vanilla arrays through which the game iterates through for various type checks.
        /// </summary>
        /// <param name="amt">The amount by which to extend.</param>
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
        /// Loads the items!
        /// </summary>
        /// <param name="dict"></param>
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
