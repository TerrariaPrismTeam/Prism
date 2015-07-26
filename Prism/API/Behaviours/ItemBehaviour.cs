using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class ItemBehaviour : EntityBehaviour<Item>
    {
        [Hook]
        public virtual void OnUsed() { }
    }
}
