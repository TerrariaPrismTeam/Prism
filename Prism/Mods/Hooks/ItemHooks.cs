using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.BHandlers;
using Terraria;

namespace Prism.Mods.Hooks
{
    static class ItemHooks
    {
        internal static void OnUpdateEquips(Player p, int _)
        {
            p.RealUpdateEquips(_);

            Item it;
            ItemBHandler bh;

            for (int i = 0; i < p.armor.Length; i++)
            {
                it = p.armor[i];

                if (it.type == 0 || it.stack <= 0)
                    continue;

                bh = it.P_BHandler as ItemBHandler;

                if (bh == null)
                    continue;

                if (i < 9)
                    bh.Effects(p, i, i < 3 ? EquipSlotKind.Armour : EquipSlotKind.Accessories);
                else
                    bh.VanityEffects(p, i, i < 13 ? EquipSlotKind.VanityArmour : EquipSlotKind.VanityAccessories);
            }
            for (int i = 0; i < p.miscEquips.Length; i++)
            {
                it = p.miscEquips[i];

                if (it.type == 0 || it.stack <= 0 || p.hideMisc[i])
                    continue;

                bh = it.P_BHandler as ItemBHandler;

                if (bh == null)
                    continue;

                bh.Effects(p, i, EquipSlotKind.Misc);
            }
            for (int i = 0; i < p.inventory.Length; i++)
            {
                it = p.inventory[i];

                if (it.type == 0 || it.stack <= 0)
                    continue;

                bh = it.P_BHandler as ItemBHandler;

                if (bh == null)
                    continue;

                bh.Effects(p, i, EquipSlotKind.Inventory);
            }
        }
    }
}
