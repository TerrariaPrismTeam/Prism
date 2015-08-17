using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.Hooks;
using Prism.Util;
using Terraria;

namespace Prism.Mods.BHandlers
{
    sealed class ItemBHandler : EntityBHandler<ItemBehaviour, Item>
    {
        IEnumerable<Func<Player, bool>> canUse;
        IEnumerable<Func<Player, bool?>> useItem;
        IEnumerable<Func<Player, Vector2, Vector2, ProjectileRef, int, float, bool>> preShoot;

        IEnumerable<Action<Player, int, EquipSlotKind>> effects, vanityEffects;

        public override void Create()
        {
            base.Create();

            canUse   = HookManager.CreateHooks<ItemBehaviour, Func<Player, bool>>(Behaviours, "CanUse");
            useItem  = HookManager.CreateHooks<ItemBehaviour, Func<Player, bool?>>(Behaviours, "UseItem");
            preShoot = HookManager.CreateHooks<ItemBehaviour, Func<Player, Vector2, Vector2, ProjectileRef, int, float, bool>>(Behaviours, "PreShoot");

            effects = HookManager.CreateHooks<ItemBehaviour, Action<Player, int, EquipSlotKind>>(Behaviours, "Effects");
            vanityEffects = HookManager.CreateHooks<ItemBehaviour, Action<Player, int, EquipSlotKind>>(Behaviours, "VanityEffects");
        }
        public override void Clear ()
        {
            base.Clear();

            canUse   = null;
            useItem  = null;
            preShoot = null;

            effects = vanityEffects = null;
        }

        public bool CanUse(Player p)
        {
            var r = HookManager.Call(canUse, p);

            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
        public Maybe<bool?> UseItem(Player p)
        {
            var r = HookManager.Call(canUse, p);

            // boo.
            return r.Length == 0 ? Maybe<bool?>.Nothing : Maybe.Just(r.Cast<bool?>().Aggregate((a, b) => a.Bind(v => b.Bind(v_ => v && v_) ?? v)));
        }
        public bool PreShoot(Player plr, Vector2 p, Vector2 v, ProjectileRef pr, int d, float kb)
        {
            var r = HookManager.Call(preShoot, plr, p, v, pr, d, kb);

            return r.Length == 0 || r.All(Convert.ToBoolean);
        }

        public void Effects(Player p, int slot, EquipSlotKind kind)
        {
            HookManager.Call(effects, p, slot, kind);
        }
        public void VanityEffects(Player p, int slot, EquipSlotKind kind)
        {
            HookManager.Call(vanityEffects, p, slot, kind);
        }
    }
}
