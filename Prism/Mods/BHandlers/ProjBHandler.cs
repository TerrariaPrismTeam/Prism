using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    public sealed class ProjBHandler : EntityBHandler<ProjectileBehaviour, Projectile>
    {
        IEnumerable<Func<bool>> preUpdate, preAI;
        IEnumerable<Action> onUpdate, onAI;

        IEnumerable<Func<SpriteBatch, bool>> preDraw;
        IEnumerable<Action<SpriteBatch>> onDraw;

        public override void Create()
        {
            base.Create();

            preUpdate = HookManager.CreateHooks<ProjectileBehaviour, Func<bool>>(Behaviours, "PreUpdate");
            onUpdate  = HookManager.CreateHooks<ProjectileBehaviour, Action    >(Behaviours, "OnUpdate" );
            preAI     = HookManager.CreateHooks<ProjectileBehaviour, Func<bool>>(Behaviours, "PreAI"    );
            onAI      = HookManager.CreateHooks<ProjectileBehaviour, Action    >(Behaviours, "OnAI"     );

            preDraw = HookManager.CreateHooks<ProjectileBehaviour, Func<SpriteBatch, bool>>(Behaviours, "PreDraw");
            onDraw  = HookManager.CreateHooks<ProjectileBehaviour, Action<SpriteBatch    >>(Behaviours, "OnDraw" );
        }
        public override void Clear ()
        {
            base.Clear();

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

        public bool PreDraw(SpriteBatch sb)
        {
            var r = HookManager.Call(preDraw, sb);

            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
        public void OnDraw (SpriteBatch sb)
        {
            HookManager.Call(onDraw, sb);
        }
    }
}
