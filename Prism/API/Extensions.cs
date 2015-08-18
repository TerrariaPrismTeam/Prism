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
        static TWanted GetBehaviour<TBehaviour, TEntity, TWanted>(EntityBHandler<TBehaviour, TEntity> handler)
            where TEntity : class
            where TBehaviour : EntityBehaviour<TEntity>
            where TWanted : TBehaviour
        {
            if (handler == null)
                return null;

            return (TWanted)handler.behaviours.FirstOrDefault(b => b is TWanted);
        }

        public static TBehaviour GetBehaviour<TBehaviour>(this Item       i )
            where TBehaviour : ItemBehaviour
        {
            return GetBehaviour<ItemBehaviour, Item, TBehaviour>(i.P_BHandler as ItemBHandler);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this NPC        n )
            where TBehaviour : NpcBehaviour
        {
            return GetBehaviour<NpcBehaviour, NPC, TBehaviour>(n.P_BHandler as NpcBHandler);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this Projectile pr)
            where TBehaviour : ProjectileBehaviour
        {
            return GetBehaviour<ProjectileBehaviour, Projectile, TBehaviour>(pr.P_BHandler as ProjBHandler);
        }
        public static TBehaviour GetBehaviour<TBehaviour>(this Player     p )
            where TBehaviour : PlayerBehaviour
        {
            return GetBehaviour<PlayerBehaviour, Player, TBehaviour>(p.P_BHandler as PlayerBHandler);
        }
    }
}
