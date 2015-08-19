using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    class BuffBHandler : IHookManager
    {
        internal List<BuffBehaviour> behaviours = new List<BuffBehaviour>();

        IEnumerable<Action<Player, int, int>>    effects, added   ;
        IEnumerable<Action<NPC   , int, int>> npcEffects, addedNpc;

        public void Create()
        {
            effects    = HookManager.CreateHooks<BuffBehaviour, Action<Player, int, int>>(behaviours, "Effects"   );
            npcEffects = HookManager.CreateHooks<BuffBehaviour, Action<NPC   , int, int>>(behaviours, "NpcEffects");

            added    = HookManager.CreateHooks<BuffBehaviour, Action<Player, int, int>>(behaviours, "OnAdded"   );
            addedNpc = HookManager.CreateHooks<BuffBehaviour, Action<NPC   , int, int>>(behaviours, "OnAddedNpc");
        }
        public void Clear ()
        {
            effects    = null;
            npcEffects = null;

            added    = null;
            addedNpc = null;
        }

        public void Effects(Player p, int tl, int s)
        {
            HookManager.Call(effects, p, tl, s);
        }
        public void NpcEffects(NPC n, int tl, int s)
        {
            HookManager.Call(npcEffects, n, tl, s);
        }

        public void OnAdded   (Player p, int tl, int s)
        {
            HookManager.Call(added   , p, tl, s);
        }
        public void OnAddedNpc(NPC    n, int tl, int s)
        {
            HookManager.Call(addedNpc, n, tl, s);
        }
    }
}
