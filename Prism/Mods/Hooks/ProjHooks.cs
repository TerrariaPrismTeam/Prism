using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.BHandlers;
using Terraria;

namespace Prism.Mods.Hooks
{
    static class ProjHooks
    {
        internal static int OnNewProjectile(float x, float y, float vx, float vy, int t, int d, float kb, int o, float ai0, float ai1)
        {
            var id = Projectile.RealNewProjectile(x, y, vx, vy, t, d, kb, o, ai0, ai1);

            var pr = Main.projectile[id];

            var h = pr.P_BHandler as ProjBHandler;

            if (h != null)
                h.OnInit();

            return id;
        }

        internal static void OnUpdate(Projectile pr, int id)
        {
            if (!pr.active)
                return;

            pr.whoAmI = id;
            pr.numUpdates = pr.extraUpdates;

            var bh = pr.P_BHandler as ProjBHandler;

            if (bh != null && bh.PreUpdate())
            {
                pr.RealUpdate(id);
                bh.OnUpdate();
            }
        }
        internal static void OnAI(Projectile pr)
        {
            var bh = pr.P_BHandler as ProjBHandler;

            if (bh != null && bh.PreAI())
            {
                pr.RealAI();
                bh.OnAI();
            }
        }

        internal static void OnKill(Projectile pr)
        {
            var bh = pr.P_BHandler as ProjBHandler;

            if (bh != null && bh.PreDestroyed())
            {
                pr.RealKill();
                bh.OnDestroyed();
            }
        }

        internal static void OnDrawProj(Main m, int prid)
        {
            var pr = Main.projectile[prid];
            var bh = pr.P_BHandler as ProjBHandler;

            if (bh != null && bh.PreDraw(Main.spriteBatch))
            {
                m.RealDrawProj(prid);
                bh.OnDraw(Main.spriteBatch);
            }
        }
    }
}
