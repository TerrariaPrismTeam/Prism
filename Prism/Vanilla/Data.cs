using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;

namespace Prism.Vanilla
{
    public static class Data
    {        
        public static EntityConstData Item       = new EntityConstData(typeof(Item)      , typeof(ItemID)      , MiscValues.MinItemNetType, ItemID.Count);
        public static EntityConstData NPC        = new EntityConstData(typeof(NPC)       , typeof(NPCID)       , MiscValues.MinItemNetType, NPCID.Count);
        public static EntityConstData Projectile = new EntityConstData(typeof(Projectile), typeof(ProjectileID), 0                        , ProjectileID.Count);

        public static EntityConstDataEnumerator Entity
        {
            get
            {
                return new EntityConstDataEnumerator();
            }
        }

        public struct EntityConstDataEnumerator
        {
            public EntityConstData this[Type type]
            {
                get
                {
                    EntityConstData data = (type == typeof(Item) ? Item
                                         : (type == typeof(NPC) ? NPC
                                         : (type == typeof(Projectile) ? Projectile
                                         : null)));

                    if (data == null)
                        throw new ArgumentException("Type '" + type.Name + "' is not that of a Terraria entity.", "type");

                    return data;
                }
            }
        }
    }
}
