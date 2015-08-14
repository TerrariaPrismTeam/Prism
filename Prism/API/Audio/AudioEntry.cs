using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Defs;
using Prism.Mods;

namespace Prism.API.Audio
{
    public abstract class AudioEntry<TEntry, TRef>
        where TEntry : AudioEntry<TEntry, TRef>
        where TRef : EntityRef<TEntry>
    {
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

        public override string ToString()
        {
            return InternalName + ", Mod={" + Mod + "}";
        }

        public static explicit operator AudioEntry<TEntry, TRef>(TRef r)
        {
            return r.Resolve();
        }
    }
}
