using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;

namespace Prism.API.Defs
{
    public partial class ProjectileDef : EntityDef<ProjectileBehaviour, Projectile>
    {
        /// <summary>
        /// Gets or sets the damage this projectile inflicts.
        /// </summary>
        /// <remarks>Projectile.damage</remarks>
        public virtual int Damage
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width of this projectile.
        /// </summary>
        /// <remarks>Projectile.width</remarks>
        public virtual int Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the height of this projectile.
        /// </summary>
        /// <remarks>Projectile.height</remarks>
        public virtual int Height
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the opacity at which the projectile's sprite is rendered (0 = fully opaque, 255 = fully transparent).
        /// </summary>
        /// <remarks>Projectile.alpha</remarks>
        public virtual int Alpha
        {
            get;
            set;
        }

        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>ProjectileID.Sets.TrailCacheLength[Type]</remarks>
        public virtual int TrailCacheLength
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the total number of animation frames in this projectile's sprite.
        /// </summary>
        /// <remarks>Main.projFrames[Type]</remarks>
        public virtual int TotalFrameCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scale at which the projectile's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>Projectile.scale</remarks>
        public virtual float Scale
        {
            get;
            set;
        }

        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>ProjectileID.Sets.DontAttachHideToAlpha[Type]</remarks>
        public virtual bool DontAttachHideToAlpha
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile functions as a grappling hook.
        /// </summary>
        /// <remarks>Main.projHook[Type]</remarks>
        public virtual bool IsHook
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile is hostile.
        /// </summary>
        /// <remarks>Main.projHostile[Type]</remarks>
        public virtual bool IsHostile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile is a pet.
        /// </summary>
        /// <remarks>Main.projPet[Type]</remarks>
        public virtual bool IsPet
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile homes in on its target.
        /// </summary>
        /// <remarks>ProjectileID.Sets.Homing[Type]</remarks>
        public virtual bool IsHoming
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile is a light pet.
        /// </summary>
        /// <remarks>ProjectileID.Sets.LightPet[Type]</remarks>
        public virtual bool IsLightPet
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile is a minion that can be sacrificed upon spawning another one.
        /// </summary>
        /// <remarks>ProjectileID.Sets.MinionSacrificable[Type]</remarks>
        public virtual bool IsSacrificableMinion
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>ProjectileID.Sets.NeedsUUID[Type]</remarks>
        public virtual bool NeedsUUID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this projectile is used for Stardust Dragons.
        /// </summary>
        /// <remarks>ProjectileID.Sets.StardustDragon[Type]</remarks>
        public virtual bool IsStartustDragon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of damage this projectile inflicts.
        /// </summary>
        public virtual ProjectileDamageType DamageType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the AI style of this projectile.
        /// </summary>
        public virtual ProjectileAiStyle AiStyle
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsDescription
        /// </summary>
        /// <remarks>ProjectileID.Sets.TrailingMode[Type]</remarks>
        public virtual TrailingMode TrailingMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the projectile's texture.
        /// </summary>
        public virtual Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        public ProjectileDef(string displayName, Func<ProjectileBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            Width = Height = 16;

            Scale = 1f;

            TrailCacheLength = 10;
            TotalFrameCount = 1;

            TrailingMode = TrailingMode.None;

            GetTexture = getTexture ?? Empty<Texture2D>.Func;
        }

        public static implicit operator ProjectileRef(ProjectileDef  def)
        {
            return new ProjectileRef(def.InternalName, def.Mod.InternalName);
        }
        public static explicit operator ProjectileDef(ProjectileRef @ref)
        {
            return @ref.Resolve();
        }
    }
}
