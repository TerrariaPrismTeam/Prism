using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;

namespace Prism.API.Defs
{
    public class NpcDef : EntityDef<NpcBehaviour, NPC>
    {
        /// <summary>
        /// Gets NpcDefs by their type number.
        /// </summary>
        public struct ByTypeIndexer
        {
            public NpcDef this[int type]
            {
                get
                {
                    return Handler.NpcDef.DefsByType[type];
                }
            }
        }
        /// <summary>
        /// Gets NpcDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameIndexer
        {
            public NpcDef this[string npcInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == PrismApi.VanillaString || modInternalName == PrismApi.TerrariaString)
                        return Handler.NpcDef.VanillaDefsByName[npcInternalName];

                    return ModData.ModsFromInternalName[modInternalName].NpcDefs[npcInternalName];
                }
            }
        }

        /// <summary>
        /// Gets NpcDefs by their type number.
        /// </summary>
        public static ByTypeIndexer ByType
        {
            get
            {
                return new ByTypeIndexer();
            }
        }

        /// <summary>
        /// Gets NpcDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameIndexer ByName
        {
            get
            {
                return new ByNameIndexer();
            }
        }

        internal int BossHeadTextureIndex;

        /// <summary>
        /// Gets or sets the amount of damage this NPC inflicts.
        /// </summary>
        /// <remarks>NPC.damage</remarks>
        public virtual int Damage
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Gets or sets the width of this NPC.
        /// </summary>
        /// <remarks>NPC.width</remarks>
        public virtual int Width
        {
            get;
            set;
        } = 16;

        /// <summary>
        /// Gets or sets the height of this NPC.
        /// </summary>
        /// <remarks>NPC.height</remarks>
        public virtual int Height
        {
            get;
            set;
        } = 16;

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
        } = 0;

        /// <summary>
        /// Gets or sets the maximum life of this NPC.
        /// </summary>
        public virtual int MaxLife
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// Gets or sets the ID of the sound effect this NPC plays upon getting hurt.
        /// </summary>
        public virtual int SoundOnHit
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// Gets or sets the ID of the sound effect this NPC plays upon dying.
        /// </summary>
        public virtual int SoundOnDeath
        {
            get;
            set;
        } = 1;                               

        /// <summary>
        /// Gets or sets the default length of this NPC's trail cache. The cache can be accessed with <see cref="NPC.oldPos"/> (a <see cref="Vector2"/>[])
        /// <para/>Note: The trail cache is resized only when <see cref="NPC.RealSetDefaults(int, float)"/> 
        /// is called, so after that point the array can be resized if you wish.
        /// </summary>
        /// <remarks>NPCID.Sets.TrailCacheLength[Type]</remarks>
        public virtual int TrailCacheLength
        {
            get;
            set;
        } = 10;

        /// <summary>
        /// Gets or sets the number of standard animation frames in this NPC's sprite.
        /// </summary>
        /// <remarks>Main.npcFrameCount[Type]</remarks>
        public virtual int FrameCount
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// Gets or sets whether or not this NPC ignores tile collision.
        /// </summary>
        public virtual bool IgnoreTileCollision
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether or not this NPC ignores gravity.
        /// </summary>
        public virtual bool IgnoreGravity
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether the NPC is a boss or not. This is for traditional bosses.
        /// <para/>For other things which are technically bosses (have a map icon etc) but aren't really 
        /// traditional boss fights (e.g. Celestial Towers), <see cref="IsTechnicallyABoss"/> should be used in that case.
        /// </summary>
        public virtual bool IsBoss
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether the NPC is a town npc.
        /// </summary>
        public virtual bool IsTownNpc
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this is a summonable boss. The game uses this to check if the enemy/boss someone is spawning in multiplayer is legitimate or if they're just hacking, ignoring netmessages trying to summon anything which doesn't have this property set to true.
        /// <para/>Note: "Summonable" refers to bosses which are summoned with the message "Boss has awakened!".
        /// </summary>
        /// <remarks>NPCID.Sets.MPAllowedEnemies[Type]</remarks>
        public virtual bool IsSummonableBoss
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this NPC gets drawn when offscreen (Wall of Flesh).
        /// </summary>
        /// <remarks>NPCID.Sets.MustAlwaysDraw[Type]</remarks>
        public virtual bool MustAlwaysDraw
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether or not this NPC receives an extra stat boost in Expert mode.
        /// Only used by a few weaker NPCs to make them less lame in Expert Mode (and on Moon Lord).
        /// </summary>
        /// <remarks>NPCID.Sets.NeedsExpertScaling[Type]</remarks>
        public virtual bool NeedsExpertScaling
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this NPC is used as a projectile that can be damaged with a weapon until dissipating.
        /// <para/>Example: Moon Lord's ice attacks.
        /// </summary>
        /// <remarks>NPCID.Sets.ProjectileNPC[Type]</remarks>
        public virtual bool IsProjectileNPC
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this NPC is excluded from death tallies (used for Mini Star Cell, etc)
        /// </summary>
        /// <remarks>NPCID.Sets.ExcludedFromDeathTally[Type]</remarks>
        public virtual bool ExcludedFromDeathTally
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this NPC's presence in the world is saved into the world file. (Used for Celestial Towers).
        /// </summary>
        /// <remarks>NPCID.Sets.SavesAndLoads[Type]</remarks>
        public virtual bool SavesAndLoads
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this NPC is a skeleton. 
        /// Skeleton Merchant will be friendly toward any NPC in this list.
        /// </summary>
        /// <remarks>NPCID.Sets.Skeletons, List, Add type if skeleton?</remarks>
        public virtual bool IsSkeleton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this NPC technically counts as a boss (blocks summoning of some bosses while in the world, etc).
        /// </summary>
        /// <remarks>NPCID.Sets.TechnicallyABoss[Type]</remarks>
        public virtual bool IsTechnicallyABoss
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets whether this NPC is a friendly town critter.
        /// </summary>
        /// <remarks>NPCID.Sets.TownCritter[Type]</remarks>
        public virtual bool IsTownCritter
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets the scale at which the NPC's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>NPC.scale</remarks>
        public virtual float Scale
        {
            get;
            set;
        } = 1.0f;

        /// <summary>
        /// Gets or sets this NPC's resistance to knockback.
        /// </summary>
        public virtual float KnockbackResistance
        {
            get;
            set;
        } = 1.0f;

        /// <summary>
        /// Gets or sets the amount of NPC slots this NPC takes up, which go toward the active NPC count limit.
        /// </summary>
        public virtual float NpcSlots
        {
            get;
            set;
        } = 1.0f;

        /// <summary>
        /// Gets or sets the color to which the NPC's sprite is tinted (<see cref="Color.White"/> = no tinting applied).
        /// </summary>
        /// <remarks>NPC.color</remarks>
        public virtual Color Colour
        {
            get;
            set;
        } = Color.White;

        /// <summary>
        /// Gets or sets the value of this NPC (used for coin drops, biome keys, etc)
        /// </summary>
        public virtual NpcValue Value
        {
            get;
            set;
        } = NpcValue.Zero;

        /// <summary>
        /// Gets or sets the AI style of this NPC.
        /// </summary>
        public virtual NpcAiStyle AiStyle
        {
            get;
            set;
        } = NpcAiStyle.None;    

        //TODO: use BuffRef... later
        /// <summary>
        /// Gets or sets the list of buff IDs this NPC is immune to.
        /// </summary>
        public virtual List<int> BuffImmunities
        {
            get;
            set;
        } = Empty<int>.List;


        /// <summary>
        /// Gets or sets the NPC's town NPC config.
        /// </summary>
        public virtual TownNpcConfig TownConfig
        {
            get;
            set;
        } = default(TownNpcConfig);

        /// <summary>
        /// Gets or sets whether this NPC is excluded from radar counts ("x enemies nearby").
        /// </summary>
        public virtual bool HasAntiRadar
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets the NPC's texture function.
        /// </summary>
        public virtual Func<Texture2D> GetTexture
        {
            get;
            set;
        } = Empty<Texture2D>.Func;

        /// <summary>
        /// Gets or sets the NPC's boss head texture function.
        /// </summary>
        public virtual Func<Texture2D> GetBossHeadTexture
        {
            get;
            set;
        } = null;

        public NpcDef()
        {
            TownConfig = new TownNpcConfig();
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
