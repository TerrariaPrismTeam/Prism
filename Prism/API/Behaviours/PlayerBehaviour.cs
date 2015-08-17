using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class PlayerBehaviour : EntityBehaviour<Player>
    {
        // all the 'Pre'-hooks are intended to be voids.
        // it's too dangerous to make them bools imo

        [Hook]
        public virtual void PreUpdate() { }
        [Hook]
        public virtual void MidUpdate() { }
        [Hook]
        public virtual void PostUpdate() { }

        [Hook]
        public virtual void PreItemCheck() { }
        [Hook]
        public virtual void PostItemCheck() { }
    }
}
