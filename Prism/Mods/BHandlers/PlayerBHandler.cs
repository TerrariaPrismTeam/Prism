using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    sealed class PlayerBHandler : EntityBHandler<PlayerBehaviour, Player>
    {
        IEnumerable<Action> preUpdate, midUpdate, postUpdate, preItemCheck, postItemCheck;

        public override void Create()
        {
            base.Create();

            preUpdate  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PreUpdate" );
            midUpdate  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "MidUpdate" );
            postUpdate = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PostUpdate");

            preItemCheck  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PreItemCheck" );
            postItemCheck = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PostItemCheck");
        }
        public override void Clear ()
        {
            base.Clear();

            preUpdate = midUpdate = postUpdate = null;
            preItemCheck = postItemCheck = null;
        }

        public void PreUpdate ()
        {
            HookManager.Call(preUpdate);
        }
        public void MidUpdate ()
        {
            HookManager.Call(midUpdate);
        }
        public void PostUpdate()
        {
            HookManager.Call(postUpdate);
        }

        public void PreItemCheck ()
        {
            HookManager.Call(preItemCheck);
        }
        public void PostItemCheck()
        {
            HookManager.Call(postItemCheck);
        }
    }
}
