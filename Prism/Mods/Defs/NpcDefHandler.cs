using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    /// <summary>
    /// Handles all the <see cref="NpcDef"/>s.
    /// </summary>
    static class NpcDefHandler
    {
        const int VanillaBossAmount = 31;

        static int nextType = ItemID.Count;
        internal static Dictionary<int   , NpcDef> DefFromType        = new Dictionary<int   , NpcDef>();
        internal static Dictionary<string, NpcDef> VanillaDefFromName = new Dictionary<string, NpcDef>();

        /// <summary>
        /// Extends the vanilla arrays through which the game iterates for various type checks.
        /// </summary>
        /// <param name="amt">The amount by which to extend the arrays.</param>
        static void ExtendArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.npcFrameCount.Length + amt : NPCID.Count;
            int newBossLen = amt > 0 && Main.dedServ ? Main.npcHeadBossTexture.Length + amt : VanillaBossAmount;

            if (!Main.dedServ)
            {
                Array.Resize(ref Main.npcTexture        , newLen    );
                Array.Resize(ref Main.npcHeadBossTexture, newBossLen);
            }

            Array.Resize(ref Main.npcName     , newLen);
            Array.Resize(ref Main.NPCLoaded   , newLen);
            Array.Resize(ref Main.npcCatchable, newLen);

            Array.Resize(ref NPC.killCount, newLen); //Hardcoded 540 in NPC.ResetKillCount();

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
        }

        /// <summary>
        /// Resets the loaded NPCs.
        /// </summary>
        internal static void Reset()
        {
            nextType = ItemID.Count;
            DefFromType.Clear();
            ExtendArrays(0);
        }
        /// <summary>
        /// Loads the NPCs into the specified Dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="Dictionary{TKey, TValue}"/> to load the items into.</param>
        internal static IEnumerable<LoaderError> Load(Dictionary<string, NpcDef> dict)
        {
            var ret = new List<LoaderError>();

            ExtendArrays(dict.Count);

            foreach (var v in dict.Values)
            {
                if (v.GetTexture == null && !Main.dedServ)
                    ret.Add(new LoaderError(v.Mod, "GetTexture is null."));
                else if (v.GetBossHeadTexture == null && v.IsTechnicallyABoss && !Main.dedServ)
                    ret.Add(new LoaderError(v.Mod, "GetBossHeadTexture is null."));
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
        /// Adds all the original vanilla items.
        /// </summary>
        internal static void FillVanilla()
        {
            for (int i = -65; i < NPCID.Count; i++)
            {
                if (i == 0)
                    continue;

                NPC n = new NPC();
                n.RealSetDefaults(i);

                NpcDef def = new NpcDef(Lang.npcName(n.type, true));

                def.InternalName = n.name;
                def.Type = n.type;
                def.NetID = i;

                CopyNpcToDef(def, n);

                DefFromType.Add(i, def);
                VanillaDefFromName.Add(n.name, def);
            }
        }

        internal static void OnSetDefaults(NPC n, int type, float scaleOverride)
        {
            if (type >= NPCID.Count)
            {
                n.RealSetDefaults(0, scaleOverride);

                if (DefFromType.ContainsKey(type))
                {
                    n.type = n.netID = type;
                    n.width = n.height = 16;

                    CopyDefToNpc(n, DefFromType[type]);

                    if (scaleOverride > -1f)
                        n.scale = scaleOverride;

                    //TODO: add def-specific hooks here
                }
            }
            else
                n.RealSetDefaults(type, scaleOverride);

            //TODO: add global hooks here
        }

        static void LoadTextures     (NpcDef def)
        {
            // def.GetTexture and GetBossHeadTexture values should be checked when it's loaded
            var texture = def.GetTexture();
            if (texture == null)
                throw new ArgumentNullException("GetTexture return value is null for NpcDef " + def.InternalName + " from mod " + def.Mod + ".");
            Main.npcTexture[def.Type] = def.GetTexture();

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
        static void LoadSetProperties(NpcDef def)
        {
            // assuming space is allocated in the ExtendArrays call
            Main.npcName[def.Type] = def.DisplayName;
            Main.npcFrameCount[def.Type] = def.TotalFrameCount;

            NPCID.Sets.AttackAverageChance   [def.Type] = def.AttackAverageChance;
            NPCID.Sets.AttackFrameCount      [def.Type] = def.AttackFrameCount;
            NPCID.Sets.AttackTime            [def.Type] = def.AttackTime;
            NPCID.Sets.AttackType            [def.Type] = def.AttackType;
            NPCID.Sets.DangerDetectRange     [def.Type] = def.DangerDetectRange;
            NPCID.Sets.ExcludedFromDeathTally[def.Type] = def.ExcludedFromDeathTally;
            NPCID.Sets.ExtraFramesCount      [def.Type] = def.ExtraFramesCount;
            NPCID.Sets.FaceEmote             [def.Type] = def.FaceEmote;
            NPCID.Sets.MagicAuraColor        [def.Type] = def.MagicAuraColour;
            NPCID.Sets.MPAllowedEnemies      [def.Type] = def.IsAllowedInMP;
            NPCID.Sets.MustAlwaysDraw        [def.Type] = def.MustAlwaysDraw;
            NPCID.Sets.NeedsExpertScaling    [def.Type] = def.NeedsExpertScaling;
            NPCID.Sets.PrettySafe            [def.Type] = def.PrettySafe;
            NPCID.Sets.ProjectileNPC         [def.Type] = def.IsProjectileNPC;
            NPCID.Sets.SavesAndLoads         [def.Type] = def.SavesAndLoads;

            if (def.IsSkeleton)
                NPCID.Sets.Skeletons.Add(def.Type);

            NPCID.Sets.TechnicallyABoss[def.Type] = def.IsTechnicallyABoss;
            NPCID.Sets.TownCritter     [def.Type] = def.IsTownCritter;
            NPCID.Sets.TrailCacheLength[def.Type] = def.TrailCacheLength;

        }

        // set properties aren't copied, type/netid is preserved
        static void CopyDefToNpc(NPC    tar, NpcDef source)
        {
            tar.damage = source.Damage;
            tar.width = source.Width;
            tar.height = source.Height;
            tar.alpha = source.Alpha;
            tar.defense = source.Defense;
            tar.scale = source.Scale;
            tar.color = source.Colour;
                        //(float)(source.Value.Min.Value);
                        //TODO: Look at NpcValue xmldoc lmao
            tar.value = Main.rand.Next(source.Value.Min.Value, source.Value.Max.Value); // close enough
            tar.aiStyle = (int)source.AiStyle;
            tar.lifeMax = source.MaxLife;

            for (int i = 0; i < source.BuffImmunities.Count; i++)
                tar.buffImmune[i] = true;

            tar.soundHit = source.SoundOnHit;
            tar.soundKilled = source.SoundOnDeath;
            tar.knockBackResist = source.KnockbackResistance;
            tar.noTileCollide = source.IgnoreTileCollision;
            tar.noGravity = source.IgnoreGravity;
            tar.npcSlots = source.NpcSlots;

            tar.name = source.InternalName;
            Main.npcName[tar.type] = source.DisplayName;
        }
        static void CopyNpcToDef(NpcDef tar, NPC    source)
        {
            tar.Damage = source.damage;
            tar.Width = source.width;
            tar.Height = source.height;
            tar.Alpha = source.alpha;
            tar.Defense = source.defense;
            tar.Scale = source.scale;
            tar.Colour = source.color;
            tar.Value = new NpcValue(new CoinValue((int)(source.value * 0.8f)), new CoinValue((int)(source.value * 1.2f)));
            tar.AiStyle = (NpcAiStyle)source.aiStyle;
            tar.MaxLife = source.lifeMax;

            for (int i = 0; i < source.buffImmune.Length; i++)
                if (source.buffImmune[i])
                    tar.BuffImmunities.Add(i);

            tar.GetTexture = () => Main.npcTexture[source.type];
            tar.GetTexture = () => Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[source.type]];

            tar.InternalName = source.name;
            tar.DisplayName = Main.npcName[source.type];
        }
    }
}
