using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods.Behaviours;
using Prism.Mods;

namespace Prism.Defs.Handlers
{
    public class ProjectileDefHandler : EntityDefHandler<ProjectileDef, ProjectileBehaviour, Projectile>
    {
        internal static void OnSetDefaults(Projectile p, int type)
        {
            ProjectileBHandler h = null;

            if (type >= ProjectileID.Count)
            {
                p.RealSetDefaults(0);

                if (Handler.ProjectileDef.DefsByType.ContainsKey(type))
                {
                    var d = Handler.ProjectileDef.DefsByType[type];

                    p.type = type;
                    p.width = p.height = 16;

                    Handler.ProjectileDef.CopyDefToEntity(d, p);

                    h = new ProjectileBHandler();
                    if (d.CreateBehaviour != null)
                    {
                        var b = d.CreateBehaviour();

                        if (b != null)
                            h.behaviours.Add(b);
                    }

                    p.BHandler = h;
                }
            }
            else
                p.RealSetDefaults(type);

            //TODO: add global hooks here (and check for null)

            if (h != null)
            {
                h.Create();
                p.BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = p;

                h.OnInit();
            }
        }

        public override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.projectileTexture.Length + amt : ProjectileID.Count;
            if (!Main.dedServ)
            {
                Array.Resize(ref Main.projectileTexture             , newLen);               
            }
            Array.Resize(ref Main.projFrames                        , newLen);
            Array.Resize(ref Main.projHook                          , newLen);
            Array.Resize(ref Main.projHostile                       , newLen);
            Array.Resize(ref Main.projPet                           , newLen);
            Array.Resize(ref ProjectileID.Sets.DontAttachHideToAlpha, newLen);
            Array.Resize(ref ProjectileID.Sets.Homing               , newLen);
            Array.Resize(ref ProjectileID.Sets.LightPet             , newLen);
            Array.Resize(ref ProjectileID.Sets.MinionSacrificable   , newLen);
            Array.Resize(ref ProjectileID.Sets.NeedsUUID            , newLen);
            Array.Resize(ref ProjectileID.Sets.StardustDragon       , newLen);
            Array.Resize(ref ProjectileID.Sets.TrailCacheLength     , newLen);
            Array.Resize(ref ProjectileID.Sets.TrailingMode         , newLen);
        }

        public override Projectile GetVanillaEntityFromID(int id)
        {
            Projectile proj = new Projectile();
            proj.SetDefaults(id);
            return proj;
        } 

        public override void CopyEntityToDef(Projectile entity, ProjectileDef def)
        {
            def.InternalName                                    = entity.name;
            def.Type                                            = entity.type;
            def.Damage                                          = entity.damage;
            def.Width                                           = entity.width;
            def.Height                                          = entity.height;
            def.Alpha                                           = entity.alpha;
            def.Scale                                           = entity.scale;            
            def.DamageType                                      = entity.melee ? ProjectileDamageType.Melee
                                                                : (entity.ranged ? ProjectileDamageType.Ranged
                                                                : (entity.magic ? ProjectileDamageType.Magic
                                                                : ProjectileDamageType.None));
            def.AiStyle                                         = (ProjectileAiStyle)entity.aiStyle;
            def.GetTexture                                      = () => Main.npcTexture[entity.type];

            def.TotalFrameCount                                 = Main.projFrames                             [def.Type];
            def.IsHook                                          = Main.projHook                               [def.Type];
            def.IsHostile                                       = Main.projHostile                            [def.Type];
            def.IsPet                                           = Main.projPet                                [def.Type];
            def.DontAttachHideToAlpha                           = ProjectileID.Sets.DontAttachHideToAlpha     [def.Type];
            def.IsHoming                                        = ProjectileID.Sets.Homing                    [def.Type];
            def.IsLightPet                                      = ProjectileID.Sets.LightPet                  [def.Type];
            def.IsSacrificableMinion                            = ProjectileID.Sets.MinionSacrificable        [def.Type];
            def.NeedsUUID                                       = ProjectileID.Sets.NeedsUUID                 [def.Type];
            def.IsStartustDragon                                = ProjectileID.Sets.StardustDragon            [def.Type];
            def.TrailCacheLength                                = ProjectileID.Sets.TrailCacheLength          [def.Type];
            def.TrailingMode                                    = (TrailingMode)ProjectileID.Sets.TrailingMode[def.Type];
        }

        public override void CopyDefToEntity(ProjectileDef def, Projectile entity)
        {
            entity.name                                         = def.InternalName;
            entity.type                                         = def.Type;            
            entity.damage                                       = def.Damage;
            entity.width                                        = def.Width;
            entity.height                                       = def.Height;
            entity.alpha                                        = def.Alpha;
            entity.scale                                        = def.Scale;
            entity.name                                         = def.InternalName;
            entity.melee                                        = (def.DamageType == ProjectileDamageType.Melee);
            entity.ranged                                       = (def.DamageType == ProjectileDamageType.Ranged);
            entity.magic                                        = (def.DamageType == ProjectileDamageType.Magic);                       
            entity.hostile                                      = def.IsHostile;
            entity.aiStyle                                      = (int)def.AiStyle;
            //tar.counterweight;
            //tar.arrow;
            //tar.bobber;

            Main.projFrames                          [def.Type] = def.TotalFrameCount;
            Main.projHook                            [def.Type] = def.IsHook;
            Main.projHostile                         [def.Type] = def.IsHostile;
            Main.projPet                             [def.Type] = def.IsPet;                
            ProjectileID.Sets.DontAttachHideToAlpha  [def.Type] = def.DontAttachHideToAlpha;
            ProjectileID.Sets.Homing                 [def.Type] = def.IsHoming;
            ProjectileID.Sets.LightPet               [def.Type] = def.IsLightPet;
            ProjectileID.Sets.MinionSacrificable     [def.Type] = def.IsSacrificableMinion;
            ProjectileID.Sets.NeedsUUID              [def.Type] = def.NeedsUUID;
            ProjectileID.Sets.StardustDragon         [def.Type] = def.IsStartustDragon;
            ProjectileID.Sets.TrailCacheLength       [def.Type] = def.TrailCacheLength;
            ProjectileID.Sets.TrailingMode           [def.Type] = (int)def.TrailingMode;
        }

        public override bool CheckTextures(ProjectileDef def)
        {
            return !(def.GetTexture == null);
        }

        public override void LoadTextures(ref List<LoaderError> err, ProjectileDef def)
        {
            var texture = def.GetTexture();
            if (texture == null)
                throw new ArgumentNullException("GetTexture return value is null for ProjectileDef " + def.InternalName + " from mod " + def.Mod + ".");
            Main.projectileTexture[def.Type] = def.GetTexture();
            Main.projectileLoaded[def.Type] = true;    
        }
    }
}