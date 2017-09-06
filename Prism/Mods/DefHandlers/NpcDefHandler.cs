using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    sealed partial class NpcDefHandler : EEntityDefHandler<NpcDef, NpcBehaviour, NPC>
    {
        const int VanillaBossHeadCount = 31;

        protected override Type IDContainerType
        {
            get
            {
                return typeof(NPCID);
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Main.npcFrameCount.Length + amt : NPCID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.npcTexture, newLen);

            Array.Resize(ref Lang._npcNameCache, newLen);

            Array.Resize(ref Main.NPCLoaded    , newLen);
            Array.Resize(ref Main.npcFrameCount, newLen);
            Array.Resize(ref Main.npcCatchable , newLen);

            Array.Resize(ref NPC.killCount, newLen); //Hardcoded 540 in NPC.ResetKillCount();

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
            var entity = new NPC();
            entity.SetDefaultsFromNetId(id);
            return entity;
        }
        protected override NpcDef NewDefFromVanilla(NPC npc)
        {
            return new NpcDef(new ObjectName(Lang.GetNPCNameValue(npc.netID)),
                    getTexture: () => Main.npcTexture[npc.type]);
        }

        protected override void CopyEntityToDef(NPC npc, NpcDef def)
        {
            def.DisplayName         = new ObjectName(npc.GivenOrTypeName);
            def.Type                = npc.type;
            def.NetID               = npc.netID;
            def.Damage              = npc.damage;
            def.Width               = npc.width;
            def.Height              = npc.height;
            def.Defense             = npc.defense;
            def.Alpha               = npc.alpha;
            def.MaxLife             = npc.lifeMax;
            def.IgnoreTileCollision = npc.noTileCollide;
            def.IgnoreGravity       = npc.noGravity;
            def.IsBoss              = npc.boss;
            def.IsTownNpc           = npc.townNPC;
            def.IsFriendly          = npc.friendly;
            def.DrawBehindTiles     = npc.behindTiles;
            def.Scale               = npc.scale;
            def.KnockbackResistance = npc.knockBackResist;
            def.NpcSlots            = npc.npcSlots;
            def.Colour              = npc.color;
            def.Value               = new NpcValue(new CoinValue((int)npc.value));
            def.AiStyle             = (NpcAiStyle)npc.aiStyle;
            def.MaxLife             = npc.lifeMax;
            def.GetTexture          = () => Main.npcTexture[npc.type];
            def.IsImmortal          = npc.immortal;
          //def.SoundOnHit          = npc.P_SoundOnHit   as SfxRef != null ? (SfxRef)npc.P_SoundOnHit : new SfxRef("NpcHit", variant: npc.soundHit);
          //def.SoundOnDeath        = npc.P_SoundOnDeath as SfxRef != null ? (SfxRef)npc.P_SoundOnDeath : new SfxRef("NpcKilled", variant: npc.soundKilled);
            def.TimeLeft            = npc.timeLeft;
            def.AlwaysUpdateInMP    = npc.netAlways;
            def.ImmuneToLava        = npc.lavaImmune;

            def.BuffImmunities.Clear();
            for (int i = 0; i < npc.buffImmune.Length; i++)
                if (npc.buffImmune[i])
                    def.BuffImmunities.Add(new BuffRef(i));
            def.CaughtAsItem = new ItemRef(npc.catchItem);

            def.IsChaseable = npc.chaseable     ;
            def.IsImmune    = npc.dontTakeDamage;
            def.Rarity      = npc.rarity        ;

            def.TownConfig = new TownNpcConfig(() => Main.npcHeadTexture[NPC.TypeToHeadIndex(npc.type)])
            {
                HeadId = NPC.TypeToHeadIndex(npc.type)
            };

            if (npc.P_Music != null && npc.P_Music is BgmRef)
                def.Music = (BgmRef)npc.P_Music;

            def.FrameCount                          = Main.npcFrameCount               [def.Type];

            def.TownConfig.AverageAttackChance      = NPCID.Sets.AttackAverageChance   [def.Type];
            def.TownConfig.AttackFrameCount         = NPCID.Sets.AttackFrameCount      [def.Type];
            def.TownConfig.AttackTime               = NPCID.Sets.AttackTime            [def.Type];
            def.TownConfig.AttackType               = (TownNpcAttackType)
                                                        NPCID.Sets.AttackType          [def.Type];
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
        protected override void CopyDefToEntity(NpcDef def, NPC npc)
        {
            npc._givenName      = def.DisplayName.ToString();
            npc.type            = def.Type;
            npc.netID           = def.NetID;
            npc.damage          = def.Damage;
            npc.width           = def.Width;
            npc.height          = def.Height;
            npc.defense         = def.Defense;
            npc.alpha           = def.Alpha;
            npc.lifeMax         = def.MaxLife;
            npc.noTileCollide   = def.IgnoreTileCollision;
            npc.noGravity       = def.IgnoreGravity;
            npc.boss            = def.IsBoss;
            npc.townNPC         = def.IsTownNpc;
            npc.friendly        = def.IsFriendly;
            npc.behindTiles     = def.DrawBehindTiles;
            npc.scale           = def.Scale;
            npc.knockBackResist = def.KnockbackResistance;
            npc.npcSlots        = def.NpcSlots;
            npc.color           = def.Colour;
            npc.dontCountMe     = def.NotOnRadar;
            npc.value           = (def.Value.Min.Value + def.Value.Max.Value) / 2; //Main.rand.Next(def.Value.Min.Value, def.Value.Max.Value); // close enough
            npc.aiStyle         = (int)def.AiStyle;
            npc.immortal        = def.IsImmortal;
            npc.timeLeft        = def.TimeLeft;
            npc.netAlways       = def.AlwaysUpdateInMP;
            npc.lavaImmune      = def.ImmuneToLava;

            npc.P_SoundOnHit   = def.SoundOnHit  ;
            npc.P_SoundOnDeath = def.SoundOnDeath;
          //npc.soundHit       = def.SoundOnHit   == null ? 0 : def.SoundOnHit  .VariantID;
          //npc.soundKilled    = def.SoundOnDeath == null ? 0 : def.SoundOnDeath.VariantID;

            for (int i = 0; i < def.BuffImmunities.Count; i++)
                npc.buffImmune[i] = true;

            npc.catchItem = def.CaughtAsItem == null ? (short)0 : (short)def.CaughtAsItem.Resolve().NetID;

            npc.P_Music = def.Music;

            npc.chaseable      = def.IsChaseable;
            npc.dontTakeDamage = def.IsImmune   ;
            npc.rarity         = def.Rarity     ;
        }

        void RegisterBossHeadTexture(NpcDef npc, Texture2D tex)
        {
            NPCID.Sets.BossHeadTextures[npc.Type] = Main.npcHeadBossTexture.Length;
            int newLen = Main.npcHeadBossTexture.Length + 1;
            Array.Resize(ref Main.npcHeadBossTexture, newLen);
            Main.npcHeadBossTexture[newLen - 1] = tex;
        }
        void RegisterTownNpcHeadTexture(NpcDef npc, Texture2D tex)
        {
            npc.TownConfig.HeadId = Main.npcHeadTexture.Length;
            int newLen = Main.npcHeadTexture.Length + 1;
            Array.Resize(ref Main.npcHeadTexture, newLen);
            Main.npcHeadTexture[newLen - 1] = tex;
        }

        protected override List<LoaderError> CheckTextures(NpcDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of NpcDef " + def + " is null."));
            if ((def.IsBoss || def.IsTechnicallyABoss) && def.GetBossHeadTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetBossHeadTexture of NpcDef " + def + " is null."));
            if (def.IsTownNpc && def.TownConfig.GetHeadTexture == null)
                ret.Add(new LoaderError(def.Mod, "TownConfig.GetHeadTexture of NpcDef " + def + " is null."));

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
                    RegisterBossHeadTexture(def, bht);
            }

            if (def.IsTownNpc)
            {
                var tch = def.TownConfig.GetHeadTexture();
                if (tch == null)
                    ret.Add(new LoaderError(def.Mod, "TownConfig.GetHeadTexture return value is null for NpcDef " + def + "."));
                else
                    RegisterTownNpcHeadTexture(def, tch);
            }

            return ret;
        }

        protected override int GetRegularType(NPC npc)
        {
            return npc.type;
        }

        protected override void CopySetProperties(NpcDef def)
        {
            if (def.NetID < 0)
                Lang._negativeNpcNameCache[-def.NetID - 1] = def.DisplayName.ToLocalization();
            else
                Lang._npcNameCache[def.Type] = def.DisplayName.ToLocalization();

            Main.npcFrameCount               [def.Type] = def.FrameCount;
            Main.npcCatchable                [def.Type] = def.CaughtAsItem != null;

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

        // give priority to regular town npc internal names instead of the field name
        // their internal name is displayed in the housing UI, and without this fix,
        // it would be shown as eg. "GoblinTinkerer" instead of "Goblin Tinkerer"
        protected override string GetNameVanillaMethod(NPC npc)
        {
            return npc.GivenOrTypeName;
        }
        protected override string InternalName(NPC npc)
        {
            return npc.TypeName;
        }
    }
}
