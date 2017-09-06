using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;

namespace Prism.API.Defs
{
    public partial class NpcDef : EntityDef<NpcBehaviour, NPC>
    {
        /// <summary>
        /// Gets or sets the amount of damage this NPC inflicts.
        /// </summary>
        /// <remarks>NPC.damage</remarks>
        public int Damage
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the defense of this NPC.
        /// </summary>
        /// <remarks>NPC.defense</remarks>
        public int Defense
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the opacity at which the NPC's sprite is rendered (0 = fully opaque, 255 = fully transparent).
        /// </summary>
        /// <remarks>NPC.alpha</remarks>
        public int Alpha
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum life of this NPC.
        /// </summary>
        public int MaxLife
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the rarity of the NPC. The NPC with the highest rarity is shown by the Lifeform Analyzer.
        /// </summary>
        /// <remarks>I don't know how this value is scaled.</remarks>
        public int Rarity
        {
            get;
            set;
        }
        public int TimeLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default length of this NPC's trail cache. The cache can be accessed with <see cref="NPC.oldPos" /> (a <see cref="Vector2" />[])
        /// <para />Note: The trail cache is resized only when <see cref="NPC.RealSetDefaults(int, float)" />
        /// is called, so after that point the array can be resized if you wish.
        /// </summary>
        /// <remarks>NPCID.Sets.TrailCacheLength[Type]</remarks>
        public int TrailCacheLength
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the number of standard animation frames in this NPC's sprite.
        /// </summary>
        /// <remarks>Main.npcFrameCount[Type]</remarks>
        public int FrameCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not this NPC ignores tile collision.
        /// </summary>
        public bool IgnoreTileCollision
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this NPC ignores gravity.
        /// </summary>
        public bool IgnoreGravity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the NPC is a boss or not. This is for traditional bosses.
        /// <para/>For other things which are technically bosses (have a map icon etc) but aren't really
        /// traditional boss fights (e.g. Celestial Towers), <see cref="IsTechnicallyABoss"/> should be used in that case.
        /// </summary>
        public bool IsBoss
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the NPC is a town npc.
        /// </summary>
        public bool IsTownNpc
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is excluded from radar counts ("x enemies nearby").
        /// </summary>
        public bool NotOnRadar
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the NPC can be harmed by the player.
        /// </summary>
        public bool IsFriendly
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the NPC should be drawn behind the tiles.
        /// </summary>
        public bool DrawBehindTiles
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is immortal. (eg. the Target Dummy is in fact an immortal NPC)
        /// </summary>
        public bool IsImmortal
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC can be chased after (by minions?).
        /// </summary>
        public bool IsChaseable
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is immune to anything.
        /// </summary>
        public bool IsImmune
        {
            get;
            set;
        }
        public bool AlwaysUpdateInMP
        {
            get;
            set;
        }
        public bool ImmuneToLava
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this is a summonable boss. The game uses this to check if the enemy/boss someone is spawning in multiplayer is legitimate or if they're just hacking, ignoring netmessages trying to summon anything which doesn't have this property set to true.
        /// <para/>Note: "Summonable" refers to bosses which are summoned with the message "&lt;boss&gt; has awoken!".
        /// </summary>
        /// <remarks>NPCID.Sets.MPAllowedEnemies[Type]</remarks>
        public bool IsSummonableBoss
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC gets drawn when offscreen (Wall of Flesh).
        /// </summary>
        /// <remarks>NPCID.Sets.MustAlwaysDraw[Type]</remarks>
        public bool MustAlwaysDraw
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether or not this NPC receives an extra stat boost in Expert mode.
        /// Only used by a few weaker NPCs to make them less lame in Expert Mode (and on Moon Lord).
        /// </summary>
        /// <remarks>NPCID.Sets.NeedsExpertScaling[Type]</remarks>
        public bool NeedsExpertScaling
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is used as a projectile that can be damaged with a weapon until dissipating.
        /// <para/>Example: Moon Lord's ice attacks.
        /// </summary>
        /// <remarks>NPCID.Sets.ProjectileNPC[Type]</remarks>
        public bool IsProjectileNPC
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is excluded from death tallies (used for Mini Star Cell, etc)
        /// </summary>
        /// <remarks>NPCID.Sets.ExcludedFromDeathTally[Type]</remarks>
        public bool ExcludedFromDeathTally
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC's presence in the world is saved into the world file. (Used for Celestial Towers).
        /// </summary>
        /// <remarks>NPCID.Sets.SavesAndLoads[Type]</remarks>
        public bool SavesAndLoads
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is a skeleton.
        /// Skeleton Merchant will be friendly toward any NPC in this list.
        /// </summary>
        /// <remarks>NPCID.Sets.Skeletons, List, Add type if skeleton?</remarks>
        public bool IsSkeleton
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC technically counts as a boss (blocks summoning of some bosses while in the world, etc).
        /// </summary>
        /// <remarks>NPCID.Sets.TechnicallyABoss[Type]</remarks>
        public bool IsTechnicallyABoss
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this NPC is a friendly town critter.
        /// </summary>
        /// <remarks>NPCID.Sets.TownCritter[Type]</remarks>
        public bool IsTownCritter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scale at which the NPC's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>NPC.scale</remarks>
        public float Scale
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this NPC's resistance to knockback (is a multiplier).
        /// </summary>
        public float KnockbackResistance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of NPC slots this NPC takes up, which go toward the active NPC count limit.
        /// </summary>
        public float NpcSlots
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to which the NPC's sprite is tinted (<see cref="Color.White"/> = no tinting applied).
        /// </summary>
        /// <remarks>NPC.color</remarks>
        public Color Colour
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the value of this NPC (used for coin drops, biome keys, etc)
        /// </summary>
        public NpcValue Value
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the AI style of this NPC.
        /// </summary>
        public NpcAiStyle AiStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the NPC's town NPC config.
        /// </summary>
        public TownNpcConfig TownConfig
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of buff IDs this NPC is immune to.
        /// </summary>
        public List<BuffRef> BuffImmunities
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the item that represents the caught NPC (when caught using a bug net. use 'null' to make the npc not catchable.)
        /// </summary>
        public ItemRef CaughtAsItem
        {
            get;
            set;
        }
        public BgmRef Music
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the sound effect this NPC plays upon getting hurt.
        /// </summary>
        public SfxRef SoundOnHit
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the sound effect this NPC plays upon dying.
        /// </summary>
        public SfxRef SoundOnDeath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the NPC's texture function.
        /// </summary>
        public Func<Texture2D> GetTexture
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the NPC's boss head texture function.
        /// </summary>
        public Func<Texture2D> GetBossHeadTexture
        {
            get;
            set;
        }

        public NpcDef(ObjectName displayName, Func<NpcBehaviour> newBehaviour = null, int lifeMax = 1, Func<Texture2D> getTexture = null, Func<Texture2D> getBossHeadTexture = null)
            : base(displayName, newBehaviour)
        {
            Width = Height = 16;
            MaxLife = lifeMax;

            SoundOnHit = SoundOnDeath = null;

            TrailCacheLength = 10;
            FrameCount = 1;

            Scale = KnockbackResistance = NpcSlots = 1f;

            Colour = Color.White;

            BuffImmunities = Empty<BuffRef>.List;

            TownConfig = new TownNpcConfig(null);

            GetTexture = getTexture ?? Empty<Texture2D>.Func;
            GetBossHeadTexture = getBossHeadTexture ?? Empty<Texture2D>.Func;

            TimeLeft = 3600;
        }

        public static implicit operator NpcRef(NpcDef  def)
        {
            return new NpcRef(def.InternalName, def.Mod.InternalName);
        }
        public static explicit operator NpcDef(NpcRef @ref /*Apparently we switched Prism over to Java, everyone*/)
        {
            return @ref.Resolve();
        }
    }
}
