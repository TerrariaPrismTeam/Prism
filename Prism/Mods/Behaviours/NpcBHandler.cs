using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.Behaviours
{
    public sealed class NpcBHandler : EntityBHandler<NpcBehaviour, NPC>
    {
        IEnumerable<Action> onUpdate;

        public override void Create()
        {
            base.Create();

            onUpdate = HookManager.CreateHooks<NpcBehaviour, Action>(Behaviours, "OnUsed");
        }
        public override void Clear ()
        {
            base.Clear ();

            onUpdate = null;
        }

        public void OnUpdate()
        {
            HookManager.Call(onUpdate);
        }
    }
}
