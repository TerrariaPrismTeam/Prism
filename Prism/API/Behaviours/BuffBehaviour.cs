using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class BuffBehaviour : HookContainer
    {
        public ModDef Mod
        {
            get;
            internal set;
        }

        [Hook]
        public virtual void Effects(Player p, int timeLeft, int slot) { }
        [Hook]
        public virtual void NpcEffects(NPC n, int timeLeft, int slot) { }

        [Hook]
        public virtual void OnAdded(Player p, int timeLeft, int slot) { }
        [Hook]
        public virtual void OnAddedNpc(Player p, int timeLeft, int slot) { }
    }
}
