using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prism.API.Defs
{
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

        public string ToString(IFormatProvider provider)
        {
            return "[" + Min.ToString(provider) + " ~ " + Max.ToString(provider) + "]";
        }
    }

    /// <summary>
    /// Vanilla NPC AI Styles
    /// </summary>
    public enum NpcAiStyle
    {
        //Ints are included to make it easier to find out which number a certain one is if simply scrolling through these or something.

        /// <summary>
        /// Doesn't move.
        /// <para>
        /// Used by: Bound Goblin, Bound Wizard, Bound Mechanic, Webbed Stylist, Sleeping Angler, Turkor The Ungrateful's Body (console-exclusive boss)
        /// </para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Hops in one direction, slides on slopes, floats in water, follows player if damaged or it's nighttime.
        /// <para>
        /// Used by: All Slimes (except King Slime), Hoppin' Jack, Grasshopper
        /// </para>
        /// </summary>
        Slime = 1,

        /// <summary>
        /// Flies, follows player, bounces off walls in an arc.
        /// <para>
        /// Used by: Demon Eye, Wandering Eye, The Hungry II (part of Wall of Flesh), Pigron
        /// </para>
        /// </summary>
        DemonEye = 2,

        /// <summary>
        /// Walks, jumps over holes, follows player. It will try to line up vertically first. If it fails to reach its target, it will back up a bit, then re-attempt.
        /// <para>
        /// Used by: Zombie, Skeleton, Undead Miner, Skeleton Archer, Angry Bones, Armored Skeleton, Goblin Scout, Goblin Archer, Corrupt Bunny, Crab, Werewolf, Clown, Chaos Elemental,
        /// Possessed Armor, Mummy, Spectral Elemental (console / mobile exclusive), Vampire, Vampire Miner, Frankenstein, Swamp Thing, Undead Viking, Corrupt Penguin, Face Monster,
        /// Snow Flinx, Nymph, Armored Viking, Lihzahrd, Icy Merman, Pirate Deckhand, Pirate Corsair, Pirate Deadeye, Pirate Crossbower, Pirate Captain, Cochineal Beetle, Cyan Beetle,
        /// Lac Beetle, Sea Snail, Ice Golem, Eyezor, Anomura Fungus, Mushi Ladybug, Rusty Armored Bones, Blue Armored Bones, Hell Armored Bones, Bone Lee, Paladin, Skeleton Sniper,
        /// Tactical Skeleton, Skeleton Commando, Scarecrow, Splinterling, Zombie Elf, Elf Archer, Gingerbread Man, Nutcracker, Yeti, Krampus
        /// </para>
        /// </summary>
        Fighter = 3,

        /// <summary>
        /// Alternates between trying to stay above the player and summoning Servants of Cthulhu, and charging at the player occasionally. Spins when low health, and begins exclusively charging at the player. Always looks at player.
        /// <para>
        /// Used by: Eye of Cthulhu
        /// </para>
        /// </summary>
        EyeOfCthulhu = 4,

        /// <summary>
        /// Flies, looks at player, follows player.
        /// <para>
        /// Used by: Servant of Cthulhu, Eater of Souls, Corruptor, Hornet, Probe, Crimera, Moss Hornet, Moth, Bee, Parrot, Dragon Hornet (console exclusive), Servant of Ocram (console exclusive), Meteor Head
        /// </para>
        /// </summary>
        Flying = 5,

        /// <summary>
        /// Burrows in ground, passes through blocks, follows player.
        /// <para>
        /// Used by: Giant Worm, Devourer, Bone Serpent, Eater of Worlds, Digger, World Feeder, Wyvern, Arch Wyvern (console exclusive), Leech
        /// </para>
        /// </summary>
        Worm = 6,

        /// <summary>
        /// Walks semi-randomly, jumps over holes, if used by town NPCs, they occasionally talk to one another.
        /// <para>
        /// Used by: All NPCs that move in to houses, Travelling Merchant, Old Man, Bunny, Penguin, Squirrel, Mouse, Frog, Scorpion, Black Scorpion
        /// </para>
        /// </summary>
        Passive = 7,

        /// <summary>
        /// Casts spells at player, stays stationary, warps after three casts, warps if falling.
        /// <para>
        /// Used by: Fire Imp, Goblin Sorcerer, Dark Caster, Necromancer, Diabolist, Ragged Caster, Tim, Rune Wizard
        /// </para>
        /// </summary>
        Caster = 8,

        /// <summary>
        /// Travels in a direct line toward player, going through blocks. Used by casters.
        /// <para>
        /// Used by: Burning Sphere, Chaos Ball, Vile Spit, Water Sphere
        /// </para>
        /// </summary>
        Spell = 9,

        /// <summary>
        /// Tries to drift toward or around player, often stays a little bit out of reach once having touched the player.
        /// <para>
        /// Used by: Cursed Skull, Giant Cursed Skull, Dragon Skull (console/mobile exclusive)
        /// </para>
        /// </summary>
        CursedSkull = 10,

        /// <summary>
        /// Tries to stay above player, spins and moves towards player occasionally.
        /// <para>
        /// Used by: Skeletron Head, Dungeon Guardian
        /// </para>
        /// </summary>
        Head = 11,

        /// <summary>
        /// Waves, damages player.
        /// <para>
        /// Used by: Skeletron Hand
        /// </para>
        /// </summary>
        SkeletronHand = 12,

        /// <summary>
        /// Extends on a vine towards player, looks at player. Dies if not rooted to a block.
        /// <para>
        /// Used by: Snatcher, Man Eater, Clinger, Dragon Snatcher, Angry Trapper, Fungi Bulb, Giant Fungi Bulb
        /// </para>
        /// </summary>
        Plant = 13,

        /// <summary>
        /// Spasmodically flies toward player.
        /// <para>
        /// Used by: Harpy, Cave Bat, Jungle Bat, Ice Bat, Lava Bat, Giant Flying Fox, Hellbat, Demon, Voodoo Demon, Giant Bat, Illuminant Bat, Slimer, Arch Demon (console/mobile exclusive), Red Devil, Vampire, Flying Snake
        /// </para>
        /// </summary>
        Bat = 14,

        /// <summary>
        /// Hops toward the player, releases Blue Slimes when damaged.
        /// <para>
        /// Used by: King Slime
        /// </para>
        /// </summary>
        KingSlime = 15,

        /// <summary>
        /// Swims back and forth, moves towards the player if they are in water.
        /// <para>
        /// Used by: Corrupt Goldfish, Piranha, Arapaima, Shark, Blood Feeder, Angler Fish, Orca (console/mobile exlusive)
        /// </para>
        /// </summary>
        Swimming = 16,

        /// <summary>
        /// Stands still until player gets within five blocks of AI. AI then acts similarly to the Flying AI.
        /// <para>
        /// Used by: Vulture, Raven
        /// </para>
        /// </summary>
        Vulture = 17,

        /// <summary>
        /// Floats back and forth, swims toward player in small bursts if player is in water.
        /// <para>
        /// Used by: Blue Jellyfish, Pink Jellyfish, Green Jellyfish, Squid, Blood Jelly, Fungo Fish
        /// </para>
        /// </summary>
        Jellyfish = 18,

        /// <summary>
        /// Looks at player, climbs overlapping blocks, shoots at nearby players.
        /// <para>
        /// Used by: Antlion, Albino Antlion (console/mobile exclusive)
        /// </para>
        /// </summary>
        Antlion = 19,

        /// <summary>
        /// Swings in a circle from a pivot point on a chain.
        /// <para>
        /// Used by: Spike Ball
        /// </para>
        /// </summary>
        SpikeBall = 20,

        /// <summary>
        /// Moves along the walls, floors and closed doors.
        /// <para>
        /// Used by: Blazing Wheel
        /// </para>
        /// </summary>
        BlazingWheel = 21,

        /// <summary>
        /// Similar to the Fighter AI, floats over the ground instead of jumping
        /// <para>
        /// Used by: Wraith, Pixie, Gastropod, Spectral Gastropod, Ice Elemental, Floaty Gross, Reaper, Ichor Sticker, Ghost, Poltergeist
        /// </para>
        /// </summary>
        Hovering = 22,

        /// <summary>
        /// Doesn't adhere to gravity or tile collisions. Spins several times, then heads straight towards the player. Any knockback cancels its attack and forces it to spin again.
        /// <para>
        /// Used by: Cursed Hammer, Enchanted Sword, Shadow Hammer (console/mobile exclusive), Crimson Axe
        /// </para>
        /// </summary>
        FlyingWeapon = 23,

        /// <summary>
        /// Stands still until player gets nearby, then flies away. Avoids walls and obstacles and changes direction if one is in the way
        /// <para>
        /// Used by: Bird, Cardinal, Blue Jay
        /// </para>
        /// </summary>
        Bird = 24,

        /// <summary>
        /// Stands still until player approaches or attacks, then leaps towards them with varying heights.
        /// <para>
        /// Used by: Mimic, Present Mimic
        /// </para>
        /// </summary>
        Mimic = 25,

        /// <summary>
        /// Slowly gains speed while moving, jumps over obstacles.
        /// <para>
        /// Used by: Unicorn, Wolf, Hellhound, Headless Horseman
        /// </para>
        /// </summary>
        Unicorn = 26,

        /// <summary>
        /// Traverses the world horizontally.
        /// <para>
        /// Automatically Summons: 2x Wall of Flesh Eye
        /// </para>
        /// <para>
        /// Used by: Wall of Flesh
        /// </para>
        /// </summary>
        WallOfFleshBody = 27,

        /// <summary>
        /// Bound to an entity, watches player, and shoots projectiles. The more damaged it is, the more it shoots.
        /// <para>
        /// Used by: Wall of Flesh Eye
        /// </para>
        /// </summary>
        WallOfFleshEye = 28,

        /// <summary>
        /// Similar to the Plant AI, but attached to an entity.
        /// <para>
        /// Used by: The Hungry (part of Wall of Flesh)
        /// </para>
        /// </summary>
        TheHungry = 29,

        /// <summary>
        /// Alternates between attempting to stay diagonally above player while shooting projectiles slowly, and attempting to stay beside player and shooting projectiles very rapidly.
        /// <para>
        /// Used by: Retinazer
        /// </para>
        /// </summary>
        Retinazer = 30,

        /// <summary>
        /// Alternates between shooting projectiles and staying beside player, and charging toward player.
        /// <para>
        /// Used by: Spazmatism
        /// </para>
        /// </summary>
        Spazmatism = 31,

        /// <summary>
        /// Tries to stay above player, spins and moves towards player occasionally. Will start spinning indefinitely when enraged.
        /// <para>
        /// Used by: Skeletron Prime
        /// </para>
        /// </summary>
        SkeletronPrimeHead = 32,

        /// <summary>
        /// Occasionally charges at the player, heads directly towards player when enraged.
        /// <para>
        /// Used by: Prime Saw
        /// </para>
        /// </summary>
        PrimeSaw = 33,

        /// <summary>
        /// Occasionally charges at the player, rapidly charges when enraged.
        /// <para>
        /// Used by: Prime Vice
        /// </para>
        /// </summary>
        PrimeVice = 34,

        /// <summary>
        /// Fires bombs upwards, aims directly at player when enraged.
        /// <para>
        /// Used by: Prime Cannon
        /// </para>
        /// </summary>
        PrimeCannon = 35,

        /// <summary>
        /// Fires projectiles at player, shoots very rapidly when enraged.
        /// <para>
        /// Used by: Prime Laser
        /// </para>
        /// </summary>
        PrimeLaser = 36,

        /// <summary>
        /// Similar to worm AI except shoot projectiles from the body and tail. It will also release Probes.
        /// <para>
        /// Used by: The Destroyer
        /// </para>
        /// </summary>
        TheDestroyer = 37,

        /// <summary>
        /// Jumps and runs toward the player, similar to Fighter AI.
        /// <para>
        /// Used by: Snowman Gangsta, Mister Stabby, Snow Balla
        /// </para>
        /// </summary>
        Snowman = 38,

        /// <summary>
        /// Crawls a bit, leaps toward player.
        /// <para>
        /// Used by: Giant Tortoise, Ice Tortoise, Giant Shelly
        /// </para>
        /// </summary>
        Tortoise = 39,

        /// <summary>
        /// Capable of climbing background walls as well as through platforms. Similar to Fighter AI when there is no background wall.
        /// <para>
        /// Used by: Black Recluse, Wall Creeper, Jungle Creeper, Blood Crawler
        /// </para>
        /// </summary>
        Spider = 40,

        /// <summary>
        /// Jumps high and attempts to land on the player. It can strafe to the sides in midair.
        /// <para>
        /// Used by: Herpling, Derpling
        /// </para>
        /// </summary>
        Herpling = 41,

        /// <summary>
        /// Turns into a Nymph when a player gets too close.
        /// <para>
        /// Used by: Lost Girl
        /// </para>
        /// </summary>
        LostGirl = 42,

        /// <summary>
        /// Alternates between attempting to stay above player while firing projectiles downards, and charging back and forth very rapidly.
        /// <para>
        /// Used by: Queen Bee
        /// </para>
        /// </summary>
        QueenBee = 43,

        /// <summary>
        /// Flies straight towards player.
        /// <para>
        /// Used by: Flying Fish
        /// </para>
        /// </summary>
        FlyingFish = 44,

        /// <summary>
        /// Jumps towards player every few seconds, shoots lasers.
        /// <para>
        /// Automatically Spawns: 1x Golem Head, 2x Golem Fist
        /// </para>
        /// <para>
        /// Used by: Golem
        /// </para>
        /// </summary>
        GolemBody = 45,

        /// <summary>
        /// Bound to an entity.
        /// <para>
        /// Used by: Golem Head
        /// </para>
        /// </summary>
        GolemHead = 46,

        /// <summary>
        /// Flies towards player, returns when hit
        /// <para>
        /// Used by: Golem Fist
        /// </para>
        /// </summary>
        GolemFist = 47,

        /// <summary>
        /// Attempts to fly back and forth, shoots projectiles at player,
        /// <para>
        /// Used by: Golem Head
        /// </para>
        /// </summary>
        FlyingGolemHead = 48,

        /// <summary>
        /// Attempts to stay directly above the player, and fires projectiles downwards.
        /// <para>
        /// Used by: Angry Nimbus
        /// </para>
        /// </summary>
        AngryNimbus = 49,

        /// <summary>
        /// Drifts downwards while following player, destroyed on contact.
        /// <para>
        /// Used by: Fungi Spore, Spore
        /// </para>
        /// </summary>
        Spore = 50,

        /// <summary>
        /// Clings to nearby blocks, chases player, fires projectiles and spiky balls that bounce around.
        /// <para>
        /// Automatically Spawn: 3x Plantera's Hook
        /// </para>
        /// <para>
        /// Used by: Plantera
        /// </para>
        /// </summary>
        Plantera = 51,

        /// <summary>
        /// Moves forward briefly before latching onto a block.
        /// <para>
        /// Used by: Plantera's Hook
        /// </para>
        /// </summary>
        PlanterasHook = 52,

        /// <summary>
        /// Acts very similar to the Plant AI, only bound to a certain entity.
        /// <para>
        /// Used by: Plantera's Tentacle
        /// </para>
        /// </summary>
        PlanterasTentacle = 53,

        /// <summary>
        /// Doesn't adhere to gravity or tile collisions, and teleports occasionally in first form. Once all Creepers are killed, it will begin rapidly teleporting and toward the player.
        /// <para>
        /// Used by: Brain of Cthulhu
        /// </para>
        /// </summary>
        BrainOfCthulhu = 54,

        /// <summary>
        /// Circles around an entity, charging at player.
        /// <para>
        /// Used by: Creeper
        /// </para>
        /// </summary>
        Creeper = 55,

        /// <summary>
        /// Moves directly towards player, gaining momentum. Emits blue-ish particles.
        /// <para>
        /// Used by: Dungeon Spirit
        /// </para>
        /// </summary>
        DungeonSpirit = 56,

        /// <summary>
        /// Moves towards player, stops, and fires projectiles straight at the player.
        /// <para>
        /// Used by: Mourning Wood, Everscream
        /// </para>
        /// </summary>
        MouningWood = 57,

        /// <summary>
        /// Tries to stay above player.
        /// <para>
        /// Automatically Spawns: 2x Pumpking Scythe
        /// </para>
        /// <para>
        /// Used by: Pumpking
        /// </para>
        /// </summary>
        Pumpking = 58,

        /// <summary>
        /// Floats around and attempts to chop at the player.
        /// <para>
        /// Used by: Pumpking Scythe
        /// </para>
        /// </summary>
        PumpkingScythe = 59,

        /// <summary>
        /// Flies around, shooting a barrage of ice-based projectiles.
        /// <para>
        /// Used by: Ice Queen
        /// </para>
        /// </summary>
        IceQueen = 60,

        /// <summary>
        /// Moves across the ground, stops moving, and launches many different types of projectiles.
        /// <para>
        /// Used by: Santa-NK1
        /// </para>
        /// </summary>
        SantaNK1 = 61,

        /// <summary>
        /// Attempts to fly around the player, shooting bullets rapidly if they are in sight.
        /// <para>
        /// Used by: Elf Copter
        /// </para>
        /// </summary>
        ElfCopter = 62,

        /// <summary>
        /// Flies towards the player at high speed.
        /// <para>
        /// Used by: Flocko
        /// </para>
        /// </summary>
        Flocko = 63,

        /// <summary>
        /// Flies slowly in any direction and occasionally glows.
        /// <para>
        /// Used by: Firefly, Lightning Bug
        /// </para>
        /// </summary>
        Firefly = 64,

        /// <summary>
        /// Flies slowly in any direction.
        /// <para>
        /// Used by: Butterfly
        /// </para>
        /// </summary>
        Butterfly = 65,

        /// <summary>
        /// Moves along the ground, pauses for a bit, then continues moving. Will avoid walls and moves in the other direction if it has hit one.
        /// <para>
        /// Used by: Worm, Truffle Worm
        /// </para>
        /// </summary>
        PassiveWorm = 66,

        /// <summary>
        /// Moves along the ground, pauses for a bit, then continues moving. Climbs up walls if it contacts them.
        /// <para>
        /// Used by: Snail, Glowing Snail
        /// </para>
        /// </summary>
        Snail = 67,

        /// <summary>
        /// Swims in water, or walks on land. Will fly when a player is nearby, landing after a while.
        /// <para>
        /// Used by: Duck, Mallard Duck
        /// </para>
        /// </summary>
        Duck = 68,

        /// <summary>
        /// Rams player multiple times before summoning entities. In second form, it flies in circles and summons entities.
        /// <para>
        /// Used by: Duke Fishron
        /// </para>
        /// </summary>
        DukeFishron = 69,

        /// <summary>
        /// Flies through the air, chases player, disappears after a period of time.
        /// <para>
        /// Used by: Detonating Bubble
        /// </para>
        /// </summary>
        DetonatingBubble = 70,

        /// <summary>
        /// Follows an arcing path, dies when it touches a wall or player.
        /// <para>
        /// Used by: Sharkron
        /// </para>
        /// </summary>
        Sharkron = 71,

        /// <summary>
        /// Surrounds Martian Officer with a bubble shield, preventing all damage until it is destroyed.
        /// <para>
        /// Used by: Martian Officer's Bubble Shield
        /// </para>
        /// </summary>
        BubbleShield = 72,

        /// <summary>
        /// Stays stationary and shoots at the player.
        /// <para>
        /// Used by: Tesla Turret
        /// </para>
        /// </summary>
        TeslaTurret = 73,

        /// <summary>
        /// Creeps up from a lower corner of the screen and then attacks, spinning.
        /// <para>
        /// Used by: Martian Drone, Corite
        /// </para>
        /// </summary>
        CreepingSpinner = 74,

        /// <summary>
        /// Mounted upon another enemy and attacks. Has separate amount of health from the enemy it's mounted upon.
        /// <para>
        /// Used by: Martian Saucer (First phase), Saucer Turret, Saucer Cannon, Scutlix Gunner, Dutchman Cannon, Drakomire Rider
        /// </para>
        /// </summary>
        MountedGunner = 75,

        /// <summary>
        /// Repeatedly charges back and forth above the player's head, attacking frantically.
        /// <para>
        /// Automatically Spawns: 1x Martian Saucer, 2x Saucer Turret, 2x Saucer Cannon
        /// </para>
        /// <para>
        /// Used by: Saucer Core
        /// </para>
        /// </summary>
        SaucerCore = 76,

        /// <summary>
        /// Flies very quickly toward the player and floats directly behind players as well as all tiles. Teleports right on top of the player if they get too far away.
        /// <para>
        /// Automatically Spawns: 2x Moon Lord Hand, 1x Moon Lord Head
        /// </para>
        /// <para>
        /// Used by: Moon Lord Core
        /// </para>
        /// </summary>
        MoonLordCore = 77,

        /// <summary>
        /// Hover's out to the side of Moon Lord Core, occasionally shooting ice attacks at the player.
        /// <para>
        /// Used by: Moon Lord Hand
        /// </para>
        /// </summary>
        MoonLordHand = 78,

        /// <summary>
        /// Hovers just above Moon Lord Core, occasionally shooting a sweeping laser beam or spawning Moon Leech Clots at the player.
        /// <para>
        /// Used by: Moon Lord Head
        /// </para>
        /// </summary>
        MoonLordHead = 79,

        /// <summary>
        /// Flies horizontally above the world surface looking for players. Flies away once found, and if allowed to escape, initiates the Martian Madness event.
        /// <para>
        /// Used by: Martian Probe
        /// </para>
        /// </summary>
        MartianProbe = 80,

        /// <summary>
        /// Flies around the general vacinity of Moon Lord Core, occasionally sweeping diagonally into players, shooting lasers, or shooting ice projectiles.
        /// <para>
        /// Used by: True Eye of Cthulhu
        /// </para>
        /// </summary>
        TrueEyeOfCthulhu = 81,

        /// <summary>
        /// Latches onto the player, inflicts the Moon Bite debuff, and heals Moon Lord.
        /// <para>
        /// Used by: Moon Leech Clot
        /// </para>
        /// </summary>
        MoonLeechClot = 82,

        /// <summary>
        /// If used by Lunar Tablet, it floats in the air and spawns Cultists. If used by Cultists, they stands still, ignore all knockback, and perform a cultist ritual.
        /// <para>
        /// Used by: Lunar Tablet, Cultist
        /// </para>
        /// </summary>
        Cultist = 83,

        /// <summary>
        /// If used by Lunatic Cultist, it teleports around using magic against the player. If used by Lunatic Cultist's clones, they follow him.
        /// <para>
        /// Used by: Lunatic Cultist, Lunatic Cultist's clones
        /// </para>
        /// </summary>
        LunaticCultist = 84,

        /// <summary>
        /// Flies to the player and latches onto them, inflicting constant damage.
        /// <para>
        /// Used by: Star Cell, Nebula Headcrab (Brain Suckler), Deadly Sphere
        /// </para>
        /// </summary>
        DeadlySphere = 85,


        /// <summary>
        /// Flies through tiles, deals damage to the player on contact, and ignores knockback.
        /// <para>
        /// Used by: Shadowflame Apparition (Goblin Summoner's minion), Ancient Vision (Lunatic Cultist's minion)
        /// </para>
        /// </summary>
        FlyingHead = 86,

        /// <summary>
        /// Hops around on the ground, occasionally flying extremely quickly into the player.
        /// <para>
        /// Used by: Corrupt Mimic, Crimson Mimic, Hallowed Mimic, Jungle Mimic
        /// </para>
        /// </summary>
        BigMimic = 87,

        /// <summary>
        /// Flies straight into the player and does contact damage while traveling through tiles.
        /// <para>
        /// Used by: Mothron
        /// </para>
        /// </summary>
        Mothron = 88,

        /// <summary>
        /// Hatches into Baby Mothrons if not destroyed quickly.
        /// <para>
        /// Used by: Mothron Egg
        /// </para>
        /// </summary>
        MothronEgg = 89,

        /// <summary>
        /// Flies through tiles and does contact damage to the player.
        /// <para>
        /// Used by: Mothron Egg
        /// </para>
        /// </summary>
        BabyMothron = 90,

        /// <summary>
        /// Floats slowly toward the player, passing through tiles if the player is far enough. In expert mode, it will drop onto the ground once attacked, making it temporarily invulnerable.
        /// <para>
        /// Used by: Granite Elemental
        /// </para>
        /// </summary>
        GraniteElemental = 91,

        /// <summary>
        /// Does not move, but can be attacked. It cannot be killed.
        /// <para>
        /// Used by: Target Dummy
        /// </para>
        /// </summary>
        TargetDummy = 92,

        /// <summary>
        /// Floats above the player, occasionally spawning either a Pirate Deckhand, a Pirate Corair, a Pirate Deadeye, or a Pirate Crossbower.
        /// <para>
        /// Automatically Spawns: 4x Dutchman Cannon
        /// </para>
        /// <para>
        /// Used by: Thing
        /// </para>
        /// </summary>
        FlyingDutchman = 93,

        /// <summary>
        /// Hovers up and down slowly. Has a shield that prevents all damage.
        /// If used by the of the 4 Celestial Towers, the shield can be destroyed after killing 100 (150 in expert mode) of its respective minions.
        /// <para>
        /// Used by: Vortex Pillar, Solar Pillar, Nebular Pillar, Stardust Pillar
        /// </para>
        /// </summary>
        CelestialTower = 94,

        /// <summary>
        /// Floats around, dealing contact damage to player.
        /// <para>
        /// Used by: Miniature Star Cell
        /// </para>
        /// </summary>
        MiniStarCell = 95,

        /// <summary>
        /// Flys around spawning projectiles that orbit around it until fired at the player.
        /// <para>
        /// Used by: Flow Invader
        /// </para>
        /// </summary>
        FlowInvader = 96,

        /// <summary>
        /// Floats around, spawning minion projectiles that shoot lightning.
        /// <para>
        /// Used by: Nebula Floater
        /// </para>
        /// </summary>
        NebulaFloater = 97,
    }
}
