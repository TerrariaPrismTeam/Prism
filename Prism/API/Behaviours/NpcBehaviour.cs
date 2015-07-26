using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class NpcBehaviour : EntityBehaviour<NPC>
    {
        [Hook]
        public virtual void OnUpdate() { }
    }
}
