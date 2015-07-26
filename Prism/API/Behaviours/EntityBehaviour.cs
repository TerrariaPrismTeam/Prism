using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;

namespace Prism.API.Behaviours
{
    public abstract class EntityBehaviour<TEntity> : HookContainer
        where TEntity : class
    {
        public ModDef  Mod
        {
            get;
            internal set;
        }
        public TEntity Entity
        {
            get;
            internal set;
        }

        [Hook]
        public virtual void OnInit     () { }
        [Hook]
        public virtual void OnDestroyed() { }
    }
}
