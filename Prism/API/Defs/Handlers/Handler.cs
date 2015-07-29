using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods.Behaviours;
using System.Reflection;

namespace Prism.Defs.Handlers
{
    public static class Handler
    {
        public static ItemDefHandler       ItemDef       = new ItemDefHandler      ();
        public static NpcDefHandler        NpcDef        = new NpcDefHandler       ();
        public static ProjectileDefHandler ProjectileDef = new ProjectileDefHandler();
    }    
}