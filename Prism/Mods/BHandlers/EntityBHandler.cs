using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;

namespace Prism.Mods.BHandlers
{
    public abstract class EntityBHandler<TBehaviour, TEntity> : IHookManager
        where TEntity : class
        where TBehaviour : EntityBehaviour<TEntity>
    {
        internal List<TBehaviour> behaviours = new List<TBehaviour>();

        IEnumerable<Action> onInit, onDestroyed;

        public IEnumerable<TBehaviour> Behaviours
        {
            get
            {
                return behaviours;
            }
        }

        public virtual void Create()
        {
            onInit      = HookManager.CreateHooks<TBehaviour, Action>(behaviours, "OnInit"     );
            onDestroyed = HookManager.CreateHooks<TBehaviour, Action>(behaviours, "OnDestroyed");
        }
        public virtual void Clear ()
        {
            onInit = onDestroyed = null;
        }

        public void OnInit     ()
        {
            HookManager.Call(onInit     );
        }
        public void OnDestroyed()
        {
            HookManager.Call(onDestroyed);
        }
    }
}
