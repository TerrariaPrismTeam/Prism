using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.BHandlers;
using Terraria;

namespace Prism.API
{
    public static class Extensions
    {
        static TWanted GetBehaviour<TBehaviour, TWanted>(IOBHandler<TBehaviour> handler)
            where TBehaviour : IOBehaviour
            where TWanted : TBehaviour
        {
            if (handler == null)
                return null;

            return (TWanted)handler.behaviours.FirstOrDefault(b => b is TWanted);
        }

        public static TBehaviour GetBuffBehaviour<TBehaviour>(this Player p, int index)
            where TBehaviour : BuffBehaviour
        {
            if (p.P_BuffBHandler != null)
                return GetBehaviour<BuffBehaviour, TBehaviour>(p.P_BuffBHandler[index] as BuffBHandler);

            return null;
        }
        public static TBehaviour GetBuffBehaviour<TBehaviour>(this NPC    n, int index)
            where TBehaviour : BuffBehaviour
        {
            if (n.P_BuffBHandler != null)
                return GetBehaviour<BuffBehaviour, TBehaviour>(n.P_BuffBHandler[index] as BuffBHandler);

            return null;
        }

        public static TBehaviour GetBehaviour<TBehaviour>(this Item       i )
            where TBehaviour : ItemBehaviour
        {
            return GetBehaviour<ItemBehaviour, TBehaviour>(i.P_BHandler as ItemBHandler);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this Mount      m )
            where TBehaviour : MountBehaviour
        {
            var bh = m.P_BHandler as MountBHandler;

            if (bh == null)
                return null;

            return (TBehaviour)bh.behaviours.FirstOrDefault(b => b is TBehaviour);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this NPC        n )
            where TBehaviour : NpcBehaviour
        {
            return GetBehaviour<NpcBehaviour, TBehaviour>(n.P_BHandler as NpcBHandler);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this Projectile pr)
            where TBehaviour : ProjectileBehaviour
        {
            return GetBehaviour<ProjectileBehaviour, TBehaviour>(pr.P_BHandler as ProjBHandler);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this Player     p )
            where TBehaviour : PlayerBehaviour
        {
            return GetBehaviour<PlayerBehaviour, TBehaviour>(p.P_BHandler as PlayerBHandler);
        }
    }
}
