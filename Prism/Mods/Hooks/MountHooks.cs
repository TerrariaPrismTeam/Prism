using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;
using Terraria.DataStructures;

namespace Prism.Mods.Hooks
{
    static class MountHooks
    {
        static MountBHandler AttachBHandler(Mount m, int type)
        {
            MountBHandler h = null;

            var def = type == -1 ? null : Handler.MountDef.DefsByType[type];

            if (def != null && def.CreateBehaviour != null)
            {
                var b = def.CreateBehaviour();

                if (b != null)
                {
                    h = new MountBHandler();

                    b.Mod = ModData.mods[def.Mod];
                    b.Mount = m;

                    h.behaviours.Add(b);
                }
            }

            var bs = ModData.mods.Values.Select(d =>
            {
                var mb = d.ContentHandler.CreateGlobalMountBInternally();

                if (mb != null)
                    mb.Mod = d;

                return mb;
            }).Where(bb => bb != null);

            if (!bs.IsEmpty())
            {
                if (h == null)
                    h = new MountBHandler();

                h.behaviours.AddRange(bs);
            }

            if (h != null)
                h.Create();

            m.P_BHandler = h;

            return h;
        }

        internal static void OnSetMount(Mount m, int type, Player p, bool faceLeft)
        {
            m.RealSetMount(type, p, faceLeft);

            var h = AttachBHandler(m, type);

            if (h != null)
                h.OnMount(p, faceLeft);
        }
        internal static void OnDismount(Mount m, Player p)
        {
            var bh = m.P_BHandler as MountBHandler;

            if (bh != null)
                bh.OnDismount(p);

            m.RealDismount(p);

            if (bh != null)
            {
                bh.Clear();

                m.P_BHandler = bh = null;
            }
        }

        internal static void OnDraw(Mount m, List<DrawData> playerDrawData, int drawType, Player p, Vector2 pos, Color colour, SpriteEffects playerEffect, float shadow)
        {
            var bh = m.P_BHandler as MountBHandler;

            if (bh == null || bh.PreDraw(Main.spriteBatch, playerDrawData, drawType, p, pos, colour, playerEffect, shadow))
            {
                m.RealDraw(playerDrawData, drawType, p, pos, colour, playerEffect, shadow);

                if (bh != null)
                    bh.OnDraw(Main.spriteBatch, playerDrawData, drawType, p, pos, colour, playerEffect, shadow);
            }
        }

        internal static bool OnHover(Mount m, Player p)
        {
            var bh = m.P_BHandler as MountBHandler;

            var r = m.RealHover(p);

            if (bh != null)
                bh.Hover(p);

            return r;
        }

        internal static int OnJumpHeight(Mount m, float xVel)
        {
            var bh = m.P_BHandler as MountBHandler;

            var r = m.RealJumpHeight(xVel);

            return bh != null ? bh.JumpHeight(xVel) ?? r : r;
        }
        internal static float OnJumpSpeed(Mount m, float xVel)
        {
            var bh = m.P_BHandler as MountBHandler;

            var r = m.RealJumpSpeed(xVel);

            return bh != null ? bh.JumpSpeed(xVel) ?? r : r;
        }

        internal static void OnUpdateFrame(Mount m, Player p, int state, Vector2 vel)
        {
            var bh = m.P_BHandler as MountBHandler;

            m.RealUpdateFrame(p, state, vel);

            if (bh != null)
                bh.UpdateFrame(p, state, vel);
        }
        internal static void OnUpdateEffects(Mount m, Player p)
        {
            var bh = m.P_BHandler as MountBHandler;

            if (bh == null || bh.PreUpdateEffects(p))
            {
                m.RealUpdateEffects(p);

                if (bh != null)
                    bh.OnUpdateEffects(p);
            }
        }
    }
}
