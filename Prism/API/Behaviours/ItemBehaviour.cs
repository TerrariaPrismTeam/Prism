using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API.Defs;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
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
        public virtual void Effects(Player player) { }
    }
}
