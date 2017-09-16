using System;
using System.Collections.Generic;
using System.Linq;

using MountSets = Terraria.ID.MountID.Sets;

namespace Prism.API.Defs
{
    public static class MountID
    {
        public static bool[] Cart
        {
            get
            {
                return MountSets.Cart;
            }
            set
            {
                MountSets.Cart = value;
            }
        }

        public const int
            None         = -1,
            Rudolph      =  0,
            Bunny        =  1,
            Pigron       =  2,
            Slime        =  3,
            Turtle       =  4,
            Bee          =  5,
            Minecart     =  6,
            UFO          =  7,
            Drill        =  8,
            Scutlix      =  9,
            Unicorn      = 10,
            MinecartMech = 11,
            CuteFishron  = 12,
            MinecartWood = 13,
            Basilisk     = 14,
            Count        = 15;

#if DEBUG
        static MountID()
        {
            if (Count != Terraria.ID.MountID.Count)
                throw new Exception("UPDATE Prism.API.Defs.MountID.Count!");
        }
#endif
    }
}

