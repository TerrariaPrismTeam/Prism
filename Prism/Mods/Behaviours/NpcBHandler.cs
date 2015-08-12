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
        IEnumerable<Action> onUpdate, onAI;

        IEnumerable<Func<bool>> preAI;

        public override void Create()
        {
            base.Create();

            onUpdate = HookManager.CreateHooks<NpcBehaviour, Action>(Behaviours, "OnUpdate");
            onAI     = HookManager.CreateHooks<NpcBehaviour, Action>(Behaviours, "OnAI");

            preAI    = HookManager.CreateHooks<NpcBehaviour, Func<bool>>(Behaviours, "PreAI");
        }
        public override void Clear ()
        {
            base.Clear ();

            onUpdate = null;
            onAI     = null;

            preAI    = null;
        }

        public void OnUpdate()
        {
            HookManager.Call(onUpdate);
        }

        public void OnAI()
        {
            HookManager.Call(onAI);
        }

        public bool PreAI()
        {
            return HookManager.Call(preAI).All(v => (bool)v);
        }
    }
}
