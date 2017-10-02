using System;
using System.Collections.Generic;
using System.Linq;
using Prism.IO;
using Prism.Mods;
using Prism.Mods.Hooks;

namespace Prism.API.Behaviours
{
    public abstract class IOBehaviour : HookContainer
    {
        public ModDef Mod
        {
            get;
            internal set;
        }

        internal void Adopt(ModInfo inf)
        {
            ModDef m;
            if (ModData.mods.TryGetValue(inf, out m))
                Mod = m;
        }

        [Hook]
        public virtual void Save(BinBuffer bb) { }
        [Hook]
        public virtual void Load(BinBuffer bb) { }
    }
}
