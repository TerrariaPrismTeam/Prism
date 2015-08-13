using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Behaviours;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Hooks
{
    static class NpcHooks
    {
        internal static int OnNewNPC(int x, int y, int type, int start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255)
        {
            var id = NPC.RealNewNPC(x, y, type, start, ai0, ai1, ai2, ai3, target);

            var n = Main.npc[id];

            var h = n.P_BHandler as NpcBHandler;

            if (h != null)
                h.OnInit();

            return id;
        }

        internal static void OnAI(NPC n)
        {
            var bh = ((NpcBHandler)n.P_BHandler);
            if (n.P_BHandler != null && bh.PreAI())
            {
                bh.OnAI();
                n.RealAI();
            }            
        }
    }
}
