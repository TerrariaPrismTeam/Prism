using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.Behaviours;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    /// <summary>
    /// Handles all the <see cref="NpcDef"/>s.
    /// </summary>
    static class ProjectileDefHandler
    {
        const int VanillaBossAmount = 31;

        static int nextType = ProjectileID.Count;
        internal static Dictionary<int   , ProjectileDef> DefFromType        = new Dictionary<int   , ProjectileDef>();
        internal static Dictionary<string, ProjectileDef> VanillaDefFromName = new Dictionary<string, ProjectileDef>();

        /// <summary>
        /// Extends the vanilla arrays through which the game iterates for various type checks.
        /// </summary>
        /// <param name="amt">The amount by which to extend the arrays.</param>
        static void ExtendArrays(int amt = 1)
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

        /// <summary>
        /// Resets the loaded NPCs.
        /// </summary>
        internal static void Reset()
        {
            nextType = ProjectileID.Count;
            DefFromType.Clear();
            ExtendArrays(0);
        }
        /// <summary>
        /// Loads the NPCs into the specified Dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="Dictionary{TKey, TValue}"/> to load the items into.</param>
        internal static IEnumerable<LoaderError> Load(Dictionary<string, ProjectileDef> dict)
        {
            var ret = new List<LoaderError>();

            ExtendArrays(dict.Count);

            foreach (var v in dict.Values)
            {
                if (v.GetTexture == null && !Main.dedServ)
                    ret.Add(new LoaderError(v.Mod, "GetTexture is null."));
                else
                {
                    int type = nextType++;

                    v.Type = type;
                    DefFromType.Add(type, v);

                    LoadSetProperties(v);

                    if (!Main.dedServ)
                        LoadTextures(v);
                }
            }

            return ret;
        }

        /// <summary>
        /// Adds all the original vanilla projectiles.
        /// </summary>
        internal static void FillVanilla()
        {
            for (int i = -65; i < ProjectileID.Count; i++)
            {
                if (i == 0)
                    continue;

                Projectile p = new Projectile();
                p.RealSetDefaults(i);

                ProjectileDef def = new ProjectileDef();

                def.InternalName = p.name;
                def.Type = p.type;

                CopyProjectileToDef(def, p);

                DefFromType.Add(i, def);
                VanillaDefFromName.Add(p.name, def);
            }
        }

        internal static void OnSetDefaults(Projectile p, int type)
        {
            ProjectileBHandler h = null;

            if (type >= ProjectileID.Count)
            {
                p.RealSetDefaults(0);

                if (DefFromType.ContainsKey(type))
                {
                    var d = DefFromType[type];

                    p.type = type;
                    p.width = p.height = 16;

                    CopyDefToProjectile(p, d);

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

        static void LoadTextures     (ProjectileDef def)
        {
            // def.GetTexture and GetBossHeadTexture values should be checked when it's loaded
            var texture = def.GetTexture();
            if (texture == null)
                throw new ArgumentNullException("GetTexture return value is null for ProjectileDef " + def.InternalName + " from mod " + def.Mod + ".");
            Main.projectileTexture[def.Type] = def.GetTexture();
            Main.projectileLoaded[def.Type] = true;       
        }
        static void LoadSetProperties(ProjectileDef def)
        {
            // assuming space is allocated in the ExtendArrays call


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

        // set properties aren't copied, type/netid is preserved
        static void CopyDefToProjectile(Projectile    tar, ProjectileDef source)
        {
            tar.damage = source.Damage;
            tar.width = source.Width;
            tar.height = source.Height;
            tar.alpha = source.Alpha;
            tar.scale = source.Scale;            
            tar.name = source.InternalName;

            switch (source.DamageType)
            {
                case DamageType.Melee:
                    tar.melee = true;
                    break;
                case DamageType.Ranged:
                    tar.ranged = true;
                    break;
                case DamageType.Magic:
                    tar.magic = true;
                    break;
                case DamageType.Minion:
                    tar.minion = true;
                    break;
                case DamageType.Thrown:
                    tar.thrown = true;
                    break;
                    // None -> all false
            }

            //tar.counterweight;
            //tar.arrow;
            //tar.bobber;
            tar.hostile = source.IsHostile;
            tar.aiStyle = (int)source.AiStyle;
        }
        static void CopyProjectileToDef(ProjectileDef tar, Projectile    source)
        {
            tar.Damage = source.damage;
            tar.Width = source.width;
            tar.Height = source.height;
            tar.Alpha = source.alpha;
            tar.Scale = source.scale;            
            tar.InternalName = source.name;

            if (source.melee)
                tar.DamageType = DamageType.Melee;
            else if (source.ranged)
                tar.DamageType = DamageType.Ranged;
            else if (source.magic)
                tar.DamageType = DamageType.Magic;
            else if (source.minion)
                tar.DamageType = DamageType.Minion;
            else if (source.thrown)
                tar.DamageType = DamageType.Thrown;

            tar.AiStyle = (ProjectileAiStyle)source.aiStyle;

            tar.GetTexture = () => Main.npcTexture[source.type];
        }
    }
}
