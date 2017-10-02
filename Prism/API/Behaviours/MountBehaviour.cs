using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Mods;
using Prism.Mods.Hooks;
using Terraria;
using Terraria.DataStructures;

namespace Prism.API.Behaviours
{
    public abstract class MountBehaviour : HookContainer
    {
        public ModDef Mod
        {
            get;
            internal set;
        }
        public Mount Mount
        {
            get;
            internal set;
        }

        internal void Adopt(ModInfo inf)
        {
            ModDef m;
            if (ModData.mods.TryGetValue(inf, out m))
                Mod = m;
        }

        [Hook]
        public virtual void OnMount   (Player p, bool faceLeft) { }
        [Hook]
        public virtual void OnDismount(Player p) { }

        [Hook]
        public virtual bool PreDraw(SpriteBatch sb, List<DrawData> playerDrawData, int drawType, Player p, Vector2 pos, Color colour, SpriteEffects playerEffect, float shadow)
        {
            return true;
        }
        [Hook]
        public virtual void OnDraw (SpriteBatch sb, List<DrawData> playerDrawData, int drawType, Player p, Vector2 pos, Color colour, SpriteEffects playerEffect, float shadow) { }

        [Hook]
        public virtual bool PreUpdateEffects(Player p)
        {
            return true;
        }
        [Hook]
        public virtual void OnUpdateEffects (Player p) { }

        [Hook]
        public virtual void UpdateFrame(Player p, int state, Vector2 vel) { }

        [Hook]
        public virtual void Hover(Player p) { }

        [Hook]
        public virtual int JumpHeight(float xVelocity)
        {
            return Mount.data.jumpHeight;
        }
        [Hook]
        public virtual float JumpSpeed(float xVelocity)
        {
            return Mount.data.jumpSpeed;
        }
    }
}
