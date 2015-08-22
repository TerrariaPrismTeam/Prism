using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Prism.Util;
using Terraria;
using Terraria.DataStructures;

namespace Prism.Mods.BHandlers
{
    class MountBHandler : IHookManager
    {
        internal List<MountBehaviour> behaviours = new List<MountBehaviour>();

        IEnumerable<Action<Player, bool>> onMount;
        IEnumerable<Action<Player>> onDismount, onUpdateEffects, hover;
        IEnumerable<Func<Player, bool>> preUpdateEffects;
        IEnumerable<Func  <SpriteBatch, List<DrawData>, int, Player, Vector2, Color, SpriteEffects, float, bool>> preDraw;
        IEnumerable<Action<SpriteBatch, List<DrawData>, int, Player, Vector2, Color, SpriteEffects, float      >> onDraw ;
        IEnumerable<Action<Player, int, Vector2>> updateFrame;
        IEnumerable<Func<float, int>> jumpHeight;
        IEnumerable<Func<float, float>> jumpSpeed;

        public void Create()
        {
            onMount = HookManager.CreateHooks<MountBehaviour, Action<Player, bool>>(behaviours, "OnMount");

            onDismount      = HookManager.CreateHooks<MountBehaviour, Action<Player>>(behaviours, "OnDismount"     );
            onUpdateEffects = HookManager.CreateHooks<MountBehaviour, Action<Player>>(behaviours, "OnUpdateEffects");
            hover           = HookManager.CreateHooks<MountBehaviour, Action<Player>>(behaviours, "Hover"          );

            preUpdateEffects = HookManager.CreateHooks<MountBehaviour, Func<Player, bool>>(behaviours, "PreUpdateEffects");

            preDraw = HookManager.CreateHooks<MountBehaviour, Func  <
                SpriteBatch, List<DrawData>, int, Player, Vector2, Color, SpriteEffects, float, bool>>(behaviours, "PreDraw");
            onDraw  = HookManager.CreateHooks<MountBehaviour, Action<
                SpriteBatch, List<DrawData>, int, Player, Vector2, Color, SpriteEffects, float      >>(behaviours, "OnDraw" );

            updateFrame = HookManager.CreateHooks<MountBehaviour, Action<Player, int, Vector2>>(behaviours, "UpdateFrame");

            jumpHeight = HookManager.CreateHooks<MountBehaviour, Func<float, int  >>(behaviours, "JumpHeight");
            jumpSpeed  = HookManager.CreateHooks<MountBehaviour, Func<float, float>>(behaviours, "JumpSpeed" );
        }
        public void Clear ()
        {
            onMount = null;

            onDismount = onUpdateEffects = hover = null;

            preUpdateEffects = null;

            preDraw = null;
            onDraw  = null;

            updateFrame = null;

            jumpHeight = null;
            jumpSpeed  = null;
        }

        public void OnMount(Player p, bool faceLeft)
        {
            HookManager.Call(onMount, p, faceLeft);
        }
        public void OnDismount(Player p)
        {
            HookManager.Call(onDismount, p);
        }

        public bool PreDraw(SpriteBatch sb, List<DrawData> playerDrawData, int drawType, Player p, Vector2 pos, Color colour, SpriteEffects playerEffect, float shadow)
        {
            return HookManager.Call(preDraw, sb, playerDrawData, drawType, p, pos, colour, playerEffect, shadow).All(Convert.ToBoolean);
        }
        public void OnDraw (SpriteBatch sb, List<DrawData> playerDrawData, int drawType, Player p, Vector2 pos, Color colour, SpriteEffects playerEffect, float shadow)
        {
            HookManager.Call(onDraw, sb, playerDrawData, drawType, p, pos, colour, playerEffect, shadow);
        }

        public bool PreUpdateEffects(Player p)
        {
            return HookManager.Call(preUpdateEffects, p).All(Convert.ToBoolean);
        }
        public void OnUpdateEffects (Player p)
        {
            HookManager.Call(onUpdateEffects, p);
        }

        public void UpdateFrame(Player p, int state, Vector2 vel)
        {
            HookManager.Call(updateFrame, p, state, vel);
        }

        public void Hover(Player p)
        {
            HookManager.Call(hover, p);
        }

        public int? JumpHeight(float xVel)
        {
            var r = HookManager.Call(jumpHeight, xVel);
            return r.IsEmpty() ? (int?)null : r.Max(Convert.ToInt32);
        }
        public float? JumpSpeed(float xVel)
        {
            var r = HookManager.Call(jumpSpeed, xVel);
            return r.IsEmpty() ? (float?)null : r.Max(o => (float)o);
        }
    }
}
