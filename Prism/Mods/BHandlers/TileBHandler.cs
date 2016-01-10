using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;
using Terraria.DataStructures;

namespace Prism.Mods.BHandlers
{
    public sealed class TileBHandler : EntityBHandler<TileBehaviour, Tile>
    {
        //! NOTE: when calling a hook, set the position of the behaviour
        //!       hasTile must be set when placed/removed, too

        IEnumerable<Action>
            onUpdate, onPlaced;

        public override void Create()
        {
            base.Create();

            onUpdate = HookManager.CreateHooks<TileBehaviour, Action>(Behaviours, "OnUpdate");
            onPlaced = HookManager.CreateHooks<TileBehaviour, Action>(Behaviours, "OnPlaced");
        }
        public override void Clear ()
        {
            base.Clear ();

            onUpdate = null;
            onPlaced = null;
        }

        public void OnUpdate(Point16 pos)
        {
            foreach (var b in Behaviours)
                b.Position = pos;

            HookManager.Call(onUpdate);
        }
        public void OnPlaced(Point16 pos)
        {
            foreach (var b in Behaviours)
            {
                b.HasTile  = true;
                b.Position = pos;
            }

            HookManager.Call(onPlaced);
        }
    }
}
