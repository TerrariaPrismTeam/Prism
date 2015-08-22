using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;

namespace Prism.API.Behaviours
{
    public abstract class MountBehaviour : HookContainer
    {
        public ModDef Mod
        {
            get;
            internal set;
        }
    }
}
