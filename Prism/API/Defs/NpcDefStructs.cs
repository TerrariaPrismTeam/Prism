using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Prism.API.Defs
{
    /// <summary>
    /// Class which contains specific NPC properties only town NPCs use.
    /// </summary>
    public class TownNpcConfig
    {
        /// <summary>
        /// Gets or sets the amount of animation frames the town NPC's sprite contains for attacking.
        /// </summary>
        /// <remarks>NPCID.Sets.AttackFrameCount[Type]</remarks>
        public int AttackFrameCount
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the length of the town NPC's attack animation (in frames).
        /// </summary>
        /// <remarks>NPCID.Sets.AttackTime[Type]</remarks>
        public int AttackTime
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the general type of attack the town NPC uses to defend itself against dangers.
        /// </summary>
        /// <remarks>NPCID.Sets.AttackType[Type]</remarks>
        public TownNpcAttackType AttackType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the town NPC's danger detection radius.
        /// </summary>
        /// <remarks>NPCID.Sets.DangerDetectRange[Type]</remarks>
        public int DangerDetectRadius
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of "extra" animation frames the town NPC has.
        /// <para/>Used for the new animations added for town NPCs in 1.3, such as sitting.
        /// </summary>
        /// <remarks>NPCID.Sets.ExtraFramesCount[Type]</remarks>
        public int ExtraFramesCount
        {
            get;
            set;
        }
        //Fucking Red pl0x
        /// <summary>
        /// Gets or sets the average attack chance of the town NPC (1/2x chance (e.g. set this to 2.5 for 20% chance; 1/2(2.5) = 1/5 = 20%))
        /// </summary>
        /// <remarks>NPCID.Sets.AttackAverageChance[Type]</remarks>
        public int AverageAttackChance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the index of the icon other town NPCs use to refer to this NPC while conversing with each other.
        /// <para/>Note: currently hardcoded for vanilla NPCs only.
        /// </summary>
        /// <remarks>NPCID.Sets.FaceEmote[Type]</remarks>
        public ChatBubbleIconIndex ChatIcon
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets radius of safety around the town in tiles* (-1 = no safety provided)
        /// <para/>
        /// <para/>*Not actually entirely sure what unit it is.
        /// </summary>
        /// <remarks>NPCID.Sets.PrettySafe[Type]</remarks>
        public int SafetyRadius
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the NPC's magic aura (used for Clothier, Wizard, Truffle, and Dryad).
        /// </summary>
        /// <remarks>NPCID.Sets.MagicAuraColor[Type]</remarks>
        public Color MagicAuraColour
        {
            get;
            set;
        } = Color.White;

        public TownNpcConfig()
        {
            AttackFrameCount = -1;
            AttackTime = -1;
            AttackType = TownNpcAttackType.None;
            DangerDetectRadius = -1;
            ExtraFramesCount = -1;
            AverageAttackChance = -1;
            ChatIcon = ChatBubbleIconIndex.None;
            SafetyRadius = -1;
            MagicAuraColour = Color.White;
        }
    }

    public enum TownNpcAttackType
    {
        None   = -1,
        /// <summary>
        /// Used by: Demolitionist (Grenades), Merchant (Throwing Knives), Goblin Tinkerer (Spiky Balls), Mechanic (Wrenches), Nurse (Syringes), Angler (Frost Daggerfish), Skeleton Merchant (Bones), Party Girl (Happy Grenades), Santa Claus (Snowballs)
        /// </summary>
        Thrown = 0,
        /// <summary>
        /// Used by: Arms Dealer (Flintlock Pistol/Minishark), Guide (Wooden Bow), Witch Doctor (Blowgun), Steampunker (Clockwork Assault Rifle),
        /// Pirate (?), Cyborg (Rocket Launcher), Traveling Merchant (Flintlock Pistol/Pulse Bow), Painter (Paintball Gun)
        /// </summary>
        Ranged = 1,
        /// <summary>
        /// Used by: Clothier (magic skulls), Wizard (fireballs), Truffle (spores), Dryad (ward)
        /// </summary>
        Magic  = 2,
        /// <summary>
        /// Used by: Dye Trader (Exotic Scimitar), Tax Collector (Sassy Cane), Stylist (Stylish Scissors)
        /// </summary>
        Melee  = 3
    }

    //TODO: OVERRIDE THE HARDCODED VALUE RANGES AND USE MAX FIELD.
    /// <summary>
    /// A structure for NPC values. Used for coin drops and overall NPC worth detection (biome keys, etc).
    /// </summary>
    public struct NpcValue : IEquatable<NpcValue>
    {
        public readonly static NpcValue Zero = new NpcValue(CoinValue.Zero, CoinValue.Zero);

        public CoinValue Min, Max;

        public NpcValue(CoinValue value)
            : this(value, value)
        {

        }
        public NpcValue(CoinValue minValue, CoinValue maxValue)
        {
            Min = minValue;
            Max = maxValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is NpcValue)
                return Equals((NpcValue)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Min.GetHashCode() | Max.GetHashCode();
        }
        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }
        public string ToString(IFormatProvider provider)
        {
            return "[" + Min.ToString(provider) + " ~ " + Max.ToString(provider) + "]";
        }
        public bool Equals(NpcValue other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public static bool operator ==(NpcValue a, NpcValue b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(NpcValue a, NpcValue b)
        {
            return !a.Equals(b);
        }
    }

    /// <summary>
    /// Vanilla NPC AI Styles
    /// </summary>
    public enum NpcAiStyle
    {
        /// <summary>
            /// Doesn't move.
            /// <para/><b>Used By: </b>Bound Goblin, Bound Wizard, Bound Mechanic, Webbed Stylist, Sleeping Angler, Turkor The Ungrateful's Body (console-exclusive boss)
        /// </summary>
        None = 0,
        /// <summary>
            /// Hops in one direction, slides on slopes, floats in water, follows player if damaged or it's nighttime.
            /// <para/><b>Used By: </b>All Slimes (except King Slime), Hoppin' Jack, Grasshopper
        /// </summary>
        Slime = 1,
        /// <summary>
            /// Flies, follows player, bounces off walls in an arc.
            /// <para/><b>Used By: </b>Demon Eye, Wandering Eye, The Hungry II (part of Wall of Flesh), Pigron
        /// </summary>
        DemonEye = 2,
        /// <summary>
            /// Walks, jumps over holes, follows player. It will try to line up vertically first. If it fails to reach its target, it will back up a bit, then re-attempt.
            /// <para/><b>Used By: </b>Zombie, Skeleton, Undead Miner, Skeleton Archer, Angry Bones, Armored Skeleton, Goblin Scout, Goblin Archer, Corrupt Bunny, Crab, Werewolf, Clown, Chaos Elemental, Possessed Armor, Mummy, Spectral Elemental (console / mobile exclusive), Vampire, Vampire Miner, Frankenstein, Swamp Thing, Undead Viking, Corrupt Penguin, Face Monster, Snow Flinx, Nymph, Armored Viking, Lihzahrd, Icy Merman, Pirate Deckhand, Pirate Corsair, Pirate Deadeye, Pirate Crossbower, Pirate Captain, Cochineal Beetle, Cyan Beetle, Lac Beetle, Sea Snail, Ice Golem, Eyezor, Anomura Fungus, Mushi Ladybug, Rusty Armored Bones, Blue Armored Bones, Hell Armored Bones, Bone Lee, Paladin, Skeleton Sniper, Tactical Skeleton, Skeleton Commando, Scarecrow, Splinterling, Zombie Elf, Elf Archer, Gingerbread Man, Nutcracker, Yeti, Krampus
        /// </summary>
        Fighter = 3,
        /// <summary>
            /// Alternates between trying to stay above the player and summoning Servants of Cthulhu, and charging at the player occasionally. Spins when low health, and begins exclusively charging at the player. Always looks at player.
            /// <para/><b>Used By: </b>Eye of Cthulhu
        /// </summary>
        EyeOfCthulhu = 4,
        /// <summary>
            /// Flies, looks at player, follows player.
            /// <para/><b>Used By: </b>Servant of Cthulhu, Eater of Souls, Corruptor, Hornet, Probe, Crimera, Moss Hornet, Moth, Bee, Parrot, Dragon Hornet (console exclusive), Servant of Ocram (console exclusive), Meteor Head
        /// </summary>
        Flying = 5,
        /// <summary>
            /// Burrows in ground, passes through blocks, follows player.
            /// <para/><b>Used By: </b>Giant Worm, Devourer, Bone Serpent, Eater of Worlds, Digger, World Feeder, Wyvern, Arch Wyvern (console exclusive), Leech
        /// </summary>
        Worm = 6,
        /// <summary>
            /// Walks semi-randomly, jumps over holes, if used by town NPCs, they occasionally talk to one another.
            /// <para/><b>Used By: </b>All NPCs that move in to houses, Travelling Merchant, Old Man, Bunny, Penguin, Squirrel, Mouse, Frog, Scorpion, Black Scorpion
        /// </summary>
        Passive = 7,
        /// <summary>
            /// Casts spells at player, stays stationary, warps after three casts, warps if falling.
            /// <para/><b>Used By: </b>Fire Imp, Goblin Sorcerer, Dark Caster, Necromancer, Diabolist, Ragged Caster, Tim, Rune Wizard
        /// </summary>
        Caster = 8,
        /// <summary>
            /// Travels in a direct line toward player, going through blocks. Used by casters.
            /// <para/><b>Used By: </b>Burning Sphere, Chaos Ball, Vile Spit, Water Sphere
        /// </summary>
        Spell = 9,
        /// <summary>
            /// Tries to drift toward or around player, often stays a little bit out of reach once having touched the player.
            /// <para/><b>Used By: </b>Cursed Skull, Giant Cursed Skull, Dragon Skull (console/mobile exclusive)
        /// </summary>
        CursedSkull = 10,
        /// <summary>
            /// Tries to stay above player, spins and moves towards player occasionally.
            /// <para/><b>Used By: </b>Skeletron Head, Dungeon Guardian
        /// </summary>
        Head = 11,
        /// <summary>
            /// Waves, damages player.
            /// <para/><b>Used By: </b>Skeletron Hand
        /// </summary>
        SkeletronHand = 12,
        /// <summary>
            /// Extends on a vine towards player, looks at player. Dies if not rooted to a block.
            /// <para/><b>Used By: </b>Snatcher, Man Eater, Clinger, Dragon Snatcher, Angry Trapper, Fungi Bulb, Giant Fungi Bulb
        /// </summary>
        Plant = 13,
        /// <summary>
            /// Spasmodically flies toward player.
            /// <para/><b>Used By: </b>Harpy, Cave Bat, Jungle Bat, Ice Bat, Lava Bat, Giant Flying Fox, Hellbat, Demon, Voodoo Demon, Giant Bat, Illuminant Bat, Slimer, Arch Demon (console/mobile exclusive), Red Devil, Vampire, Flying Snake
        /// </summary>
        Bat = 14,
        /// <summary>
            /// Hops toward the player, releases Blue Slimes when damaged.
            /// <para/><b>Used By: </b>King Slime
        /// </summary>
        KingSlime = 15,
        /// <summary>
            /// Swims back and forth, moves towards the player if they are in water.
            /// <para/><b>Used By: </b>Corrupt Goldfish, Piranha, Arapaima, Shark, Blood Feeder, Angler Fish, Orca (console/mobile exlusive)
        /// </summary>
        Swimming = 16,
        /// <summary>
            /// Stands still until player gets within five blocks of AI. AI then acts similarly to the Flying AI.
            /// <para/><b>Used By: </b>Vulture, Raven
        /// </summary>
        Vulture = 17,
        /// <summary>
            /// Floats back and forth, swims toward player in small bursts if player is in water.
            /// <para/><b>Used By: </b>Blue Jellyfish, Pink Jellyfish, Green Jellyfish, Squid, Blood Jelly, Fungo Fish
        /// </summary>
        Jellyfish = 18,
        /// <summary>
            /// Looks at player, climbs overlapping blocks, shoots at nearby players.
            /// <para/><b>Used By: </b>Antlion, Albino Antlion (console/mobile exclusive)
        /// </summary>
        Antlion = 19,
        /// <summary>
            /// Swings in a circle from a pivot point on a chain.
            /// <para/><b>Used By: </b>Spike Ball
        /// </summary>
        SpikeBall = 20,
        /// <summary>
            /// Moves along the walls, floors and closed doors.
            /// <para/><b>Used By: </b>Blazing Wheel
        /// </summary>
        BlazingWheel = 21,
        /// <summary>
            /// Similar to the Fighter AI, floats over the ground instead of jumping
            /// <para/><b>Used By: </b>Wraith, Pixie, Gastropod, Spectral Gastropod, Ice Elemental, Floaty Gross, Reaper, Ichor Sticker, Ghost, Poltergeist
        /// </summary>
        Hovering = 22,
        /// <summary>
            /// Doesn't adhere to gravity or tile collisions. Spins several times, then heads straight towards the player. Any knockback cancels its attack and forces it to spin again.
            /// <para/><b>Used By: </b>Cursed Hammer, Enchanted Sword, Shadow Hammer (console/mobile exclusive), Crimson Axe
        /// </summary>
        FlyingWeapon = 23,
        /// <summary>
            /// Stands still until player gets nearby, then flies away. Avoids walls and obstacles and changes direction if one is in the way
            /// <para/><b>Used By: </b>Bird, Cardinal, Blue Jay
        /// </summary>
        Bird = 24,
        /// <summary>
            /// Stands still until player approaches or attacks, then leaps towards them with varying heights.
            /// <para/><b>Used By: </b>Mimic, Present Mimic
        /// </summary>
        Mimic = 25,
        /// <summary>
            /// Slowly gains speed while moving, jumps over obstacles.
            /// <para/><b>Used By: </b>Unicorn, Wolf, Hellhound, Headless Horseman
        /// </summary>
        Unicorn = 26,
        /// <summary>
            /// Traverses the world horizontally.
            /// <para/><b>Used By: </b>Wall of Flesh
            /// <para/><b>Note:    </b>Automatically spawns 2x Wall of Flesh Eye
        /// </summary>
        WallOfFleshBody = 27,
        /// <summary>
            /// Bound to an entity, watches player, and shoots projectiles. The more damaged it is, the more it shoots.
            /// <para/><b>Used By: </b>Wall of Flesh Eye
        /// </summary>
        WallOfFleshEye = 28,
        /// <summary>
            /// Similar to the Plant AI, but attached to an entity.
            /// <para/><b>Used By: </b>The Hungry (part of Wall of Flesh)
        /// </summary>
        TheHungry = 29,
        /// <summary>
            /// Alternates between attempting to stay diagonally above player while shooting projectiles slowly, and attempting to stay beside player and shooting projectiles very rapidly.
            /// <para/><b>Used By: </b>Retinazer
        /// </summary>
        Retinazer = 30,
        /// <summary>
            /// Alternates between shooting projectiles and staying beside player, and charging toward player.
            /// <para/><b>Used By: </b>Spazmatism
        /// </summary>
        Spazmatism = 31,
        /// <summary>
            /// Tries to stay above player, spins and moves towards player occasionally. Will start spinning indefinitely when enraged.
            /// <para/><b>Used By: </b>Skeletron Prime
        /// </summary>
        SkeletronPrimeHead = 32,
        /// <summary>
            /// Occasionally charges at the player, heads directly towards player when enraged.
            /// <para/><b>Used By: </b>Prime Saw
        /// </summary>
        PrimeSaw = 33,
        /// <summary>
            /// Occasionally charges at the player, rapidly charges when enraged.
            /// <para/><b>Used By: </b>Prime Vice
        /// </summary>
        PrimeVice = 34,
        /// <summary>
            /// Fires bombs upwards, aims directly at player when enraged.
            /// <para/><b>Used By: </b>Prime Cannon
        /// </summary>
        PrimeCannon = 35,
        /// <summary>
            /// Fires projectiles at player, shoots very rapidly when enraged.
            /// <para/><b>Used By: </b>Prime Laser
        /// </summary>
        PrimeLaser = 36,
        /// <summary>
            /// Similar to worm AI except shoot projectiles from the body and tail. It will also release Probes.
            /// <para/><b>Used By: </b>The Destroyer
        /// </summary>
        TheDestroyer = 37,
        /// <summary>
            /// Jumps and runs toward the player, similar to Fighter AI.
            /// <para/><b>Used By: </b>Snowman Gangsta, Mister Stabby, Snow Balla
        /// </summary>
        Snowman = 38,
        /// <summary>
            /// Crawls a bit, leaps toward player.
            /// <para/><b>Used By: </b>Giant Tortoise, Ice Tortoise, Giant Shelly
        /// </summary>
        Tortoise = 39,
        /// <summary>
            /// Capable of climbing background walls as well as through platforms. Similar to Fighter AI when there is no background wall.
            /// <para/><b>Used By: </b>Black Recluse, Wall Creeper, Jungle Creeper, Blood Crawler
        /// </summary>
        Spider = 40,
        /// <summary>
            /// Jumps high and attempts to land on the player. It can strafe to the sides in midair.
            /// <para/><b>Used By: </b>Herpling, Derpling
        /// </summary>
        Herpling = 41,
        /// <summary>
            /// Turns into a Nymph when a player gets too close.
            /// <para/><b>Used By: </b>Lost Girl
        /// </summary>
        LostGirl = 42,
        /// <summary>
            /// Alternates between attempting to stay above player while firing projectiles downards, and charging back and forth very rapidly.
            /// <para/><b>Used By: </b>Queen Bee
        /// </summary>
        QueenBee = 43,
        /// <summary>
            /// Flies straight towards player.
            /// <para/><b>Used By: </b>Flying Fish
        /// </summary>
        FlyingFish = 44,
        /// <summary>
            /// Jumps towards player every few seconds, shoots lasers.
            /// <para/><b>Used By: </b>Golem
            /// <para/><b>Note:    </b>Automatically spawns 1x Golem Head, 2x Golem Fist
        /// </summary>
        GolemBody = 45,
        /// <summary>
            /// Bound to an entity.
            /// <para/><b>Used By: </b>Golem Head
        /// </summary>
        GolemHead = 46,
        /// <summary>
            /// Flies towards player, returns when hit
            /// <para/><b>Used By: </b>Golem Fist
        /// </summary>
        GolemFist = 47,
        /// <summary>
            /// Attempts to fly back and forth, shoots projectiles at player,
            /// <para/><b>Used By: </b>Golem Head
        /// </summary>
        FlyingGolemHead = 48,
        /// <summary>
            /// Attempts to stay directly above the player, and fires projectiles downwards.
            /// <para/><b>Used By: </b>Angry Nimbus
        /// </summary>
        AngryNimbus = 49,
        /// <summary>
            /// Drifts downwards while following player, destroyed on contact.
            /// <para/><b>Used By: </b>Fungi Spore, Spore
        /// </summary>
        Spore = 50,
        /// <summary>
            /// Clings to nearby blocks, chases player, fires projectiles and spiky balls that bounce around.
            /// <para/><b>Used By: </b>Plantera
            /// <para/><b>Note:    </b>Automatically spawns 3x Plantera's Hook
        /// </summary>
        Plantera = 51,
        /// <summary>
            /// Moves forward briefly before latching onto a block.
            /// <para/><b>Used By: </b>Plantera's Hook
        /// </summary>
        PlanterasHook = 52,
        /// <summary>
            /// Acts very similar to the Plant AI, only bound to a certain entity.
            /// <para/><b>Used By: </b>Plantera's Tentacle
        /// </summary>
        PlanterasTentacle = 53,
        /// <summary>
            /// Doesn't adhere to gravity or tile collisions, and teleports occasionally in first form. Once all Creepers are killed, it will begin rapidly teleporting and toward the player.
            /// <para/><b>Used By: </b>Brain of Cthulhu
        /// </summary>
        BrainOfCthulhu = 54,
        /// <summary>
            /// Circles around an entity, charging at player.
            /// <para/><b>Used By: </b>Creeper
        /// </summary>
        Creeper = 55,
        /// <summary>
            /// Moves directly towards player, gaining momentum. Emits blue-ish particles.
            /// <para/><b>Used By: </b>Dungeon Spirit
        /// </summary>
        DungeonSpirit = 56,
        /// <summary>
            /// Moves towards player, stops, and fires projectiles straight at the player.
            /// <para/><b>Used By: </b>Mourning Wood, Everscream
        /// </summary>
        MouningWood = 57,
        /// <summary>
            /// Tries to stay above player.
            /// <para/><b>Used By: </b>Pumpking
            /// <para/><b>Note:    </b>Automatically spawns 2x Pumpking Scythe
        /// </summary>
        Pumpking = 58,
        /// <summary>
            /// Floats around and attempts to chop at the player.
            /// <para/><b>Used By: </b>Pumpking Scythe
        /// </summary>
        PumpkingScythe = 59,
        /// <summary>
            /// Flies around, shooting a barrage of ice-based projectiles.
            /// <para/><b>Used By: </b>Ice Queen
        /// </summary>
        IceQueen = 60,
        /// <summary>
            /// Moves across the ground, stops moving, and launches many different types of projectiles.
            /// <para/><b>Used By: </b>Santa-NK1
        /// </summary>
        SantaNK1 = 61,
        /// <summary>
            /// Attempts to fly around the player, shooting bullets rapidly if they are in sight.
            /// <para/><b>Used By: </b>Elf Copter
        /// </summary>
        ElfCopter = 62,
        /// <summary>
            /// Flies towards the player at high speed.
            /// <para/><b>Used By: </b>Flocko
        /// </summary>
        Flocko = 63,
        /// <summary>
            /// Flies slowly in any direction and occasionally glows.
            /// <para/><b>Used By: </b>Firefly, Lightning Bug
        /// </summary>
        Firefly = 64,
        /// <summary>
            /// Flies slowly in any direction.
            /// <para/><b>Used By: </b>Butterfly
        /// </summary>
        Butterfly = 65,
        /// <summary>
            /// Moves along the ground, pauses for a bit, then continues moving. Will avoid walls and moves in the other direction if it has hit one.
            /// <para/><b>Used By: </b>Worm, Truffle Worm
        /// </summary>
        PassiveWorm = 66,
        /// <summary>
            /// Moves along the ground, pauses for a bit, then continues moving. Climbs up walls if it contacts them.
            /// <para/><b>Used By: </b>Snail, Glowing Snail
        /// </summary>
        Snail = 67,
        /// <summary>
            /// Swims in water, or walks on land. Will fly when a player is nearby, landing after a while.
            /// <para/><b>Used By: </b>Duck, Mallard Duck
        /// </summary>
        Duck = 68,
        /// <summary>
            /// Rams player multiple times before summoning entities. In second form, it flies in circles and summons entities.
            /// <para/><b>Used By: </b>Duke Fishron
        /// </summary>
        DukeFishron = 69,
        /// <summary>
            /// Flies through the air, chases player, disappears after a period of time.
            /// <para/><b>Used By: </b>Detonating Bubble
        /// </summary>
        DetonatingBubble = 70,
        /// <summary>
            /// Follows an arcing path, dies when it touches a wall or player.
            /// <para/><b>Used By: </b>Sharkron
        /// </summary>
        Sharkron = 71,
        /// <summary>
            /// Surrounds Martian Officer with a bubble shield, preventing all damage until it is destroyed.
            /// <para/><b>Used By: </b>Martian Officer's Bubble Shield
        /// </summary>
        BubbleShield = 72,
        /// <summary>
            /// Stays stationary and shoots at the player.
            /// <para/><b>Used By: </b>Tesla Turret
        /// </summary>
        TeslaTurret = 73,
        /// <summary>
            /// Creeps up from a lower corner of the screen and then attacks, spinning.
            /// <para/><b>Used By: </b>Martian Drone, Corite
        /// </summary>
        CreepingSpinner = 74,
        /// <summary>
            /// Mounted upon another enemy and attacks. Has separate amount of health from the enemy it's mounted upon.
            /// <para/><b>Used By: </b>Martian Saucer (First phase), Saucer Turret, Saucer Cannon, Scutlix Gunner, Dutchman Cannon, Drakomire Rider
        /// </summary>
        MountedGunner = 75,
        /// <summary>
            /// Repeatedly charges back and forth above the player's head, attacking frantically.
            /// <para/><b>Used By: </b>Saucer Core
            /// <para/><b>Note:    </b>Automatically spawns 1x Martian Saucer, 2x Saucer Turret, 2x Saucer Cannon
        /// </summary>
        SaucerCore = 76,
        /// <summary>
            /// Flies very quickly toward the player and floats directly behind players as well as all tiles. Teleports right on top of the player if they get too far away.
            /// <para/><b>Used By: </b>Moon Lord Core
            /// <para/><b>Note:    </b>Automatically spawns 2x Moon Lord Hand, 1x Moon Lord Head
        /// </summary>
        MoonLordCore = 77,
        /// <summary>
            /// Hover's out to the side of Moon Lord Core, occasionally shooting ice attacks at the player.
            /// <para/><b>Used By: </b>Moon Lord Hand
        /// </summary>
        MoonLordHand = 78,
        /// <summary>
            /// Hovers just above Moon Lord Core, occasionally shooting a sweeping laser beam or spawning Moon Leech Clots at the player.
            /// <para/><b>Used By: </b>Moon Lord Head
        /// </summary>
        MoonLordHead = 79,
        /// <summary>
            /// Flies horizontally above the world surface looking for players. Flies away once found, and if allowed to escape, initiates the Martian Madness event.
            /// <para/><b>Used By: </b>Martian Probe
        /// </summary>
        MartianProbe = 80,
        /// <summary>
            /// Flies around the general vacinity of Moon Lord Core, occasionally sweeping diagonally into players, shooting lasers, or shooting ice projectiles.
            /// <para/><b>Used By: </b>True Eye of Cthulhu
        /// </summary>
        TrueEyeOfCthulhu = 81,
        /// <summary>
            /// Latches onto the player, inflicts the Moon Bite debuff, and heals Moon Lord.
            /// <para/><b>Used By: </b>Moon Leech Clot
        /// </summary>
        MoonLeechClot = 82,
        /// <summary>
            /// If used by Lunar Tablet, it floats in the air and spawns Cultists. If used by Cultists, they stands still, ignore all knockback, and perform a cultist ritual.
            /// <para/><b>Used By: </b>Lunar Tablet, Cultist
        /// </summary>
        Cultist = 83,
        /// <summary>
            /// If used by Lunatic Cultist, it teleports around using magic against the player. If used by Lunatic Cultist's clones, they follow him.
            /// <para/><b>Used By: </b>Lunatic Cultist, Lunatic Cultist's clones
        /// </summary>
        LunaticCultist = 84,
        /// <summary>
            /// Flies to the player and latches onto them, inflicting constant damage.
            /// <para/><b>Used By: </b>Star Cell, Nebula Headcrab (Brain Suckler), Deadly Sphere
        /// </summary>
        DeadlySphere = 85,

        /// <summary>
            /// Flies through tiles, deals damage to the player on contact, and ignores knockback.
            /// <para/><b>Used By: </b>Shadowflame Apparition (Goblin Summoner's minion), Ancient Vision (Lunatic Cultist's minion)
        /// </summary>
        FlyingHead = 86,
        /// <summary>
            /// Hops around on the ground, occasionally flying extremely quickly into the player.
            /// <para/><b>Used By: </b>Corrupt Mimic, Crimson Mimic, Hallowed Mimic, Jungle Mimic
        /// </summary>
        BigMimic = 87,
        /// <summary>
            /// Flies straight into the player and does contact damage while traveling through tiles.
            /// <para/><b>Used By: </b>Mothron
        /// </summary>
        Mothron = 88,
        /// <summary>
            /// Hatches into Baby Mothrons if not destroyed quickly.
            /// <para/><b>Used By: </b>Mothron Egg
        /// </summary>
        MothronEgg = 89,
        /// <summary>
            /// Flies through tiles and does contact damage to the player.
            /// <para/><b>Used By: </b>Mothron Egg
        /// </summary>
        BabyMothron = 90,
        /// <summary>
            /// Floats slowly toward the player, passing through tiles if the player is far enough. In expert mode, it will drop onto the ground once attacked, making it temporarily invulnerable.
            /// <para/><b>Used By: </b>Granite Elemental
        /// </summary>
        GraniteElemental = 91,
        /// <summary>
            /// Does not move, but can be attacked. It cannot be killed.
            /// <para/><b>Used By: </b>Target Dummy
        /// </summary>
        TargetDummy = 92,
        /// <summary>
            /// Floats above the player, occasionally spawning either a Pirate Deckhand, a Pirate Corair, a Pirate Deadeye, or a Pirate Crossbower.
            /// <para/><b>Used By: </b>Flying Dutchman
            /// <para/><b>Note:    </b>Automatically spawns 4x Dutchman Cannon
        /// </summary>
        FlyingDutchman = 93,
        /// <summary>
            /// Hovers up and down slowly. Has a shield that prevents all damage.
            /// If used by the of the 4 Celestial Towers, the shield can be destroyed after killing 100 (150 in expert mode) of its respective minions.
            /// <para/><b>Used By: </b>Vortex Pillar, Solar Pillar, Nebular Pillar, Stardust Pillar
        /// </summary>
        CelestialTower = 94,
        /// <summary>
            /// Floats around, dealing contact damage to player.
            /// <para/><b>Used By: </b>Miniature Star Cell
        /// </summary>
        MiniStarCell = 95,
        /// <summary>
            /// Flys around spawning projectiles that orbit around it until fired at the player.
            /// <para/><b>Used By: </b>Flow Invader
        /// </summary>
        FlowInvader = 96,
        /// <summary>
            /// Floats around, spawning minion projectiles that shoot lightning.
            /// <para/><b>Used By: </b>Nebula Floater
        /// </summary>
        NebulaFloater = 97,
    }

    public enum ChatBubbleIconIndex
    {
        None                          = -1,
        Icon_Heart                    = 0,
        Icon_Irritated                = 1,
        Icon_SweatDrop                = 2,
        Icon_Exclamation              = 3,
        Weather_Rain                  = 4,
        Weather_Lightning             = 5,
        Weather_Rainbow               = 6,
        Icon_GoldRing                 = 7,
        Icon_GreenSkull               = 8,
        Icon_SmokeySkull              = 9,
        Icon_Ellipsis                 = 10,
        Icon_Denied                   = 11,
        Critter_Bee                   = 12,
        Critter_Slime                 = 13,
        Tree                          = 14,
        Face_Grin                     = 15,
        Face_Frown                    = 16,
        Icon_MusicNote                = 17,
        Event_BloodMoon               = 18,
        Event_SolarEclipse            = 19,
        Event_PumpkinMoon             = 20,
        Event_FrostMoon               = 21,
        Biome_FloatingIsland          = 22,
        Biome_Forest                  = 23,
        Biome_Jungle                  = 24,
        Biome_Crimson                 = 25,
        Biome_Corruption              = 26,
        Biome_Hallow                  = 27,
        Biome_Desert                  = 28,
        Biome_Ocean                   = 29,
        Biome_Underground             = 30,
        Biome_Hell                    = 31,
        Biome_Snow                    = 32,
        RPS_Scissors_F                = 33,
        RPS_Rock_F                    = 34,
        RPS_Paper_F                   = 35,
        RPS_Scissors                  = 36,
        RPS_Rock                      = 37,
        RPS_Paper                     = 38,
        Boss_EyeOfCthulhu             = 39,
        Boss_EaterOfWorlds            = 40,
        Boss_BrainOfCthulhu           = 41,
        Boss_QueenBee                 = 42,
        Boss_Skeletron                = 43,
        Boss_WallOfFlesh              = 44,
        Boss_TheDestroyer             = 45,
        Boss_SkeletronPrime           = 46,
        Boss_Retinazor                = 47,
        Boss_Plantera                 = 48,
        Boss_Golem                    = 49,
        Boss_DukeFishron              = 50,
        Boss_SlimeKing                = 51,
        Boss_LunaticCultist           = 52,
        Boss_MoonLord                 = 53,
        PumpkinMoon_MourningWood      = 54,
        PumpkinMoon_Pumpking          = 55,
        FrostMoon_Everscream          = 56,
        FrostMoon_IceQueen            = 57,
        FrostMoon_SantaNK1            = 58,
        PirateInvasion_FlyingDutchman = 59,
        MartianMadness_Saucer         = 60,
        SolarEclipse_Eyez0r           = 61,
        Critter_Bunny                 = 62,
        Critter_Butterfly             = 63, // a chat bubble showing a butterfly
        Enemy_GoblinSummoner          = 64,
        PirateInvasion_PirateDeadeye  = 65,
        FrostLegion_SnowBalla         = 66,
        Enemy_Spider                  = 67,
        Critter_Bird                  = 68,
        Critter_Mouse                 = 69,
        Critter_Goldfish              = 70,
        Enemy_Martian                 = 71,
        Enemy_Skull                   = 72,
        Item_HealthPotion             = 73,
        Item_ManaPotion               = 74,
        Item_Soup                     = 75,
        Item_Fish                     = 76,
        Item_Drink                    = 77,
        Item_Sword                    = 78,
        Item_FishingRod               = 79,
        Item_BugNet                   = 80,
        Item_Dynamite                 = 81,
        Item_Minishark                = 82,
        Item_Gear                     = 83,
        Item_Gravestone               = 84,
        Item_Gold                     = 85,
        Item_DiamondRing              = 86,
        QuestionMark                  = 87,
        Lips                          = 88,
        Action_Sleep                  = 89,
        Item_Pickaxe                  = 90,
        Action_Run                    = 91,
        Action_Kick                   = 92,
        PvPSwordIcon                  = 93,
        Face_Cry                      = 94,
        Weather_Sunny                 = 95,
        Weather_StormCloud            = 96,
        Weather_Tornado               = 97,
        Weather_Saucer                = 98,
        Fireball                      = 99,
        Flame                         = 100,
        NPC_Merchant                  = 101,
        NPC_Nurse                     = 102,
        NPC_ArmsDealer                = 103,
        NPC_Dryad                     = 104,
        NPC_Guide                     = 105,
        NPC_OldMan                    = 106,
        NPC_Demolitionist             = 107,
        NPC_Clothier                  = 108,
        NPC_GoblinTinkerer            = 109,
        NPC_Wizard                    = 110,
        NPC_Mechanic                  = 111,
        NPC_SantaClaus                = 112,
        NPC_Truffle                   = 113,
        NPC_Steampunker               = 114,
        NPC_DyeTrader                 = 115,
        NPC_PartyGirl                 = 116,
        NPC_Cyborg                    = 117,
        NPC_Painter                   = 118,
        NPC_WitchDoctor               = 119,
        NPC_Pirate                    = 120,
        NPC_Stylist                   = 121,
        NPC_TravelingMerchant         = 122,
        NPC_Angler                    = 123,
        NPC_SkeletonMerchant          = 124,
        NPC_TaxCollector              = 125
    }
}
