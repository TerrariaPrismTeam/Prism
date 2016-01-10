using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;
using Terraria;
using Terraria.DataStructures;

namespace Prism.API.Behaviours
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TileSpecificInstanceAttribute : Attribute { }

    public abstract class TileBehaviour : EntityBehaviour<Tile>
    {
        public bool HasTile
        {
            get;
            internal set;
        }
        public Point16 Position
        {
            get;
            internal set;
        }

        [Hook]
        public virtual void OnUpdate() { }
        [Hook]
        public virtual void OnPlaced() { }
    }
}
