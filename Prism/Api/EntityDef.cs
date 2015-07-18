using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;

namespace Prism.API
{
    public abstract class EntityDef
    {
        public readonly static string
            VanillaString  = "Vanilla" ,
            TerrariaString = "Terraria";

        public string InternalName
        {
            get;
            internal set;
        }
        public ModInfo Mod
        {
            get;
            internal set;
        }
        public int Type
        {
            get;
            internal set;
        }

        public virtual string DisplayName
        {
            get;
            set;
        }
    }
}
