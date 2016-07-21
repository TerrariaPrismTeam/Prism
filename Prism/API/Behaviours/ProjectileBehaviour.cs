using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class ProjectileBehaviour : EntityBehaviour<Projectile>
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
        public virtual bool PreDraw(SpriteBatch sb)
        {
            return true;
        }
        [Hook]
        public virtual void OnDraw(SpriteBatch sb) { }
        [Hook]
        public virtual bool IsColliding(Rectangle projRect, Rectangle targetRect) { return projRect.Intersects(targetRect); }
    }
}
