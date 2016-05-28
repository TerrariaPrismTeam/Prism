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

        /// <summary>
        /// A hook called when the behaviour is created, not always when the entity itself is created.
        /// </summary>
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
