using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Terraria.ID;

namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
        public static Dictionary<int, int> TestItems  = new Dictionary<int, int>();
        public static Dictionary<int, int> TestNpcs   = new Dictionary<int, int>();
        public static Dictionary<int, int> TestBosses = new Dictionary<int, int>();

        public override void OnAllModsLoaded()
        {
            TestItems = new Dictionary<int, int>
            {
                { ItemID.Gel                       , 999 },
                { ItemDef.Defs["Ant"         ].Type,   1 },
                { ItemDef.Defs["Pizzantzioli"].Type,   1 },
                { ItemDef.Defs["Pizzant"     ].Type,   1 },
            };

            TestNpcs = new Dictionary<int, int>
            {
                { NpcDef.Defs["PizzaNPC"].Type, 1 },
            };

            TestBosses = new Dictionary<int, int>
            {
                { NpcDef.Defs["PizzaBoss"].Type, 1 },
            };
        }

        protected override ContentHandler CreateContentHandler()
        {
            return new Content();
        }
    }
}
