using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class NpcBehaviour : EntityBehaviour<NPC>
    {
        [Hook]
        public virtual bool PreUpdate()
        {
            return true;
        }
        [Hook]
        public virtual void OnUpdate() { }
        [Hook]
        public virtual bool PreAI()
        {
            return true;
        }
        [Hook]
        public virtual void OnAI() { }

        [Hook]
        public virtual bool PreDraw(SpriteBatch sb, bool behindTiles)
        {
            return true;
        }
        [Hook]
        public virtual void OnDraw (SpriteBatch sb, bool behindTiles) { }

        [Hook]
        public virtual void FindFrame() { }
    }
}
