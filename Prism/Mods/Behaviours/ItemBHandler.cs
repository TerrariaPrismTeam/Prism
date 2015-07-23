using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.Behaviours
{
    public sealed class ItemBHandler : EntityBHandler<ItemBehaviour, Item>
    {
        IEnumerable<Action> onUsed;

        public override void Create()
        {
            base.Create();

            onUsed = HookManager.CreateHooks<ItemBehaviour, Action>(Behaviours, "OnUsed");
        }
        public override void Clear ()
        {
            base.Clear();

            onUsed = null;
        }

        public void OnUsed()
        {
            HookManager.Call(onUsed);
        }
    }
}
