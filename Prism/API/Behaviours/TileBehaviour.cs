using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;
using Terraria;
using Terraria.DataStructures;

namespace Prism.API.Behaviours
{
    public abstract class TileBehaviour : EntityBehaviour<Tile>
    {
        public Point16 Position
        {
            get;
            internal set;
        }

        [Hook]
        public virtual void OnUpdate() { }
    }
}
