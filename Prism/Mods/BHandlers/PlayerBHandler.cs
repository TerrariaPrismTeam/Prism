using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    sealed class PlayerBHandler : EntityBHandler<PlayerBehaviour, Player>
    {
        IEnumerable<Action>
            preUpdate, midUpdate, postUpdate,
            preItemCheck, postItemCheck,
            onCreated, onLoaded;
        IEnumerable<Action<SpriteBatch>>
            preDraw, postDraw;

        public override void Create()
        {
            base.Create();

            preUpdate  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PreUpdate" );
            midUpdate  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "MidUpdate" );
            postUpdate = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PostUpdate");

            preItemCheck  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PreItemCheck" );
            postItemCheck = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "PostItemCheck");

            onCreated = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "OnCreated");
            onLoaded  = HookManager.CreateHooks<PlayerBehaviour, Action>(Behaviours, "OnLoaded" );

            preDraw  = HookManager.CreateHooks<PlayerBehaviour, Action<SpriteBatch>>(Behaviours, "PreDraw" );
            postDraw = HookManager.CreateHooks<PlayerBehaviour, Action<SpriteBatch>>(Behaviours, "PostDraw");
        }
        public override void Clear ()
        {
            base.Clear();

            preUpdate = midUpdate = postUpdate = null;
            preItemCheck = postItemCheck = null;
            onCreated = onLoaded = null;
            preDraw = postDraw = null;
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

        public void OnCreated()
        {
            HookManager.Call(onCreated);
        }
        public void OnLoaded ()
        {
            HookManager.Call(onLoaded);
        }

        public void PreDraw (SpriteBatch sb)
        {
            HookManager.Call(preDraw , sb);
        }
        public void PostDraw(SpriteBatch sb)
        {
            HookManager.Call(postDraw, sb);
        }
    }
}
