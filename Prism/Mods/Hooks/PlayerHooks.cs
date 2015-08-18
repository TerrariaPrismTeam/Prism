using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.Mods.BHandlers;
using Terraria;
using Terraria.GameContent.UI.States;
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

        internal static PlayerFileData OnGetFiledata(string path, bool cloud)
        {
            var fd = Player.RealGetFileData(path, cloud);

            if (fd.Player != null)
            {
                var bh = AttachBHandler(fd.Player);

                if (bh != null)
                    bh.OnLoaded();
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
            ItemHooks.OnUpdateEquips(p, _);

            var bh = p.P_BHandler as PlayerBHandler;

            if (bh != null)
                bh.MidUpdate();
        }
    }
}
