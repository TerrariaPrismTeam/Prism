using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Hooks;

namespace Prism.API.Behaviours
{
    public abstract class EntityBehaviour<TEntity> : IOBehaviour
        where TEntity : class
    {
        public TEntity Entity
        {
            get;
            internal set;
        }

        [Hook]
        public virtual void OnInit     () { }

        [Hook]
        public virtual bool PreDestroyed()
        {
            return true;
        }
        [Hook]
        public virtual void OnDestroyed() { }
    }
}
