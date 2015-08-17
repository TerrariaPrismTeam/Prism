using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API.Defs;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public enum EquipSlotKind
    {
        /// <summary>
        /// <see cref="Player.armor" />[0..2]
        /// </summary>
        Armour,
        /// <summary>
        /// <see cref="Player.armor" />[3..8+<see cref="Player.extraAccessorySlots" />]
        /// </summary>
        Accessories,
        /// <summary>
        /// <see cref="Player.armor" />[10..12]
        /// </summary>
        VanityArmour,
        /// <summary>
        /// <see cref="Player.armor" />[13..18+<see cref="Player.extraAccessorySlots" />]
        /// </summary>
        VanityAccessories,
        /// <summary>
        /// <see cref="Player.miscEquips" />
        /// </summary>
        Misc,
        /// <summary>
        /// <see cref="Player.inventory" />
        /// </summary>
        Inventory
    }

    public abstract class ItemBehaviour : EntityBehaviour<Item>
    {
        [Hook]
        public virtual bool CanUse(Player player)
        {
            return true;
        }
        [Hook]
        public virtual bool? UseItem(Player player)
        {
            return null;
        }
        [Hook]
        public virtual bool PreShoot(Player player, Vector2 position, Vector2 velocity, ProjectileRef proj, int damage, float knockback)
        {
            return true;
        }

        [Hook]
        public virtual void Effects(Player player, int slot, EquipSlotKind kind) { }
        [Hook]
        public virtual void VanityEffects(Player player, int slot, EquipSlotKind kind) { }
    }
}
