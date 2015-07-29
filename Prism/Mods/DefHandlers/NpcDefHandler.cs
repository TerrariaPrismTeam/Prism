using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods;
using Prism.Mods.Behaviours;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    sealed class NpcDefHandler : EntityDefHandler<NpcDef, NpcBehaviour, NPC>
    {
        const int VanillaBossHeadCount = 31;

        protected override int MinVanillaID
        {
            get
            {
                return -65;
            }
        }
        protected override int MaxVanillaID
        {
            get
            {
                return NPCID.Count;
            }
        }

        internal static void OnSetDefaults(NPC n, int type, float scaleOverride)
        {
            NpcBHandler h = null;

            if (type >= NPCID.Count)
            {
                n.RealSetDefaults(0, scaleOverride);

                if (Handler.NpcDef.DefsByType.ContainsKey(type))
                {
                    var d = Handler.NpcDef.DefsByType[type];

                    n.type = n.netID = type;
                    n.width = n.height = 16;

                    Handler.NpcDef.CopyDefToEntity(d, n);

                    if (scaleOverride > -1f)
                        n.scale = scaleOverride;

                    h = new NpcBHandler();
                    if (d.CreateBehaviour != null)
                    {
                        var b = d.CreateBehaviour();

                        if (b != null)
                            h.behaviours.Add(b);
                    }

                    n.BHandler = h;
                }
            }
            else
                n.RealSetDefaults(type, scaleOverride);

            //TODO: add global hooks here (and check for null)

            if (h != null)
            {
                h.Create();
                n.BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = n;

                h.OnInit();
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen     = amt > 0                  ? Main.npcName           .Length + amt : NPCID.Count;
            int newBossLen = amt > 0 && !Main.dedServ ? Main.npcHeadBossTexture.Length + amt : VanillaBossHeadCount;

            if (!Main.dedServ)
            {
                Array.Resize(ref Main.npcTexture, newLen);
                Array.Resize(ref Main.npcHeadBossTexture, newBossLen);
            }

            Array.Resize(ref Main.npcName                      , newLen);
            Array.Resize(ref Main.NPCLoaded                    , newLen);
            Array.Resize(ref Main.npcFrameCount                , newLen);
            Array.Resize(ref Main.npcCatchable                 , newLen);
            Array.Resize(ref NPC.killCount                     , newLen); //Hardcoded 540 in NPC.ResetKillCount();
            Array.Resize(ref NPCID.Sets.AttackAverageChance    , newLen);
            Array.Resize(ref NPCID.Sets.AttackFrameCount       , newLen);
            Array.Resize(ref NPCID.Sets.AttackTime             , newLen);
            Array.Resize(ref NPCID.Sets.AttackType             , newLen);
            Array.Resize(ref NPCID.Sets.BossHeadTextures       , newLen);
            Array.Resize(ref NPCID.Sets.DangerDetectRange      , newLen);
            Array.Resize(ref NPCID.Sets.ExcludedFromDeathTally , newLen);
            Array.Resize(ref NPCID.Sets.ExtraFramesCount       , newLen);
            Array.Resize(ref NPCID.Sets.FaceEmote              , newLen);
            Array.Resize(ref NPCID.Sets.MagicAuraColor         , newLen);
            Array.Resize(ref NPCID.Sets.MPAllowedEnemies       , newLen);
            Array.Resize(ref NPCID.Sets.MustAlwaysDraw         , newLen);
            Array.Resize(ref NPCID.Sets.NeedsExpertScaling     , newLen);
            Array.Resize(ref NPCID.Sets.PrettySafe             , newLen);
            Array.Resize(ref NPCID.Sets.ProjectileNPC          , newLen);
            Array.Resize(ref NPCID.Sets.SavesAndLoads          , newLen);
            Array.Resize(ref NPCID.Sets.TechnicallyABoss       , newLen);
            Array.Resize(ref NPCID.Sets.TownCritter            , newLen);
            Array.Resize(ref NPCID.Sets.TrailCacheLength       , newLen);
        }

        protected override NPC GetVanillaEntityFromID(int id)
        {
            NPC entity = new NPC();
            entity.netDefaults(id);
            return entity;
        }

        protected override void CopyEntityToDef(NPC npc, NpcDef def)
        {
            def.InternalName        = npc.name;
            def.DisplayName         = npc.displayName;
            def.Type                = npc.type;
            def.NetID               = npc.netID;
            def.Damage              = npc.damage;
            def.Width               = npc.width;
            def.Height              = npc.height;
            def.Defense             = npc.defense;
            def.Alpha               = npc.alpha;
            def.MaxLife             = npc.lifeMax;
            def.SoundOnHit          = npc.soundHit;
            def.SoundOnDeath        = npc.soundKilled;
            def.IgnoreTileCollision = npc.noTileCollide;
            def.IgnoreGravity       = npc.noGravity;
            def.IsBoss              = npc.boss;
            def.IsTownNpc           = npc.townNPC;
            def.Scale               = npc.scale;
            def.KnockbackResistance = npc.knockBackResist;
            def.NpcSlots            = npc.npcSlots;
            def.Colour              = npc.color;
            def.Value               = new NpcValue(new CoinValue((int)(npc.value * 0.8f)),
                                                                       new CoinValue((int)(npc.value * 1.2f)));
            def.AiStyle             = (NpcAiStyle)npc.aiStyle;
            def.MaxLife             = npc.lifeMax;
            def.GetTexture          = () => Main.npcTexture[npc.type];
            def.GetBossHeadTexture  = () => Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[npc.type]];

            def.BuffImmunities.Clear();
            for (int i = 0; i < npc.buffImmune.Length; i++)
                if (npc.buffImmune[i])
                    def.BuffImmunities.Add(i);

            def.DisplayName     = Main.npcName      [def.Type];
            def.TotalFrameCount = Main.npcFrameCount[def.Type];

            def.AttackAverageChance    = NPCID.Sets.AttackAverageChance   [def.Type];
            def.AttackFrameCount       = NPCID.Sets.AttackFrameCount      [def.Type];
            def.AttackTime             = NPCID.Sets.AttackTime            [def.Type];
            def.AttackType             = NPCID.Sets.AttackType            [def.Type];
            def.DangerDetectRange      = NPCID.Sets.DangerDetectRange     [def.Type];
            def.ExtraFramesCount       = NPCID.Sets.ExtraFramesCount      [def.Type];
            def.FaceEmote              = NPCID.Sets.FaceEmote             [def.Type];
            def.PrettySafe             = NPCID.Sets.PrettySafe            [def.Type];
            def.TrailCacheLength       = NPCID.Sets.TrailCacheLength      [def.Type];
            def.ExcludedFromDeathTally = NPCID.Sets.ExcludedFromDeathTally[def.Type];
            def.IsAllowedInMP          = NPCID.Sets.MPAllowedEnemies      [def.Type];
            def.MustAlwaysDraw         = NPCID.Sets.MustAlwaysDraw        [def.Type];
            def.NeedsExpertScaling     = NPCID.Sets.NeedsExpertScaling    [def.Type];
            def.IsProjectileNPC        = NPCID.Sets.ProjectileNPC         [def.Type];
            def.SavesAndLoads          = NPCID.Sets.SavesAndLoads         [def.Type];
            def.IsTechnicallyABoss     = NPCID.Sets.TechnicallyABoss      [def.Type];
            def.IsTownCritter          = NPCID.Sets.TownCritter           [def.Type];
            def.MagicAuraColour        = NPCID.Sets.MagicAuraColor        [def.Type];
        }
        protected override void CopyDefToEntity(NpcDef def, NPC npc)
        {
            npc.name            = def.InternalName;
            npc.displayName     = def.DisplayName;
            npc.type            = def.Type;
            npc.netID           = def.NetID;
            npc.damage          = def.Damage;
            npc.width           = def.Width;
            npc.height          = def.Height;
            npc.defense         = def.Defense;
            npc.alpha           = def.Alpha;
            npc.lifeMax         = def.MaxLife;
            npc.soundHit        = def.SoundOnHit;
            npc.soundKilled     = def.SoundOnDeath;
            npc.noTileCollide   = def.IgnoreTileCollision;
            npc.noGravity       = def.IgnoreGravity;
            npc.boss            = def.IsBoss;
            npc.townNPC         = def.IsTownNpc;
            npc.scale           = def.Scale;
            npc.knockBackResist = def.KnockbackResistance;
            npc.npcSlots        = def.NpcSlots;
            npc.color           = def.Colour;
            npc.dontCountMe     = def.HasAntiRadar;
            npc.value           = Main.rand.Next(def.Value.Min.Value, def.Value.Max.Value); // close enough
            npc.aiStyle         = (int)def.AiStyle;

            for (int i = 0; i < def.BuffImmunities.Count; i++)
                npc.buffImmune[i] = true;
        }

        protected override List<LoaderError> CheckTextures(NpcDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of NpcDef " + def + " is null."));
            if (def.IsTechnicallyABoss && def.GetBossHeadTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetBossHeadTexture of NpcDef " + def + " is null."));

            return ret;
        }
        protected override List<LoaderError> LoadTextures (NpcDef def)
        {
            var ret = new List<LoaderError>();

            // def.GetTexture and GetBossHeadTexture values should be checked when it's loaded
            var t = def.GetTexture();
            if (t == null)
            {
                ret.Add(new LoaderError(def.Mod, "GetTexture return value is null for NpcDef " + def + "."));
                return ret;
            }
            Main.npcTexture[def.Type] = def.GetTexture();
            Main.NPCLoaded[def.Type] = true;

            if (def.IsTechnicallyABoss)
            {
                var bht = def.GetBossHeadTexture();
                if (bht == null)
                    ret.Add(new LoaderError(def.Mod, "GetBossHeadTexture return value is null for NpcDef " + def + "."));
                else
                {
                    // this shouldn't be -1, because new entries were allocated in ExtendArrays
                    int firstNull = Array.IndexOf(Main.npcHeadBossTexture, null);
                    Main.npcHeadBossTexture[firstNull] = def.GetBossHeadTexture();

                    NPCID.Sets.BossHeadTextures[def.Type] = firstNull;
                    def.BossHeadTextureIndex = firstNull;
                }
            }

            //TODO: npc head textures, head-in-chat-bubble textures(?)

            return ret;
        }

        protected override NpcDef CreateEmptyDefWithDisplayName(NPC npc)
        {
            return new NpcDef(Lang.npcName(npc.netID, true));
        }
        protected override string InternalNameOfEntity(NPC npc)
        {
            return npc.name;
        }
        protected override int NonNetIDTypeOfEntity(NPC npc)
        {
            return npc.type;
        }

        protected override void LoadSetProperties(NpcDef def)
        {
            Main.npcName      [def.Type] = def.DisplayName    ;
            Main.npcFrameCount[def.Type] = def.TotalFrameCount;

            NPCID.Sets.AttackAverageChance   [def.Type] = def.AttackAverageChance   ;
            NPCID.Sets.AttackFrameCount      [def.Type] = def.AttackFrameCount      ;
            NPCID.Sets.AttackTime            [def.Type] = def.AttackTime            ;
            NPCID.Sets.AttackType            [def.Type] = def.AttackType            ;
            NPCID.Sets.DangerDetectRange     [def.Type] = def.DangerDetectRange     ;
            NPCID.Sets.ExtraFramesCount      [def.Type] = def.ExtraFramesCount      ;
            NPCID.Sets.FaceEmote             [def.Type] = def.FaceEmote             ;
            NPCID.Sets.PrettySafe            [def.Type] = def.PrettySafe            ;
            NPCID.Sets.TrailCacheLength      [def.Type] = def.TrailCacheLength      ;
            NPCID.Sets.ExcludedFromDeathTally[def.Type] = def.ExcludedFromDeathTally;
            NPCID.Sets.MPAllowedEnemies      [def.Type] = def.IsAllowedInMP         ;
            NPCID.Sets.MustAlwaysDraw        [def.Type] = def.MustAlwaysDraw        ;
            NPCID.Sets.NeedsExpertScaling    [def.Type] = def.NeedsExpertScaling    ;
            NPCID.Sets.ProjectileNPC         [def.Type] = def.IsProjectileNPC       ;
            NPCID.Sets.SavesAndLoads         [def.Type] = def.SavesAndLoads         ;
            NPCID.Sets.TechnicallyABoss      [def.Type] = def.IsTechnicallyABoss    ;
            NPCID.Sets.TownCritter           [def.Type] = def.IsTownCritter         ;
            NPCID.Sets.MagicAuraColor        [def.Type] = def.MagicAuraColour       ;

            if (def.IsSkeleton && !NPCID.Sets.Skeletons.Contains(def.Type))
                NPCID.Sets.Skeletons.Add(def.Type);
        }
    }
}
