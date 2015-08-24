using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    public sealed class NpcBHandler : EntityBHandler<NpcBehaviour, NPC>
    {
        IEnumerable<Func<bool>> preUpdate, preAI;
        IEnumerable<Action> onUpdate, onAI, findFrame;

        IEnumerable<Func<SpriteBatch, bool, bool>> preDraw;
        IEnumerable<Action<SpriteBatch, bool>> onDraw;

        public override void Create()
        {
            base.Create();

            preUpdate = HookManager.CreateHooks<NpcBehaviour, Func<bool>>(Behaviours, "PreUpdate");
            onUpdate  = HookManager.CreateHooks<NpcBehaviour, Action    >(Behaviours, "OnUpdate" );
            preAI     = HookManager.CreateHooks<NpcBehaviour, Func<bool>>(Behaviours, "PreAI"    );
            onAI      = HookManager.CreateHooks<NpcBehaviour, Action    >(Behaviours, "OnAI"     );

            preDraw = HookManager.CreateHooks<NpcBehaviour, Func  <SpriteBatch, bool, bool>>(Behaviours, "PreDraw");
            onDraw  = HookManager.CreateHooks<NpcBehaviour, Action<SpriteBatch, bool      >>(Behaviours, "OnDraw" );

            findFrame = HookManager.CreateHooks<NpcBehaviour, Action>(Behaviours, "FindFrame");
        }
        public override void Clear ()
        {
            base.Clear ();

            preUpdate = null;
            onUpdate  = null;
            preAI     = null;
            onAI      = null;

            preDraw = null;
            onDraw  = null;
        }

        public bool PreUpdate()
        {
            var r = HookManager.Call(preUpdate);

            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
        public void OnUpdate ()
        {
            HookManager.Call(onUpdate);
        }

        public bool PreAI()
        {
            var r = HookManager.Call(preAI);

            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
        public void OnAI ()
        {
            HookManager.Call(onAI);
        }

        public bool PreDraw(SpriteBatch sb, bool bt)
        {
            var r = HookManager.Call(preDraw, sb, bt);

            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
        public void OnDraw (SpriteBatch sb, bool bt)
        {
            HookManager.Call(onDraw, sb, bt);
        }

        public void FindFrame()
        {
            HookManager.Call(findFrame);
        }
    }
}
