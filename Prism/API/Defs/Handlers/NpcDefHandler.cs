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
    public class NpcDefHandler : EntityDefHandler<NpcDef, NpcBehaviour, NPC>
    {
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

        public override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.npcFrameCount.Length + amt : NPCID.Count;
            int newBossLen = amt > 0 && Main.dedServ ? Main.npcHeadBossTexture.Length + amt : Prism.Vanilla.MiscValues.BossHeadCount;
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

        public override NPC GetVanillaEntityFromID(int id)
        {
            NPC entity = new NPC();
            entity.netDefaults(id);
            return entity;
        }

        public override void CopyEntityToDef(NPC entity, NpcDef def)
        {
            def.InternalName                            = entity.name;
            def.DisplayName                             = entity.displayName;
            def.Type                                    = entity.type;
            def.NetID                                   = entity.netID;
            def.Damage                                  = entity.damage;
            def.Width                                   = entity.width;
            def.Height                                  = entity.height;
            def.Defense                                 = entity.defense;
            def.Alpha                                   = entity.alpha;
            def.MaxLife                                 = entity.lifeMax;
            def.SoundOnHit                              = entity.soundHit;
            def.SoundOnDeath                            = entity.soundKilled;
            def.IgnoreTileCollision                     = entity.noTileCollide;
            def.IgnoreGravity                           = entity.noGravity;
            def.IsBoss                                  = entity.boss;
            def.IsTownNpc                               = entity.townNPC;
            def.Scale                                   = entity.scale;
            def.KnockbackResistance                     = entity.knockBackResist;
            def.NpcSlots                                = entity.npcSlots;
            def.Colour                                  = entity.color;                                                
            def.Value                                   = new NpcValue(new CoinValue((int)(entity.value * 0.8f)), 
                                                                       new CoinValue((int)(entity.value * 1.2f)));
            def.AiStyle                                 = (NpcAiStyle)entity.aiStyle;
            def.MaxLife                                 = entity.lifeMax;                       
            def.GetTexture                              = () => Main.npcTexture[entity.type];
            def.GetBossHeadTexture                      = () => Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[entity.type]];            
            def.BuffImmunities.Clear(); 
            for (int i = 0; i < entity.buffImmune.Length; i++)
            {
                if (entity.buffImmune[i])
                {
                    def.BuffImmunities.Add(i);
                }
            }

            def.DisplayName                             = Main.npcName                     [def.Type];
            def.TotalFrameCount                         = Main.npcFrameCount               [def.Type];
            def.AttackAverageChance                     = NPCID.Sets.AttackAverageChance   [def.Type];
            def.AttackFrameCount                        = NPCID.Sets.AttackFrameCount      [def.Type];
            def.AttackTime                              = NPCID.Sets.AttackTime            [def.Type];
            def.AttackType                              = NPCID.Sets.AttackType            [def.Type];
            def.DangerDetectRange                       = NPCID.Sets.DangerDetectRange     [def.Type];
            def.ExtraFramesCount                        = NPCID.Sets.ExtraFramesCount      [def.Type];
            def.FaceEmote                               = NPCID.Sets.FaceEmote             [def.Type];
            def.PrettySafe                              = NPCID.Sets.PrettySafe            [def.Type];
            def.TrailCacheLength                        = NPCID.Sets.TrailCacheLength      [def.Type];
            def.ExcludedFromDeathTally                  = NPCID.Sets.ExcludedFromDeathTally[def.Type];
            def.IsAllowedInMP                           = NPCID.Sets.MPAllowedEnemies      [def.Type];
            def.MustAlwaysDraw                          = NPCID.Sets.MustAlwaysDraw        [def.Type];
            def.NeedsExpertScaling                      = NPCID.Sets.NeedsExpertScaling    [def.Type];
            def.IsProjectileNPC                         = NPCID.Sets.ProjectileNPC         [def.Type];
            def.SavesAndLoads                           = NPCID.Sets.SavesAndLoads         [def.Type];
            def.IsTechnicallyABoss                      = NPCID.Sets.TechnicallyABoss      [def.Type];
            def.IsTownCritter                           = NPCID.Sets.TownCritter           [def.Type];
            def.MagicAuraColour                         = NPCID.Sets.MagicAuraColor        [def.Type];
        }

        public override void CopyDefToEntity(NpcDef def, NPC entity)
        {
            entity.name                                 = def.InternalName;
            entity.displayName                          = def.DisplayName;
            entity.type                                 = def.Type;
            entity.netID                                = def.NetID;
            entity.damage                               = def.Damage;
            entity.width                                = def.Width;
            entity.height                               = def.Height;
            entity.defense                              = def.Defense;
            entity.alpha                                = def.Alpha;
            entity.lifeMax                              = def.MaxLife;
            entity.soundHit                             = def.SoundOnHit;
            entity.soundKilled                          = def.SoundOnDeath;
            entity.noTileCollide                        = def.IgnoreTileCollision;
            entity.noGravity                            = def.IgnoreGravity;
            entity.boss                                 = def.IsBoss;
            entity.townNPC                              = def.IsTownNpc;
            entity.scale                                = def.Scale;
            entity.knockBackResist                      = def.KnockbackResistance;
            entity.npcSlots                             = def.NpcSlots;
            entity.color                                = def.Colour;
            entity.dontCountMe                          = def.HasAntiRadar;
            entity.value                                = Main.rand.Next(def.Value.Min.Value, def.Value.Max.Value); // close enough
            entity.aiStyle                              = (int)def.AiStyle;                        
            for (int i = 0; i < def.BuffImmunities.Count; i++)
                entity.buffImmune[i]                    = true;

            Main.npcName                     [def.Type] = def.DisplayName;
            Main.npcFrameCount               [def.Type] = def.TotalFrameCount;
            NPCID.Sets.AttackAverageChance   [def.Type] = def.AttackAverageChance;
            NPCID.Sets.AttackFrameCount      [def.Type] = def.AttackFrameCount;
            NPCID.Sets.AttackTime            [def.Type] = def.AttackTime;
            NPCID.Sets.AttackType            [def.Type] = def.AttackType;
            NPCID.Sets.DangerDetectRange     [def.Type] = def.DangerDetectRange;
            NPCID.Sets.ExtraFramesCount      [def.Type] = def.ExtraFramesCount;
            NPCID.Sets.FaceEmote             [def.Type] = def.FaceEmote;
            NPCID.Sets.PrettySafe            [def.Type] = def.PrettySafe;
            NPCID.Sets.TrailCacheLength      [def.Type] = def.TrailCacheLength;
            NPCID.Sets.ExcludedFromDeathTally[def.Type] = def.ExcludedFromDeathTally;
            NPCID.Sets.MPAllowedEnemies      [def.Type] = def.IsAllowedInMP;
            NPCID.Sets.MustAlwaysDraw        [def.Type] = def.MustAlwaysDraw;
            NPCID.Sets.NeedsExpertScaling    [def.Type] = def.NeedsExpertScaling;
            NPCID.Sets.ProjectileNPC         [def.Type] = def.IsProjectileNPC;
            NPCID.Sets.SavesAndLoads         [def.Type] = def.SavesAndLoads;
            NPCID.Sets.TechnicallyABoss      [def.Type] = def.IsTechnicallyABoss;
            NPCID.Sets.TownCritter           [def.Type] = def.IsTownCritter;
            NPCID.Sets.MagicAuraColor        [def.Type] = def.MagicAuraColour;
            if (def.IsSkeleton && !NPCID.Sets.Skeletons.Contains(def.Type))
            {
                NPCID.Sets.Skeletons.Add(def.Type);
            }
        }        

        public override bool CheckTextures(NpcDef def)
        {
            return !(def.GetTexture == null) && !(def.GetBossHeadTexture == null);
        }

        public override void LoadTextures(ref List<LoaderError> err, NpcDef def)
        {
            // def.GetTexture and GetBossHeadTexture values should be checked when it's loaded
            var texture = def.GetTexture();
            if (texture == null)
                throw new ArgumentNullException("GetTexture return value is null for NpcDef " + def.InternalName + " from mod " + def.Mod + ".");
            Main.npcTexture[def.Type] = def.GetTexture();
            Main.NPCLoaded[def.Type] = true;

            if (def.IsTechnicallyABoss)
            {
                var bossHeadTex = def.GetBossHeadTexture();
                if (bossHeadTex != null)
                {
                    // this shouldn't be -1, because new entries were allocated in ExtendArrays
                    int firstNull = Array.IndexOf(Main.npcHeadBossTexture, null);
                    Main.npcHeadBossTexture[firstNull] = def.GetBossHeadTexture();

                    NPCID.Sets.BossHeadTextures[def.Type] = firstNull;
                    def.BossHeadTextureIndex = firstNull;
                }
            }

            // Might have to do something else here or something idk meh
            //TODO: npc head textures, head-in-chat-bubble textures(?)
        }
    }
}