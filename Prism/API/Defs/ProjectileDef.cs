using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Defs.Handlers;
using Terraria;

namespace Prism.API.Defs
{
    public class ProjectileDef : EntityDef<ProjectileBehaviour, Projectile>
    {
        /// <summary>
        /// Gets ProjectileDefs by their type number.
        /// </summary>
        public struct ByTypeIndexer
        {
            public ProjectileDef this[int type]
            {
                get
                {
                    return Handler.ProjectileDef.DefsByType[type];
                }
            }
        }
        /// <summary>
        /// Gets ProjectileDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameIndexer
        {
            public ProjectileDef this[string projectileInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == PrismApi.VanillaString || modInternalName == PrismApi.TerrariaString)
                        return Handler.ProjectileDef.VanillaDefsByName[projectileInternalName];

                    return ModData.ModsFromInternalName[modInternalName].ProjectileDefs[projectileInternalName];
                }
            }
        }

        /// <summary>
        /// Gets ProjectileDefs by their type number.
        /// </summary>
        public static ByTypeIndexer ByType
        {
            get
            {
                return new ByTypeIndexer();
            }
        }
        /// <summary>
        /// Gets ProjectileDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameIndexer ByName
        {
            get
            {
                return new ByNameIndexer();
            }
        }

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
        /// Gets or sets the type of damage this projectile inflicts.
        /// </summary>
        public virtual ProjectileDamageType DamageType
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
        /// Gets or sets the scale at which the projectile's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>Projectile.scale</remarks>
        public virtual float Scale
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
        
        public ProjectileDef() { }

        public ProjectileDef(
            #region arguments
            Func<ProjectileBehaviour> newBehaviour = null,
            int damage = 0,
            int width = 16,
            int height = 16,           
            int alpha = 0,            
            int frameCount = 1,
            float scale = 1f,                        
            ProjectileAiStyle aiStyle = ProjectileAiStyle.None,

            int trailCacheLength = 0,

            Func<Texture2D> getTex         = null
            #endregion
            )
        {
            CreateBehaviour = newBehaviour ?? (() => null);

            Damage = damage;
            Width = width;
            Height = height;
            Alpha = alpha;                        
            TotalFrameCount = frameCount;
            Scale = scale;
            AiStyle = aiStyle;

            TrailCacheLength = trailCacheLength;

            GetTexture         = getTex         ?? (() => null);
        }
    }
}
