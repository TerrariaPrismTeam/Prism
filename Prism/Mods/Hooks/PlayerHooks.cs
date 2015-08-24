using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.IO;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.UI;

namespace Prism.Mods.Hooks
{
    static class PlayerHooks
    {
        static PlayerBHandler AttachBHandler(Player p)
        {
            var bs = ModData.mods.Values.Select(d =>
            {
                var pb = d.ContentHandler.CreatePlayerBInternally();

                if (pb != null)
                {
                    pb.Mod    = d;
                    pb.Entity = p;
                }

                return pb;
            }).Where(pb => pb != null);

            if (bs.Count() == 0)
                return null;

            var bh = new PlayerBHandler();
            bh.behaviours.AddRange(bs);

            bh.Create();

            p.P_BHandler = bh;

            return bh;
        }
        static BuffBHandler AttachBuffBHandler(Player p, int slot, int type)
        {
            if (slot == -1)
                return null;

            var obh = p.P_BuffBHandler[slot] as BuffBHandler;
            Dictionary<string, BinBuffer> data = obh == null ? null : obh.data;

            BuffBHandler h = null;

            if (Handler.BuffDef.DefsByType.ContainsKey(type))
            {
                var d = Handler.BuffDef.DefsByType[type];

                if (d.CreateBehaviour != null)
                {
                    var b = d.CreateBehaviour();

                    if (b != null)
                    {
                        h = new BuffBHandler();

                        b.Mod = d.Mod == PrismApi.VanillaInfo ? null : ModData.mods[d.Mod];

                        h.behaviours.Add(b);
                    }
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
            {
                h.data = data;

                h.Create();
            }

            p.P_BuffBHandler[slot] = h;

            return h;
        }

        static int RealAddBuff(Player p, int type, int time, bool quiet = true)
        {
            if (p.buffImmune[type])
                return -1;

            if (Main.expertMode && p.whoAmI == Main.myPlayer && (type == 20 || type == 22 || type == 23 || type == 24 || type == 30 || type == 31 || type == 32 || type == 33 || type == 35 || type == 36 || type == 39 || type == 44 || type == 46 || type == 47 || type == 69 || type == 70 || type == 80))
                time = (int)(Main.expertDebuffTime * time);

            if (!quiet && Main.netMode == 1)
            {
                bool doesntHaveTheBuff = true;
                for (int i = 0; i < Player.maxBuffs; i++)
                    if (p.buffType[i] == type)
                    {
                        doesntHaveTheBuff = false;
                        break;
                    }

                if (doesntHaveTheBuff)
                    NetMessage.SendData(55, -1, -1, String.Empty, p.whoAmI, type, time);
            }

            for (int i = 0; i < Player.maxBuffs; i++)
                if (p.buffType[i] == type)
                {
                    if (type == BuffID.ManaSickness)
                    {
                        p.buffTime[i] += time;

                        if (p.buffTime[i] > Player.manaSickTimeMax)
                        {
                            p.buffTime[i] = Player.manaSickTimeMax;
                            return -1;
                        }
                    }
                    else if (p.buffTime[i] < time)
                        p.buffTime[i] = time;

                    return -1;
                }

            if (Main.vanityPet[type] || Main.lightPet[type])
                for (int i = 0; i < Player.maxBuffs; i++)
                {
                    if (Main.vanityPet[type] && Main.vanityPet[p.buffType[i]])
                        p.DelBuff(i);
                    if (Main.lightPet [type] && Main.lightPet [p.buffType[i]])
                        p.DelBuff(i);
                }

            int slot = -1;
            do
            {
                int buffSeek = -1;

                for (int i = 0; i < Player.maxBuffs; i++)
                    if (!Main.debuff[p.buffType[i]])
                    {
                        buffSeek = i;
                        break;
                    }

                if (buffSeek == -1)
                    return -1;

                for (int i = buffSeek; i < Player.maxBuffs; i++)
                    if (p.buffType[i] == 0)
                    {
                        slot = i;
                        break;
                    }

                if (slot == -1)
                    p.DelBuff(buffSeek);
            } while (slot == -1);

            p.buffType[slot] = type;
            p.buffTime[slot] = time;

            if (Main.meleeBuff[type])
                for (int i = 0; i < Player.maxBuffs; i++)
                    if (i != slot && Main.meleeBuff[p.buffType[i]])
                        p.DelBuff(i);

            return slot;
        }

        internal static PlayerFileData OnGetFiledata(string path, bool cloud)
        {
            var fd = Player.RealGetFileData(path, cloud);

            if (fd.Player != null)
            {
                var obh = fd.Player.P_BHandler as PlayerBHandler;
                Dictionary<string, BinBuffer> data = obh == null ? null : obh.data;

                var bh = AttachBHandler(fd.Player);

                if (bh != null)
                {
                    bh.data = data;

                    bh.OnLoaded();
                }
            }

            return fd;
        }
        internal static void OnNewCharacterClick(UICharacterSelect state, UIMouseEvent evt, UIElement listeningElement)
        {
            state.RealNewCharacterClick(evt, listeningElement);

            var bh = AttachBHandler(Main.PendingPlayer);

            if (bh != null)
                bh.OnCreated();
        }

        internal static void OnItemCheck(Player p, int i)
        {
            var bh = p.P_BHandler as PlayerBHandler;

            if (bh != null)
                bh.PreItemCheck ();

            p .RealItemCheck(i);

            if (bh != null)
                bh.PostItemCheck();
        }

        internal static void OnEnterWorld(Player p)
        {
            var bh = p.P_BHandler as PlayerBHandler;

            if (bh != null)
                bh.OnInit();
        }
        internal static void OnKillMe(Player p, double dmg, int hd, bool pvp, string dt)
        {
            var bh = p.P_BHandler as PlayerBHandler;

            if (bh != null)
                bh.PreDestroyed();

            p.RealKillMe(dmg, hd, pvp, dt);

            if (bh != null)
                bh.OnDestroyed();
        }

        internal static void OnDrawPlayer(Main m, Player p, Vector2 pos, float r, Vector2 o, float shadow)
        {
            var bh = p.P_BHandler as PlayerBHandler;

            if (bh != null)
                bh.PreDraw(Main.spriteBatch);

            m.RealDrawPlayer(p, pos, r, o, shadow);

            if (bh != null)
                bh.PostDraw(Main.spriteBatch);
        }

        internal static void OnUpdate(Player p, int id)
        {
            var bh = p.P_BHandler as PlayerBHandler;

            var oldId = p.whoAmI;
            p.whoAmI = id;

            if (bh != null)
                bh.PreUpdate();

            p.whoAmI = oldId;
            p.RealUpdate(id);

            if (bh != null)
                bh.PostUpdate();
        }
        internal static void OnMidUpdate(Player p, int _)
        {
            var bh = p.P_BHandler as PlayerBHandler;

            if (bh != null)
                bh.MidUpdate();
        }

        internal static void OnUpdateBuffs(Player p, int _)
        {
            p.RealUpdateBuffs(_);

            for (int i = 0; i < p.P_BuffBHandler.Length; i++)
            {
                var bh = p.P_BuffBHandler[i] as BuffBHandler;

                if (bh == null)
                    continue;

                bh.Effects(p, p.buffTime[i], i);
            }
        }
        internal static void OnAddBuff(Player p, int type, int time, bool quiet)
        {
            var s = RealAddBuff(p, type, time, quiet);

            if (s == -1)
                return;

            var h = AttachBuffBHandler(p, s, type);

            if (h == null)
                return;

            h.OnAdded(p, time, s);
        }
    }
}
