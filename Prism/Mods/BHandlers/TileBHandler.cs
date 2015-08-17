using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    sealed class TileBHandler : EntityBHandler<TileBehaviour, Tile>
    {
        IEnumerable<Action> onUpdate;

        public override void Create()
        {
            base.Create();

            onUpdate = HookManager.CreateHooks<TileBehaviour, Action>(Behaviours, "OnUpdate");
        }
        public override void Clear ()
        {
            base.Clear ();

            onUpdate = null;
        }

        public void OnUpdate()
        {
            HookManager.Call(onUpdate);
        }
    }
}
