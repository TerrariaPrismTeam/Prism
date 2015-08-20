using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;
using Terraria.GameContent.UI;

namespace Prism.Mods.Hooks
{
    static class NpcHooks
    {
        static BuffBHandler AttachBuffBHandler(NPC n, int slot, int type)
        {
            if (slot == -1)
                return null;

            BuffBHandler h = null;

            if (Handler.BuffDef.DefsByType.ContainsKey(type))
            {
                var d = Handler.BuffDef.DefsByType[type];

                if (d.CreateBehaviour != null)
                {
                    h = new BuffBHandler();

                    h.behaviours.Add(d.CreateBehaviour());
                }
            }

            var bs = ModData.mods.Values.Select(d =>
            {
                var bb = d.ContentHandler.CreateGlobalBuffBInternally();

                if (bb != null)
                    bb.Mod = d;

                return bb;
            }).Where(bb => bb != null);

            if (!bs.IsEmpty())
            {
                if (h == null)
                    h = new BuffBHandler();

                h.behaviours.AddRange(bs);
            }

            if (h != null)
                h.Create();

            n.P_BuffBHandler[slot] = h;

            return h;
        }

        static int RealAddBuff(NPC n, int type, int time, bool quiet = false)
        {
            if (n.buffImmune[type])
                return -1;

            if (!quiet)
            {
                if (Main.netMode == 1)
                    NetMessage.SendData(53, -1, -1, String.Empty, n.whoAmI, type, time);
                else if (Main.netMode == 2)
                    NetMessage.SendData(54, -1, -1, String.Empty, n.whoAmI);
            }
            for (int i = 0; i < 5; i++)
                if (n.buffType[i] == type)
                {
                    if (n.buffTime[i] < time)
                        n.buffTime[i] = time;

                    return -1;
                }

            int slot = -1;
            do
            {
                int buffSeek = -1;
                for (int j = 0; j < 5; j++)
                    if (!Main.debuff[n.buffType[j]])
                    {
                        buffSeek = j;
                        break;
                    }

                if (buffSeek == -1)
                    return -1;

                for (int k = buffSeek; k < 5; k++)
                    if (n.buffType[k] == 0)
                    {
                        slot = k;
                        break;
                    }

                if (slot == -1)
                    n.DelBuff(buffSeek);

            } while (slot == -1);

            n.buffType[slot] = type;
            n.buffTime[slot] = time;

            return slot;
        }

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
                try
                {
                    n.RealUpdateNPC(id);
                }
                catch (IndexOutOfRangeException ioore)
                {
                    if (ioore.TargetSite.DeclaringType != typeof(EmoteBubble)) // this somehow, sometimes crashes.
                        throw new IndexOutOfRangeException(ioore.Message, ioore);
                }
                bh.OnUpdate();
            }
        }

        internal static void OnFindFrame(NPC n)
        {
            n.RealFindFrame();

            var bh = n.P_BHandler as NpcBHandler;

            if (bh != null)
                bh.FindFrame();
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

        internal static void OnBuffEffects(NPC n)
        {
            for (int i = 0; i < n.P_BuffBHandler.Length; i++)
            {
                var bh = n.P_BuffBHandler[i] as BuffBHandler;

                if (bh == null)
                    continue;

                bh.NpcEffects(n, n.buffTime[i], i);
            }
        }
        internal static void OnAddBuff(NPC n, int type, int time, bool quiet)
        {
            var s = RealAddBuff(n, type, time, quiet);

            if (s == -1)
                return;

            var h = AttachBuffBHandler(n, s, type);

            if (h == null)
                return;

            h.OnAddedNpc(n, time, s);
        }
    }
}
