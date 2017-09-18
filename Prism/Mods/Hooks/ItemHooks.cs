using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;

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

        static void DoSetBonusStuff(Player p, int offset)
        {
            for (int i = 0 + offset; i < 3 + offset; i++)
            {
                if (p.armor[i].type == 0 || p.armor[i].stack <= 0)
                    continue;

                var bh = p.armor[i].P_BHandler as ItemBHandler;

                if (bh == null)
                    continue;

                var d = Handler.ItemDef.DefsByType[p.armor[i].type];

                for (int j = 0 + offset; j < 3 + offset; j++)
                    if (Handler.ItemDef.DefsByType[p.armor[i].type].SetName != d.SetName)
                        goto OUTER_CONTINUE;

                if (offset != 0 ? bh.VanitySetBonus(p) : bh.SetBonus(p))
                    break;

                OUTER_CONTINUE:
                ;
            }
        }
        internal static void OnUpdateArmourSets(Player p, int _)
        {
            p.RealUpdateArmorSets(_);

            DoSetBonusStuff(p, 10);
            DoSetBonusStuff(p,  0);
        }

        internal static void WingMovement(Player p)
        {
            p.RealWingMovement();

            for (int i = 7 + p.extraAccessorySlots; i > 2; i--)
            {
                if (p.armor[i].wingSlot < 0)
                    continue;

                var bh = p.armor[i].P_BHandler as ItemBHandler;

                if (bh != null)
                {
                    bh.WingEffects(p, i);

                    break;
                }
            }
        }

        internal static int PreShoot(Vector2 pos, Vector2 vel, int t, int d, float kb, int pid, float ai0, float ai1)
        {
            if (pid == 0xFF)
                goto NORMAL;

            var pref = ProjectileRef.FromIDUnsafe(t);

            var p = Main.player[pid];
            var item = p.inventory[p.selectedItem];

            var bh = item.P_BHandler as ItemBHandler;

            if (bh != null)
                if (!(bh.PreShoot(p, pos, vel, pref, d, kb, ai0, ai1) ?? false))
                    return Main.maxProjectiles - 1;

        NORMAL:
            return Projectile.NewProjectile(pos.X, pos.Y, vel.X, vel.Y, t, d, kb, pid, ai0, ai1);
        }
    }
}
