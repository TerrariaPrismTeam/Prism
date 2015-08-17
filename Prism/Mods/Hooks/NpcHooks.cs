using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.BHandlers;
using Terraria;

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

        internal static void OnUpdateNPC(NPC n, int id)
        {
            n.whoAmI = id;

            var bh = n.P_BHandler as NpcBHandler;

            if (bh != null && bh.PreUpdate())
            {
                n.RealUpdateNPC(id);
                bh.OnUpdate();
            }
        }
        internal static void OnAI(NPC n)
        {
            var bh = n.P_BHandler as NpcBHandler;

            if (bh != null && bh.PreAI())
            {
                n.RealAI();
                bh.OnAI();
            }
        }

        internal static void OnNPCLoot(NPC n)
        {
            var bh = n.P_BHandler as NpcBHandler;

            if (bh != null && bh.PreDestroyed())
            {
                n.RealNPCLoot();
                bh.OnDestroyed();
            }
        }

        internal static void OnDrawNPC(Main m, int nid, bool behindTiles)
        {
            var n = Main.npc[nid];
            var bh = n.P_BHandler as NpcBHandler;

            if (bh != null && bh.PreDraw(Main.spriteBatch, behindTiles))
            {
                m.RealDrawNPC(nid, behindTiles);
                bh.OnDraw(Main.spriteBatch, behindTiles);
            }
        }
    }
}
