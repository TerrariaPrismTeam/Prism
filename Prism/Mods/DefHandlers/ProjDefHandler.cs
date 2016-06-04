using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Debugging;
using Prism.Mods.BHandlers;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    sealed class ProjDefHandler : EEntityDefHandler<ProjectileDef, ProjectileBehaviour, Projectile>
    {
        protected override Type IDContainerType
        {
            get
            {
                return typeof(ProjectileID);
            }
        }

        internal static void OnSetDefaults(Projectile p, int type)
        {
            if (ModLoader.Reloading)
            {
                p.RealSetDefaults(type);

                if (!FillingVanilla)
                    Logging.LogWarning("Tried to call SetDefaults on a Projectile while [re|un]?loading mods.");

                return;
            }

            ProjBHandler h = null; // will be set to <non-null> only if a behaviour handler will be attached

            p.RealSetDefaults(type);

            if (Handler.ProjDef.DefsByType.ContainsKey(type))
            {
                var d = Handler.ProjDef.DefsByType[type];

                p.type = type;
                p.width = p.height = 16;

                Handler.ProjDef.CopyDefToEntity(d, p);

                if (d.CreateBehaviour != null)
                {
                    var b = d.CreateBehaviour();

                    if (b != null)
                    {
                        h = new ProjBHandler();

                        b.Mod = d.Mod == PrismApi.VanillaInfo ? null : ModData.mods[d.Mod];

                        h.behaviours.Add(b);
                    }
                }

                p.active = true;
            }

            var bs = ModData.mods.Values
                .Select(m => new KeyValuePair<ModDef, ProjectileBehaviour>(m, m.ContentHandler.CreateGlobalProjBInternally()))
                .Where(kvp => kvp.Value != null)
                .Select(kvp =>
            {
                kvp.Value.Mod = kvp.Key;
                return kvp.Value;
            });

            if (!bs.IsEmpty() && h == null)
                h = new ProjBHandler();

            if (h != null)
            {
                h.behaviours.AddRange(bs);

                h.Create();
                p.P_BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = p;

                h.OnInit();
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Main.projectileLoaded.Length + amt: ProjectileID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.projectileTexture, newLen);

            Array.Resize(ref Main.projectileLoaded                  , newLen);
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

        protected override Projectile GetVanillaEntityFromID(int id)
        {
            var proj = new Projectile();
            proj.SetDefaults(id);
            return proj;
        }
        protected override ProjectileDef NewDefFromVanilla(Projectile proj)
        {
            return new ProjectileDef(proj.name, getTexture: () => Main.projectileTexture[proj.type]);
        }

        protected override void CopyEntityToDef(Projectile proj, ProjectileDef def)
        {
            def.DisplayName  = proj.name  ;
            def.Type         = proj.type  ;
            def.Damage       = proj.damage;
            def.Knockback    = proj.knockBack;
            def.Width        = proj.width ;
            def.Height       = proj.height;
            def.Alpha        = proj.alpha ;
            def.Scale        = proj.scale ;
            def.AiStyle      = (ProjectileAiStyle)proj.aiStyle;
            def.GetTexture   = () => Main.npcTexture[proj.type];
            def.DamageType   = proj.melee
                                ? ProjectileDamageType.Melee
                                : proj.ranged
                                    ? ProjectileDamageType.Ranged
                                    : proj.magic
                                        ? ProjectileDamageType.Magic
                                        : proj.minion
                                            ? ProjectileDamageType.Summoner
                                            : proj.thrown
                                                ? ProjectileDamageType.Thrown
                                                : ProjectileDamageType.None;

            def.TotalFrameCount = Main.projFrames [def.Type];
            def.IsHook          = Main.projHook   [def.Type];
            def.IsHostile       = Main.projHostile[def.Type];
            def.IsPet           = Main.projPet    [def.Type];

            def.DontAttachHideToAlpha = ProjectileID.Sets.DontAttachHideToAlpha     [def.Type];
            def.IsHoming              = ProjectileID.Sets.Homing                    [def.Type];
            def.IsLightPet            = ProjectileID.Sets.LightPet                  [def.Type];
            def.IsSacrificableMinion  = ProjectileID.Sets.MinionSacrificable        [def.Type];
            def.NeedsUUID             = ProjectileID.Sets.NeedsUUID                 [def.Type];
            def.IsStartustDragon      = ProjectileID.Sets.StardustDragon            [def.Type];
            def.TrailCacheLength      = ProjectileID.Sets.TrailCacheLength          [def.Type];
            def.TrailingMode          = (TrailingMode)ProjectileID.Sets.TrailingMode[def.Type];
        }
        protected override void CopyDefToEntity(ProjectileDef def, Projectile proj)
        {
            proj.name       = def.DisplayName;
            proj.type       = def.Type;
            proj.damage     = def.Damage;
            proj.knockBack  = def.Knockback;
            proj.width      = def.Width;
            proj.height     = def.Height;
            proj.alpha      = def.Alpha;
            proj.scale      = def.Scale;
            proj.melee      = def.DamageType == ProjectileDamageType.Melee;
            proj.ranged     = def.DamageType == ProjectileDamageType.Ranged;
            proj.magic      = def.DamageType == ProjectileDamageType.Magic;
            proj.minion     = def.DamageType == ProjectileDamageType.Summoner;
            proj.thrown     = def.DamageType == ProjectileDamageType.Thrown;
            proj.hostile    = def.IsHostile;
            proj.aiStyle    = (int)def.AiStyle;
            //proj.counterweight
            //proj.arrow
            //proj.bobber
        }

        protected override List<LoaderError> CheckTextures(ProjectileDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of ProjectileDef " + def + " is null."));

            return ret;
        }
        protected override List<LoaderError> LoadTextures (ProjectileDef def)
        {
            var ret = new List<LoaderError>();

            var t = def.GetTexture();
            if (t == null)
            {
                ret.Add(new LoaderError(def.Mod, "GetTexture return value is null for ProjectileDef " + def + "."));
                return ret;
            }

            Main.projectileTexture[def.Type] = t;
            Main.projectileLoaded [def.Type] = true;

            return ret;
        }

        protected override int GetRegularType(Projectile proj)
        {
            return proj.type;
        }

        protected override void CopySetProperties(ProjectileDef def)
        {
            Main.projFrames [def.Type] = def.TotalFrameCount;
            Main.projHook   [def.Type] = def.IsHook         ;
            Main.projHostile[def.Type] = def.IsHostile      ;
            Main.projPet    [def.Type] = def.IsPet          ;

            ProjectileID.Sets.DontAttachHideToAlpha[def.Type] = def.DontAttachHideToAlpha;
            ProjectileID.Sets.Homing               [def.Type] = def.IsHoming             ;
            ProjectileID.Sets.LightPet             [def.Type] = def.IsLightPet           ;
            ProjectileID.Sets.MinionSacrificable   [def.Type] = def.IsSacrificableMinion ;
            ProjectileID.Sets.NeedsUUID            [def.Type] = def.NeedsUUID            ;
            ProjectileID.Sets.StardustDragon       [def.Type] = def.IsStartustDragon     ;
            ProjectileID.Sets.TrailCacheLength     [def.Type] = def.TrailCacheLength     ;
            ProjectileID.Sets.TrailingMode         [def.Type] = (int)def.TrailingMode    ;
        }
    }
}
