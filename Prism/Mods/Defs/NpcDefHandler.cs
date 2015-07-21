using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    /// <summary>
    /// Handles all the <see cref="NpcDef"/>s.
    /// </summary>
    static class NpcDefHandler
    {
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

            if (!Main.dedServ)
                Array.Resize(ref Main.npcTexture, newLen);

            Array.Resize(ref Main.npcName, newLen);
            Array.Resize(ref Main.NPCLoaded, newLen);
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
                if (v.GetTexture == null)
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
        /// Adds all the original vanilla items.
        /// </summary>
        internal static void FillVanilla()
        {
            for (int i = -65; i < NPCID.Count; i++)
            {                
                if (i == 0)
                    continue;

                NPC n = new NPC();
                //n.RealSetDefaults(i, true);

                NpcDef def = new NpcDef(Lang.npcName(n.type, true));

                def.InternalName = n.name;
                def.Type = n.type;
                def.NetID = i;

                CopyNpcToDef(def, n);

                DefFromType.Add(i, def);
                VanillaDefFromName.Add(n.name, def);
            }
        }

        internal static void OnSetDefaults(NPC n, int type, bool noMatCheck)
        {
            if (type >= NPCID.Count)
            {
                //n.RealSetDefaults(0, noMatCheck);

                if (DefFromType.ContainsKey(type))
                {
                    n.type = n.netID = type;
                    n.width = n.height = 16;

                    CopyDefToNpc(n, DefFromType[type]);
                }
            }
            else { }
                //n.RealSetDefaults(type, noMatCheck);
        }

        static void LoadTextures(NpcDef def)
        {
            // def.GetTexture itself should be checked when it's loaded
            var texture = def.GetTexture();
            if (texture == null)
                throw new ArgumentNullException("GetTexture return value is null for NpcDef " + def.InternalName + " from mod " + def.Mod + ".");
            Main.npcTexture[def.Type] = def.GetTexture();

            var bossHeadTex = def.GetBossHeadTexture();
            if (bossHeadTex != null)
            {
                Array.Resize(ref Main.npcHeadBossTexture, Main.npcHeadBossTexture.Length + 1);
                Main.npcHeadBossTexture[Main.npcHeadBossTexture.Length - 1] = def.GetBossHeadTexture();
                def.BossHeadTextureIndex = Main.npcHeadBossTexture.Length - 1;
            }

            //Might have to do something else here or something idk meh
        }
        static void LoadSetProperties(NpcDef def)
        {
            // assuming space is allocated in the ExtendArrays call
            Main.npcName[def.Type] = def.DisplayName;
            Main.npcFrameCount[def.Type] = def.TotalFrameCount;

            NPCID.Sets.AttackAverageChance[def.Type] = def.AttackAverageChance;
            NPCID.Sets.AttackFrameCount[def.Type] = def.AttackFrameCount;
            NPCID.Sets.AttackTime[def.Type] = def.AttackTime;
            NPCID.Sets.AttackType[def.Type] = def.AttackType;
            NPCID.Sets.BossHeadTextures[def.Type] = def.BossHeadTexture;
            NPCID.Sets.DangerDetectRange[def.Type] = def.DangerDetectRange;
            NPCID.Sets.ExcludedFromDeathTally[def.Type] = def.ExcludedFromDeathTally;
            NPCID.Sets.ExtraFramesCount[def.Type] = def.ExtraFramesCount;
            NPCID.Sets.FaceEmote[def.Type] = def.FaceEmote;
            NPCID.Sets.MagicAuraColor[def.Type] = def.MagicAuraColor;
            NPCID.Sets.MPAllowedEnemies[def.Type] = def.IsAllowedInMP;
            NPCID.Sets.MustAlwaysDraw[def.Type] = def.MustAlwaysDraw;
            NPCID.Sets.NeedsExpertScaling[def.Type] = def.NeedsExpertScaling;
            NPCID.Sets.PrettySafe[def.Type] = def.PrettySafe;
            NPCID.Sets.ProjectileNPC[def.Type] = def.IsProjectileNPC;
            NPCID.Sets.SavesAndLoads[def.Type] = def.SavesAndLoads;

            if (def.IsSkeleton) NPCID.Sets.Skeletons.Add(def.Type);

            NPCID.Sets.TechnicallyABoss[def.Type] = def.IsTechnicallyABoss;
            NPCID.Sets.TownCritter[def.Type] = def.IsTownCritter;
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
            tar.value = (float)(source.Value.Min.Value);   //TO-DO: Look at NpcValue xmldoc lmao
            tar.aiStyle = (int)source.AiStyle;
            tar.lifeMax = source.MaxLife;

            for (int i = 0; i < source.BuffImmunityIDs.Count; i++)
            {
                tar.buffImmune[i] = true;
            }

            tar.soundHit;
            tar.soundKilled;
            tar.knockBackResist;
            tar.noTileCollide;
            tar.noGravity;
            tar.npcSlots;

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
            tar.Value = new NpcValue((CoinValue)(int)source.value, (CoinValue)(int)source.value /* + some number for vanilla or something */);
            tar.AiStyle = (NpcAiStyle)source.aiStyle;
            tar.MaxLife = source.lifeMax;

            for (int i = 0; i < source.buffImmune.Length; i++)
            {
                if (source.buffImmune[i]) tar.BuffImmunityIDs.Add(i);
            }

            tar.GetTexture = () => Main.npcTexture[source.type];
            tar.GetTexture = () => Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[source.type]];

            tar.InternalName = source.name;
            tar.DisplayName = Main.npcName[source.type];
        }
    }
}
