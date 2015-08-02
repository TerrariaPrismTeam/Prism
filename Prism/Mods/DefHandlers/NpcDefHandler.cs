using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.Behaviours;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    sealed class NpcDefHandler : EntityDefHandler<NpcDef, NpcBehaviour, NPC>
    {
        const int VanillaBossHeadCount = 31;

        protected override Type IDContainerType
        {
            get
            {
                return typeof(NPCID);
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

                    n.life = n.lifeMax; //! BEEP BOOP

                    if (scaleOverride > -1f)
                        n.scale = scaleOverride;

                    h = new NpcBHandler();
                    if (d.CreateBehaviour != null)
                    {
                        var b = d.CreateBehaviour();

                        if (b != null)
                            h.behaviours.Add(b);
                    }
                }
            }
            else
                n.RealSetDefaults(type, scaleOverride);

            if (h != null)
            {
                h.behaviours.AddRange(ModData.mods.Values.Select(m => m.CreateGlobalNpcBInternally()).Where(b => b != null));

                h.Create();
                n.BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = n;

                h.OnInit();
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.npcName.Length : NPCID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.npcTexture, newLen);

            Array.Resize(ref Main.npcName                     , newLen);
            Array.Resize(ref Main.NPCLoaded                   , newLen);
            Array.Resize(ref Main.npcFrameCount               , newLen);
            Array.Resize(ref Main.npcCatchable                , newLen);
            Array.Resize(ref NPC.killCount                    , newLen); //Hardcoded 540 in NPC.ResetKillCount();
            Array.Resize(ref NPCID.Sets.AttackAverageChance   , newLen);
            Array.Resize(ref NPCID.Sets.AttackFrameCount      , newLen);
            Array.Resize(ref NPCID.Sets.AttackTime            , newLen);
            Array.Resize(ref NPCID.Sets.AttackType            , newLen);
            Array.Resize(ref NPCID.Sets.DangerDetectRange     , newLen);
            Array.Resize(ref NPCID.Sets.ExcludedFromDeathTally, newLen);
            Array.Resize(ref NPCID.Sets.ExtraFramesCount      , newLen);
            Array.Resize(ref NPCID.Sets.FaceEmote             , newLen);
            Array.Resize(ref NPCID.Sets.MagicAuraColor        , newLen);
            Array.Resize(ref NPCID.Sets.MPAllowedEnemies      , newLen);
            Array.Resize(ref NPCID.Sets.MustAlwaysDraw        , newLen);
            Array.Resize(ref NPCID.Sets.NeedsExpertScaling    , newLen);
            Array.Resize(ref NPCID.Sets.PrettySafe            , newLen);
            Array.Resize(ref NPCID.Sets.ProjectileNPC         , newLen);
            Array.Resize(ref NPCID.Sets.SavesAndLoads         , newLen);
            Array.Resize(ref NPCID.Sets.TechnicallyABoss      , newLen);
            Array.Resize(ref NPCID.Sets.TownCritter           , newLen);
            Array.Resize(ref NPCID.Sets.TrailCacheLength      , newLen);
            Array.Resize(ref NPCID.Sets.BossHeadTextures      , newLen);
        }

        protected override NPC GetVanillaEntityFromID(int id)
        {
            NPC entity = new NPC();
            entity.netDefaults(id);
            return entity;
        }
        protected override NpcDef NewDefFromVanilla(NPC npc)
        {
            return new NpcDef(Lang.npcName(npc.netID, true), getTexture: () => Main.npcTexture[npc.type]);
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

            def.BuffImmunities.Clear();
            for (int i = 0; i < npc.buffImmune.Length; i++)
                if (npc.buffImmune[i])
                    def.BuffImmunities.Add(i);

            def.DisplayName                         = Main.npcName                     [def.Type];
            def.FrameCount                          = Main.npcFrameCount               [def.Type];

            def.TownConfig.AverageAttackChance      = NPCID.Sets.AttackAverageChance   [def.Type];
            def.TownConfig.AttackFrameCount         = NPCID.Sets.AttackFrameCount      [def.Type];
            def.TownConfig.AttackTime               = NPCID.Sets.AttackTime            [def.Type];
            def.TownConfig.AttackType               = (TownNpcAttackType)NPCID.Sets.AttackType[def.Type];
            def.TownConfig.DangerDetectRadius       = NPCID.Sets.DangerDetectRange     [def.Type];
            def.TownConfig.ExtraFramesCount         = NPCID.Sets.ExtraFramesCount      [def.Type];
            def.TownConfig.ChatIcon                 = (ChatBubbleIconIndex)
                                                        NPCID.Sets.FaceEmote           [def.Type];
            def.TownConfig.SafetyRadius             = NPCID.Sets.PrettySafe            [def.Type];
            def.TownConfig.MagicAuraColour          = NPCID.Sets.MagicAuraColor        [def.Type];
            def.TrailCacheLength                    = NPCID.Sets.TrailCacheLength      [def.Type];
            def.ExcludedFromDeathTally              = NPCID.Sets.ExcludedFromDeathTally[def.Type];
            def.IsSummonableBoss                    = NPCID.Sets.MPAllowedEnemies      [def.Type];
            def.MustAlwaysDraw                      = NPCID.Sets.MustAlwaysDraw        [def.Type];
            def.NeedsExpertScaling                  = NPCID.Sets.NeedsExpertScaling    [def.Type];
            def.IsProjectileNPC                     = NPCID.Sets.ProjectileNPC         [def.Type];
            def.SavesAndLoads                       = NPCID.Sets.SavesAndLoads         [def.Type];
            def.IsTechnicallyABoss                  = NPCID.Sets.TechnicallyABoss      [def.Type];
            def.IsTownCritter                       = NPCID.Sets.TownCritter           [def.Type];
        }
        public void RegisterBossHeadTexture(NpcDef npc)
        {
            NPCID.Sets.BossHeadTextures[npc.Type] = Main.npcHeadBossTexture.Length;
            int newLen = Main.npcHeadBossTexture.Length + 1;
            Array.Resize(ref Main.npcHeadBossTexture, newLen);
            Main.npcHeadBossTexture[newLen - 1] = npc.GetBossHeadTexture();
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
            npc.dontCountMe     = def.NotOnRadar;
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
            if ((def.IsBoss || def.IsTechnicallyABoss) && def.GetBossHeadTexture == null)
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

            if (def.IsBoss || def.IsTechnicallyABoss)
            {
                var bht = def.GetBossHeadTexture();
                if (bht == null)
                    ret.Add(new LoaderError(def.Mod, "GetBossHeadTexture return value is null for NpcDef " + def + "."));
                else
                {
                    RegisterBossHeadTexture(def);
                }
            }

            return ret;
        }

        protected override int GetRegularType(NPC npc)
        {
            return npc.type;
        }

        protected override void CopySetProperties(NpcDef def)
        {
            Main.npcName                     [def.Type] = def.DisplayName;
            Main.npcFrameCount               [def.Type] = def.FrameCount;

            NPCID.Sets.AttackAverageChance   [def.Type] = def.TownConfig.AverageAttackChance;
            NPCID.Sets.AttackFrameCount      [def.Type] = def.TownConfig.AttackFrameCount;
            NPCID.Sets.AttackTime            [def.Type] = def.TownConfig.AttackTime;
            NPCID.Sets.AttackType            [def.Type] = (int)def.TownConfig.AttackType;
            NPCID.Sets.DangerDetectRange     [def.Type] = def.TownConfig.DangerDetectRadius;
            NPCID.Sets.ExtraFramesCount      [def.Type] = def.TownConfig.ExtraFramesCount;
            NPCID.Sets.FaceEmote             [def.Type] = (int)def.TownConfig.ChatIcon;
            NPCID.Sets.PrettySafe            [def.Type] = def.TownConfig.SafetyRadius;
            NPCID.Sets.TrailCacheLength      [def.Type] = def.TrailCacheLength;
            NPCID.Sets.ExcludedFromDeathTally[def.Type] = def.ExcludedFromDeathTally;
            NPCID.Sets.MPAllowedEnemies      [def.Type] = def.IsSummonableBoss;
            NPCID.Sets.MustAlwaysDraw        [def.Type] = def.MustAlwaysDraw;
            NPCID.Sets.NeedsExpertScaling    [def.Type] = def.NeedsExpertScaling;
            NPCID.Sets.ProjectileNPC         [def.Type] = def.IsProjectileNPC;
            NPCID.Sets.SavesAndLoads         [def.Type] = def.SavesAndLoads;
            NPCID.Sets.TechnicallyABoss      [def.Type] = def.IsTechnicallyABoss;
            NPCID.Sets.TownCritter           [def.Type] = def.IsTownCritter;
            NPCID.Sets.MagicAuraColor        [def.Type] = def.TownConfig.MagicAuraColour;

            NPCID.Sets.BossHeadTextures      [def.Type] = -1; //No more EoC heads on map for literally everything \o/
                                                              //Note that these still get set later in LoadTextures()

            if (def.IsSkeleton && !NPCID.Sets.Skeletons.Contains(def.Type))
                NPCID.Sets.Skeletons.Add(def.Type);
        }
    }
}
