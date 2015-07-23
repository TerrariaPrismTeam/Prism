using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Mods;
using Prism.Mods.Defs;

namespace Prism.API.Defs
{
    public class NpcDef : EntityDef
    {
        /// <summary>
        /// Gets ItemDefs by their type number.
        /// </summary>
        public struct ByTypeGetter
        {
            public ItemDef this[int type]
            {
                get
                {
                    return ItemDefHandler.DefFromType[type];
                }
            }
        }
        /// <summary>
        /// Gets ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameGetter
        {
            public NpcDef this[string itemInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == VanillaString || modInternalName == TerrariaString)
                        return NpcDefHandler.VanillaDefFromName[itemInternalName];

                    return ModData.ModsFromInternalName[modInternalName].NpcDefs[itemInternalName];
                }
            }
        }

        /// <summary>
        /// Gets ItemDefs by their type number.
        /// </summary>
        public static ByTypeGetter ByType
        {
            get
            {
                return new ByTypeGetter();
            }
        }
        /// <summary>
        /// Gets ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameGetter ByName
        {
            get
            {
                return new ByNameGetter();
            }
        }

        // stupid red and his stupid netids
        int setNetID = 0;
        /// <summary>
        /// Gets this item's NetID.
        /// </summary>
        public int NetID
        {
            get
            {
                return setNetID == 0 ? Type : setNetID;
            }
            internal set
            {
                setNetID = value;
            }
        }

        internal int BossHeadTextureIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the damage this NPC inflicts.
        /// </summary>
        /// <remarks>NPC.damage</remarks>
        public virtual int Damage
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width of this NPC.
        /// </summary>
        /// <remarks>NPC.width</remarks>
        public virtual int Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the height of this NPC.
        /// </summary>
        /// <remarks>NPC.height</remarks>
        public virtual int Height
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the defense of this NPC.
        /// </summary>
        /// <remarks>NPC.defense</remarks>
        public virtual int Defense
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the opacity at which the NPC's sprite is rendered (0 = fully opaque, 255 = fully transparent).
        /// </summary>
        /// <remarks>NPC.alpha</remarks>
        public virtual int Alpha
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum life of this NPC.
        /// </summary>
        public virtual int MaxLife
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the ID of the sound effect this NPC plays upon getting hurt.
        /// </summary>
        public virtual int SoundOnHit
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the ID of the sound effect this NPC plays upon dying.
        /// </summary>
        public virtual int SoundOnDeath
        {
            get;
            set;
        }

        //Fucking Red pl0x
        /// <summary>
        /// Gets or sets the average attack chance of this enemy (1/2x chance (e.g. set this to 2.5 for 20% chance; 1/2(2.5) = 1/5 = 20%))
        /// </summary>
        /// <remarks>NPCID.Sets.AttackAverageChance[Type]</remarks>
        public virtual int AttackAverageChance
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>NPCID.Sets.AttackFrameCount[Type]</remarks>
        public virtual int AttackFrameCount
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>NPCID.Sets.AttackTime[Type]</remarks>
        public virtual int AttackTime
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>NPCID.Sets.AttackType[Type]</remarks>
        public virtual int AttackType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this NPC's danger detection range (for town NPCs).
        /// </summary>
        /// <remarks>NPCID.Sets.DangerDetectRange[Type]</remarks>
        public virtual int DangerDetectRange
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of extra animation frames this enemy has.
        /// </summary>
        /// <remarks>NPCID.Sets.ExtraFramesCount[Type]</remarks>
        public virtual int ExtraFramesCount
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC chats with other NPCs with emotes.
        /// </summary>
        /// <remarks>NPCID.Sets.FaceEmote[Type]</remarks>
        public virtual int FaceEmote
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is "pretty safe", or poses little to no threat to the player.
        /// </summary>
        /// <remarks>NPCID.Sets.PrettySafe[Type]</remarks>
        public virtual int PrettySafe
        {
            get;
            set;
        }
        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>NPCID.Sets.TrailCacheLength[Type]</remarks>
        public virtual int TrailCacheLength
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the total number of animation frames in this NPC's sprite.
        /// </summary>
        /// <remarks>Main.npcFrameCount[Type]</remarks>
        public virtual int TotalFrameCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not this NPC ignores tile collision.
        /// </summary>
        public virtual bool IgnoreTileCollision
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this NPC ignores gravity.
        /// </summary>
        public virtual bool IgnoreGravity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the NPC is a boss or not.
        /// </summary>
        public virtual bool IsBoss
        {
            get;
            set;
        }

        /// <summary>
        /// NeedsSummary
        /// </summary>
        /// <remarks>NPCID.Sets.MPAllowedEnemies[Type]</remarks>
        public virtual bool IsAllowedInMP
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC gets drawn when offscreen (Wall of Flesh).
        /// </summary>
        /// <remarks>NPCID.Sets.MustAlwaysDraw[Type]</remarks>
        public virtual bool MustAlwaysDraw
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this NPC gets a damage boost in Expert mode.
        /// </summary>
        /// <remarks>NPCID.Sets.NeedsExpertScaling[Type]</remarks>
        public virtual bool NeedsExpertScaling
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is used as a projectile that can be destroyed using a weapon.
        /// </summary>
        /// <remarks>NPCID.Sets.ProjectileNPC[Type]</remarks>
        public virtual bool IsProjectileNPC
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is excluded from death tallies (for banners, etc).
        /// </summary>
        /// <remarks>NPCID.Sets.ExcludedFromDeathTally[Type]</remarks>
        public virtual bool ExcludedFromDeathTally
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPCs spawning is saved and loaded with the world file (Used for Celestial Towers).
        /// </summary>
        /// <remarks>NPCID.Sets.SavesAndLoads[Type]</remarks>
        public virtual bool SavesAndLoads
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is a skeleton.
        /// </summary>
        /// <remarks>NPCID.Sets.Skeletons, List, Add type if skeleton?</remarks>
        public virtual bool IsSkeleton
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC technically counts as a boss (either probably to display on the map or to play boss music, or something like that).
        /// </summary>
        /// <remarks>NPCID.Sets.TechnicallyABoss[Type]</remarks>
        public virtual bool IsTechnicallyABoss
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is a friendly town critter.
        /// </summary>
        /// <remarks>NPCID.Sets.TownCritter[Type]</remarks>
        public virtual bool IsTownCritter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scale at which the NPC's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>NPC.scale</remarks>
        public virtual float Scale
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this NPC's resistance to knockback.
        /// </summary>
        public virtual float KnockbackResistance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of NPC slots this NPC takes up, which go toward the active NPC count limit.
        /// </summary>
        public virtual float NpcSlots
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to which the NPC's sprite is tinted (<see cref="Color.White"/> = no tinting applied).
        /// </summary>
        /// <remarks>NPC.color</remarks>
        public virtual Color Colour
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the value of this NPC (used for coin drops, biome keys, etc)
        /// </summary>
        public virtual NpcValue Value
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the AI style of this NPC.
        /// </summary>
        public virtual NpcAiStyle AiStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of this NPC's magic aura (if it has one).
        /// </summary>
        /// <remarks>NPCID.Sets.MagicAuraColor[Type]</remarks>
        public virtual Color MagicAuraColour
        {
            get;
            set;
        }

        //TODO: use BuffRef... later
        /// <summary>
        /// Gets or sets the list of buff IDs this NPC is immune to.
        /// </summary>
        public virtual List<int> BuffImmunities
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the NPC's texture.
        /// </summary>
        public virtual Func<Texture2D> GetTexture
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the NPC's boss head texture function.
        /// </summary>
        public virtual Func<Texture2D> GetBossHeadTexture
        {
            get;
            set;
        }

        public NpcDef(
            #region arguments
            string displayName,

            int damage = 0,
            int width = 16,
            int height = 16,
            int defense = 0,
            int alpha = 0,
            int lifeMax = 1,
            int soundHit = 1,
            int soundKilled = 1,

            int attackAverageChance = 1,
            int attackFrameCount = 0,
            int attackTime = 0,
            int attackType = 0,
            int dangerDetectRange = 0,
            int extraFramesCount = 0,
            int faceEmote = 0,
            int prettySafe = 0,
            int trailCacheLength = 0,
            int frameCount = 1,

            bool noTileCollide = false,
            bool noGravity = false,
            bool boss = false,

            bool allowedInMP = true,
            bool alwaysDraw = false,
            bool needsExpertScaling = true,
            bool projectileNpc = false,
            bool excludedFromDeathTally = true,
            bool savesAndLoads = false,
            bool skeleton = false,
            bool? technicallyABoss = null,
            bool townCritter = false,

            float scale = 1f,
            float knockbackResist = 1f,
            float npcSlots = 1f,

            Color color = default(Color),
            NpcValue value = default(NpcValue),
            NpcAiStyle aiStyle = NpcAiStyle.None,

            Color magicAuraColour = default(Color),

            List<int> buffImmunities = null,

            Func<Texture2D> getTex         = null,
            Func<Texture2D> getBossHeadTex = null
            #endregion
            )
        {
            DisplayName = displayName;

            Damage = damage;
            Width = width;
            Height = height;
            Defense = defense;
            Alpha = alpha;
            MaxLife = lifeMax;
            SoundOnHit = soundHit;
            SoundOnDeath = soundKilled;

            AttackAverageChance = attackAverageChance;
            AttackFrameCount = attackFrameCount;
            AttackTime = attackTime;
            AttackType = attackType;
            DangerDetectRange = dangerDetectRange;
            ExtraFramesCount = extraFramesCount;
            FaceEmote = faceEmote;
            PrettySafe = prettySafe;
            TrailCacheLength = trailCacheLength;
            TotalFrameCount = frameCount;

            IgnoreTileCollision = noTileCollide;
            IgnoreGravity = noGravity;
            IsBoss = boss;

            IsAllowedInMP = allowedInMP;
            MustAlwaysDraw = alwaysDraw;
            NeedsExpertScaling = needsExpertScaling;
            IsProjectileNPC = projectileNpc;
            ExcludedFromDeathTally = excludedFromDeathTally;
            SavesAndLoads = savesAndLoads;
            IsSkeleton = skeleton;
            IsTechnicallyABoss = technicallyABoss ?? boss;

            Scale = scale;
            KnockbackResistance = knockbackResist;
            NpcSlots = npcSlots;

            Colour = color;
            Value = value;
            AiStyle = aiStyle;

            MagicAuraColour = magicAuraColour;

            BuffImmunities = buffImmunities ?? new List<int>(0);

            GetTexture         = getTex         ?? (() => null);
            GetBossHeadTexture = getBossHeadTex ?? (() => null);
        }
    }
}
