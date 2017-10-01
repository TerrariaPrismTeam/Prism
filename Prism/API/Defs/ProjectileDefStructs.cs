using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prism.API.Defs
{
    /// <summary>
    /// Vanilla Projectile AI Styles
    /// </summary>
    public enum ProjectileAiStyle
    {
        #region Nopezal's Hard Work

        /// <summary>
        /// No AI. Use this if you plan on making completely custom AI.
        /// <para/><b>Used by:</b>Nothing
        /// </summary>
        None = 0,
        /// <summary>
        /// Basic arrow and bullet projectile AI. (also used for Lunar Flare)
        /// <para/><b>Used by:</b>Wooden Arrow, Flaming Arrow, Musket Ball, Meteor Shot, Lunar Flare
        /// </summary>
        Arrow = 1,
        /// <summary>
        /// Shuriken projectile AI. Affected by gravity.
        /// <para/><b>Used by:</b>Shuriken, Throwing Knife, Holy Water
        /// </summary>
        Shuriken = 2,
        /// <summary>
        /// Boomerang AI. Returns to sender.
        /// <para/><b>Used by:</b>Wooden Boomerang, Enchanted Boomerang, Flamarang, Light Disc
        /// </summary>
        Boomerang = 3,
        /// <summary>
        /// Vilethorn AI. Fades in a projectile, and quickly fades it out.
        /// <para/><b>Used by:</b>
        /// </summary>
        Vilethorn = 4,
        /// <summary>
        /// Falling Star AI. Projectile spins and emits particles.
        /// <para/><b>Used by:</b>
        /// </summary>Falling Star, Starfury, Star Cannon
        Star = 5,
        /// <summary>
        /// Vile/Purification Powder AI. Slows down over time.
        /// <para/><b>Used by:</b>Vile Powder, Purification Powder, Vicious Powder
        /// </summary>
        Powder = 6,
        /// <summary>
        /// Grappling Hook AI. Projectile attaches to blocks.
        /// <para/><b>Used by:</b>Grappling Hook, the Gem Hooks, Dual Hook
        /// </summary>
        Hook = 7,
        /// <summary>
        /// Fire Flower AI. Projectile bounces on blocks.
        /// <para/><b>Used by:</b>Fire Flower, Ice Flower, Cursed Flames
        /// </summary>
        Bouncing = 8,
        /// <summary>
        /// Magic Missile AI. Follows cursor.
        /// <para/><b>Used by:</b> Magic Missile, Flamelash, Rainbow Rod, Flying Knife
        /// </summary>
        MagicMissile = 9,
        /// <summary>
        /// Falling Sand AI. Projectile is affected by gravity.
        /// <para/><b>Used by:</b>Dirt Rod, Falling Sand, Falling Silt, Falling Slush
        /// </summary>
        FallingSand = 10,
        /// <summary>
        /// Shadow Orb AI. Projectile follows the player.
        /// <para/><b>Used by:</b>Shadow Orb, Fairy Bell
        /// </summary>
        ShadowOrb = 11,
        /// <summary>
        /// Water Stream effect. Projectile emits particles.
        /// <para/><b>Used by:</b>Water Stream, Golden Shower
        /// </summary>
        Stream = 12,
        /// <summary>
        /// Harpoon AI. Projectile is affected by gravity and retracts afterwards.
        /// <para/><b>Used by:</b>Harpoon, KO Punch, Chain Knife
        /// </summary>
        Harpoon = 13,
        /// <summary>
        /// Spiky Ball AI. Projectile is affected by gravity and rolls on the surface for a while.
        /// <para/><b>Used by:</b>Spiky Ball
        /// </summary>
        SpikyBall = 14,
        /// <summary>
        /// Flail AI. Projectile is affected by gravity and immediatly remains stationary.
        /// <para/><b>Used by:</b>Ball o' Hurt, Blue Moon, Dao of Pow
        /// </summary>
        FlailBall = 15,
        /// <summary>
        /// Bomb AI. Projectile is affected by gravity and rolls longer/shorter based on the angle it is thrown.
        /// <para/><b>Used by:</b>Bomb, Dynamite, Grenade
        /// </summary>
        Bomb = 16,
        /// <summary>
        /// Tombstone AI. Projectile is affected by gravity.
        /// <para/><b>Used by:</b>Tombstone
        /// </summary>
        Tombstone = 17,
        /// <summary>
        /// Sickle AI. Projectile speeds up exponentially.
        /// <para/><b>Used by:</b>Demon Scythe, Demon Sickle
        /// </summary>
        Sickle = 18,
        /// <summary>
        /// Spear AI. Projectile extends and retracts.
        /// <para/><b>Used by:</b>Spear, Trident, Gungnir
        /// </summary>
        Spear = 19,
        /// <summary>
        /// Drill AI. Projectile rotates towards the cursor.
        /// <para/><b>Used by:</b>Hardmode Drills and Chainsaws, Drax
        /// </summary>
        Drill = 20,
        /// <summary>
        /// Harp note AI. Projectile velocity determined by the distance between player and cursor, bounces off walls and emits particles.
        /// <para/><b>Used by:</b>Magical Harp
        /// </summary>
        HarpNote = 21,
        /// <summary>
        /// Ice Rod AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Ice Rod
        /// </summary>
        IceBlock = 22,
        /// <summary>
        /// Spazmatism Flame effect. Projectile emits particles.
        /// <para/><b>Used by:</b>Spazmatism Flame
        /// </summary>
        SpazFlame = 23,
        /// <summary>
        /// Crystal Storm AI. Projectile exponentially slows down.
        /// <para/><b>Used by:</b>Crystal Storm
        /// </summary>
        CrystalStorm = 24,
        /// <summary>
        /// Boulder AI. Projectile is affected by gravity and when it hits the ground, rolls untill it hits a block.
        /// <para/><b>Used by:</b>Boulder
        /// </summary>
        Boulder = 25,
        /// <summary>
        /// Pet Bunny AI. Projectile follows the player in the air and on the ground.
        /// <para/><b>Used by:</b>Pet Bunny, Pet Turtle, Pet Dinosaur
        /// </summary>
        PetBunny = 26, //Really, used for most pets, but since one of the Pumpkin Moon pets doesn't I called it "pet bunny"
        /// <summary>
        /// Sword Beam AI. Projectile fades in and flies towards the cursor.
        /// <para/><b>Used by:</b>Beam Sword, True Excalibur/Night's Edge, Terra Blade
        /// </summary>
        SwordBeam = 27,
        /// <summary>
        /// Frost Staff AI. Projectile emits particles.
        /// <para/><b>Used by:</b>Frost Staff, Ice Sword
        /// </summary>
        FrostBolt = 28,
        /// <summary>
        /// Staff AI. Projectile emits particles.
        /// <para/><b>Used by:</b>Gem Staves, Nebula Arcanum
        /// </summary>
        StaffBolt = 29, //Used for nebula arcanum too O_o
        /// <summary>
        /// Mushroom Spear Projectiles AI. Projectile is stationary and unaffected by gravity.
        /// <para/><b>Used by:</b>Mushroom Spear
        /// </summary>
        Mushroom = 30,
        /// <summary>
        /// Clentaminator Spray effect. Projectile emits particles.
        /// <para/><b>Used by:</b>Clentaminator Solutions
        /// </summary>
        ClentaminatorSpray = 31,
        /// <summary>
        /// Beach Ball AI. Projectile is affected by gravity and bounces on the ground. Projectile also floats.
        /// <para/><b>Used by:</b>Beach Ball
        /// </summary>
        BeachBall = 32,
        /// <summary>
        /// Flare AI. Projectile sticks to surface.
        /// <para/><b>Used by:</b>Flares
        /// </summary>
        Flare = 33,
        /// <summary>
        /// Rocket AI. Projectile goes in a straight line and emits particles.
        /// <para/><b>Used by:</b>Rockets
        /// </summary>
        Rocket = 34,
        /// <summary>
        /// Rope Coil AI. Projectile is affected by gravity.
        /// <para/><b>Used by:</b>Rope Coil
        /// </summary>
        RopeCoil = 35,
        /// <summary>
        /// Bee AI. Projectile homes in on enemies.
        /// <para/><b>Used by:</b>Bee
        /// </summary>
        Bee = 36,
        /// <summary>
        /// Spear AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Spear
        /// </summary>
        Spear2 = 37, //Weird that there are 2 spear AIs...
        /// <summary>
        /// Flamethrower AI. Projectile emits particles.
        /// <para/><b>Used by:</b>Flamethrower, Elf Melter
        /// </summary>
        Flamethrower = 38,
        /// <summary>
        /// Piranha Gun AI. Projectile homes in and sticks to enemies until the left mouse button is released or the enemy is defeated, in wich case it will stick to another enemy.
        /// <para/><b>Used by:</b>Piranha Gun
        /// </summary>
        MechPiranha = 39,
        /// <summary>
        /// Leaf AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Leaf Blower
        /// </summary>
        Leaf = 40,
        /// <summary>
        /// Flower of Power petal AI. Projectile slowly falls like a leaf.
        /// <para/><b>Used by:</b>Flower of Power
        /// </summary>
        Petal = 41,
        /// <summary>
        /// Orichalcum set bonus AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Orichalcum Armor
        /// </summary>
        CrystalLeaf = 42,
        /// <summary>
        /// Orichalcum set bonus AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Orichalcum Armor
        /// </summary>
        CrystalLeaf2 = 43, //Probably orichalcum stuff
        /// <summary>
        /// Chlorophyte Saber Projectile AI. Projectile slows down and fades out.
        /// <para/><b>Used by:</b>Chlorophyte Saber, Chlorophyte Partisan
        /// </summary>
        SporeCloud = 44,
        /// <summary>
        /// Crimson Rod AI. Projectile flies towards cursor and remains stationarry at that spot.
        /// <para/><b>Used by:</b>Crimson Rod, Nimbus Rod
        /// </summary>
        Cloud = 45,
        /// <summary>
        /// Rainbow Gun AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Rainbow Gun
        /// </summary>
        Rainbow = 46,
        /// <summary>
        /// Magnet Sphere AI. Projectile goes very slow and fades out.
        /// <para/><b>Used by:</b>
        /// </summary>
        MagnetSphere = 47,
        /// <summary>
        /// Magnet Sphere AI. (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        MagnetSphereBeam = 48, //REQUIRES CHECKING, Pretty sure I'm right tho
        /// <summary>
        /// Explosive Bunny AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Bunny Cannon, Explosive Bunny
        /// </summary>
        ExplosiveBunny = 49,
        /// <summary>
        /// Inferno Fork AI. (needsDesc)
        /// <para/><b>Used by:</b>Inferno Fork, Diabolist
        /// </summary>
        Inferno = 50,
        /// <summary>
        /// Spectre Staff AI. Projectile homes into enemies.
        /// <para/><b>Used by:</b>Spectre Staff, Ragged Caster
        /// </summary>
        LostSoul = 51,
        /// <summary>
        /// Spectre Heal AI. Projectile homes into player.
        /// <para/><b>Used by:</b>Spectre Set Bonus
        /// </summary>Spectre Set Bonus
        SpectreHeal = 52, //renamed cuz this makes more sense
        /// <summary>
        /// Frost Hydra AI. Projectile sprite direction always faces the nearest enemy.
        /// <para/><b>Used by:</b>Frost Hydra
        /// </summary>
        FrostHydra = 53,
        /// <summary>
        /// Raven Minion AI. Projectile stays near the player and homes into enemies.
        /// <para/><b>Used by:</b>
        /// </summary>
        MinionRaven = 54,
        /// <summary>
        /// (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        FlamingJack = 55, //Jack o Lantern Launcher? Never used it
        /// <summary>
        /// Pumpking Scythe Projectile AI. Projectile rotates and accelerates.
        /// <para/><b>Used by:</b>
        /// </summary>
        FlamingScythe = 56, //Pumpking's proj
        /// <summary>
        /// North Pole projectile AI. Projectile is affected by gravity
        /// <para/><b>Used by:</b>
        /// </summary>
        NorthPoleFlakes = 57,
        /// <summary>
        /// Santa-NK1 Gift AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Santa-NK1
        /// </summary>
        SantankPresent = 58,
        /// <summary>
        /// Spectre Set Bonus AI. Projectiles home into enemies.
        /// <para/><b>Used by:</b>Spectre Set Bonus
        /// </summary>
        SpectreDamage = 59, //Again, renamed cuz this makes more sense
        /// <summary>
        /// Water Gun AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Water Gun
        /// </summary>
        WaterGunStream = 60,
        /// <summary>
        /// (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        Bobber = 61, //requires bobber bool to be active (atleast, pretty sure it is)
        /// <summary>
        /// Hornet Minion AI. Projectiles follow the player and come closer to enemies, but not contact range.
        /// <para/><b>Used by:</b>Hornet Minions
        /// </summary>
        MinionHornet = 62,
        /// <summary>
        /// Spider Turret Baby AI. Projectiles run over the surface and jump onto an enemy.
        /// <para/><b>Used by:</b>Spider Turret
        /// </summary>
        SpiderTurretBaby = 63, //the spiders that pop outta the eggs
        /// <summary>
        /// Sharknado AI. Projectiles follow player and stay on a distance from enemies.
        /// <para/><b>Used by:</b>Sharknado
        /// </summary>
        Sharknado = 64,
        /// <summary>
        /// Sharknado Bolt AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Sharknado Bolt
        /// </summary>
        SharknadoShark = 65,
        /// <summary>
        /// Twins Minion AI. (NeedsDesc, since it's used for both twins)
        /// <para/><b>Used by:</b>Twins Minion
        /// </summary>
        MinionTwins = 66,
        /// <summary>
        /// Pirate Minion AI. Projectile follows player and stays at range from enemies on the ground.
        /// <para/><b>Used by:</b>Pirate Minion
        /// </summary>
        MinionPirate = 67,
        /// <summary>
        /// Molotov Cocktail AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Molotov Cocktail
        /// </summary>
        MolotovCocktail = 68,
        /// <summary>
        /// Flairon AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Flairon
        /// </summary>
        Flairon = 69,
        /// <summary>
        /// Flairon Bubble AI. Projectile homes into enemies.
        /// <para/><b>Used by:</b>Flairon
        /// </summary>
        FlaironBubble = 70,
        /// <summary>
        /// Razorblade Typhoon AI. Projectile homes into enemies.
        /// <para/><b>Used by:</b>Razorblade Typhoon
        /// </summary>
        RazorbladeTyphoon = 71,
        /// <summary>
        /// Bubble Gun AI. Projectile exponentially slows down and cycles through the last frames before killing it.
        /// <para/><b>Used by:</b>Bubble Gun
        /// </summary>
        Bubble = 72,
        /// <summary>
        /// Firework Fountain AI. Projectile sparks.
        /// <para/><b>Used by:</b>Firework Fountain
        /// </summary>
        FireworkFountain = 73,
        /// <summary>
        /// Scutlix Mount AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Scutlix Mount
        /// </summary>
        ScutlixLaser = 74, //1.3 stuff \o/
        /// <summary>
        /// Laser Machinegun AI. Projectile frames cycle accelerates.
        /// <para/><b>Used by:</b>Laser Machinegun
        /// </summary>
        LaserMachinegun = 75,
        /// <summary>
        /// Scutlix Crosshair AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Scutlix Mount
        /// </summary>
        ScutlixCrosshair = 76,
        /// <summary>
        /// Electrosphere AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Electrosphere Launcher
        /// </summary>
        Electrosphere = 77,
        /// <summary>
        /// Xenopopper AI. Projectile slows down and cycles through the last of its frames.
        /// <para/><b>Used by:</b>Xenopopper
        /// </summary>
        Xenopopper = 78,
        /// <summary>
        /// Martian Deathray AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Martian Saucer
        /// </summary>
        MartianDeathray = 80,
        /// <summary>
        /// Martian Rocket AI. Projectile homes into enemies.
        /// <para/><b>Used by:</b>
        /// </summary>
        MartianRocket = 81,
        /// <summary>
        /// Phantasmal Eye AI. Projectile remains stationary in the air and falls diagonally downwards after a while.
        /// <para/><b>Used by:</b>Moon Lord
        /// </summary>
        PhantasmalEye = 82,
        /// <summary>
        /// Phantasmal Sphere AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Moon Lord/True Eye of Cthulhu
        /// </summary>
        PhantasmalSphere = 83,
        /// <summary>
        /// Phantasmal Deathray AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Moon Lord/True Eye of Cthulhu
        /// </summary>
        PhantasmalDeathRay = 84,
        /// <summary>
        /// Moon Leech AI. Projectile homes into player and remains onto the player for a while. Projectile then retracts.
        /// <para/><b>Used by:</b>Moon Lord
        /// </summary>
        MoonLeech = 85,
        /// <summary>
        /// Ice Mist AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Lunatic Cultist
        /// </summary>
        IceMist = 86,
        /// <summary>
        /// Cursed Flames AI. (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        CursedFlames = 87,
        /// <summary>
        /// Lightning Orb AI. Projectile fades in and remains stationary.
        /// <para/><b>Used by:</b>Lunatic Cultist
        /// </summary>
        LightningOrb = 88,
        /// <summary>
        /// Lightning Ritual AI. (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        LightningRitual = 89,
        /// <summary>
        /// Magic Lantern AI. Projectile follows the player and (maybe? not tested) lights up ores around the projectile.
        /// <para/><b>Used by:</b>Magic Lantern
        /// </summary>
        PetLantern = 90,
        /// <summary>
        /// Shadowflame AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Goblin Summoner
        /// </summary>
        Shadowflame = 91,
        /// <summary>
        /// Toxic Flask Cloud AI. Projectiles slow down and fade out.
        /// <para/><b>Used by:</b>Toxic Flask
        /// </summary>
        ToxicCloud = 92,
        /// <summary>
        /// Nail Gun AI. Projectiles cling onto enemies, ande explode afterwards (?)
        /// <para/><b>Used by:</b>Nail Gun
        /// </summary>
        Nail = 93,
        /// <summary>
        /// Coin Portal AI. (NeedDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        CoinPortal = 94,
        /// <summary>
        /// (NeedDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        ToxicBubble = 95,
        /// <summary>
        /// Blade Tongue AI. Projectile arcs.
        /// <para/><b>Used by:</b>Bladetongue
        /// </summary>
        Bladetongue = 96,
        /// <summary>
        /// Flying piggy bank AI. (NeedDesc)
        /// <para/><b>Used by:</b>Money Through
        /// </summary>
        FlyingPiggyBank = 97,
        /// <summary>
        /// (NeedDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        Energy = 98,
        /// <summary>
        /// Yoyo AI. Projectile follows the cursor until it can't reach.
        /// <para/><b>Used by:</b>Wooden yoyo, Malaise, Kraken, Terrarian
        /// </summary>
        Yoyo = 99,
        /// <summary>
        /// Medusa Head AI. Projectile zaps enemies.
        /// <para/><b>Used by:</b>Medusa Head
        /// </summary>
        MedusaRayLarge = 100, //not sure, needs confirmation
        /// <summary>
        /// Medusa Head AI. Projectile zaps enemies.
        /// <para/><b>Used by:</b>Medusa Head
        /// </summary>
        MedusaRaySmall = 101, //not sure either
        /// <summary>
        /// Flow Invader AI. Projectile aims at the player, then fires.
        /// <para/><b>Used by:</b>
        /// </summary>
        FlowInvader = 102,
        /// <summary>
        /// (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        Starmark = 103,
        /// <summary>
        /// Brain of Confusion AI. (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        BrainOfConfusion = 104,
        /// <summary>
        /// Spore Bag AI. Projectile homes into enemies when there are enemies. If not, it will fade out.
        /// <para/><b>Used by:</b>Spore Bag
        /// </summary>
        Spore = 105,
        /// <summary>
        /// Spore Bag AI. Projectile homes into enemies when there are enemies. If not, it will fade out.
        /// <para/><b>Used by:</b>Spore Bag
        /// </summary>
        Spore2 = 106, //not sure what this is, spore bag?
        /// <summary>
        /// Nebula Sphere AI. Projectile homes into the player.
        /// <para/><b>Used by:</b>Nebula Sphere
        /// </summary>
        NebulaSphere = 107,
        /// <summary>
        /// The portal-like projectiles that spawn monsters or lightning, summoned by the Vortex Pillar.
        /// <para/><b>Used by:</b>Vortex Pillar
        /// </summary>
        Vortex = 108,
        /// <summary>
        /// Mechanic's Wrench AI. Projectile acts like a boomerang, but returns to the NPC that threw it.
        /// <para/><b>Used by:</b>Mechanic's Wrench
        /// </summary>
        MechanicWrench = 109,
        /// <summary>
        /// (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        Syringe = 110, //liquid drops?
        /// <summary>
        /// Dryad's Blessing Leaves AI. (NeedsDesc)
        /// <para/><b>Used by:</b>Dryad's Blessing
        /// </summary>
        DryadWard = 111,
        /// <summary>
        /// Truffle Spore AI. Projectile remains stationary.
        /// <para/><b>Used by:</b>Truffle Spores
        /// </summary>
        TruffleSpore = 112,
        /// <summary>
        /// Javelin AI. Projectile sticks to enemies.
        /// <para/><b>Used by:</b>Bone Javelin, Daybreak
        /// </summary>
        Javelin = 113,
        /// <summary>
        /// Portal Gate AI. (Used by portal gun)
        /// <para/><b>Used by:</b>Portal Gun
        /// </summary>
        PortalGate = 114,
        /// <summary>
        /// Terrarian projectile AI. Homes into enemies very slowly.
        /// <para/><b>Used by:</b>Terrarian.
        /// </summary>
        Terrarian = 115,
        /// <summary>
        /// Solar Flare AI. Projectile acts like a harpoon, but sent at different angles. Optimal when tilecollide is off.
        /// <para/><b>Used by:</b>Solar Flare
        /// </summary>
        SolarFlare = 116,
        /// <summary>
        /// (NeedsDesc, not sure if it's the solar armor shield)
        /// <para/><b>Used by:</b>
        /// </summary>
        SolarRadiance = 117,
        /// <summary>
        /// Nebula Arcanum AI. Projectiles fly around a specified projectile (or fly around the base nebula arcanum)
        /// <para/><b>Used by:</b>Nebula Arcanum
        /// </summary>
        NebulaArcanum = 118,
        /// <summary>
        /// Nebula Arcanum AI. Projectiles fly around a specified projectile (or fly around the base nebula arcanum)
        /// <para/><b>Used by:</b>Nebula Arcanum
        /// </summary>
        NebulaArcanum2 = 119,
        /// <summary>
        /// Stardust Guardian AI. Dependant on your keybindings, projectile will, when DOWN is pressed twice, move to your cursor.
        /// <para/><b>Used by:</b>Stardust Armor Set bonus
        /// </summary>
        StardustGuardian = 120,
        /// <summary>
        /// Stardust Dragon AI. Projectile will fly around you and home into enemies.
        /// <para/><b>Used by:</b>
        /// </summary>
        StardustDragon = 121,
        /// <summary>
        /// (NeedsDesc)
        /// <para/><b>Used by:</b>
        /// </summary>
        ReleasedEnergy = 122,
        /// <summary>
        /// Lunar Portal AI. Projectile will stay stationary in the sky.
        /// <para/><b>Used by:</b>Lunar Portal
        /// </summary>
        LunarPortal = 123,
        /// <summary>
        /// Suspicious Looking Tentacle AI. Projectile will fly around you and (maybe?) light nearby ores and treasure.
        /// <para/><b>Used by:</b>Suspicious Tentacle
        /// </summary>
        SuspiciousTentacle = 124,

        //Also, I added the numbers cuz some of them (like the spear one) might be copies
        #endregion
        #region MageKing17's hard work
        /// <summary>
        /// Wire Kite AI. This is apparently the "wire preview" of the Grand Design, although only some of the behavior is linked to the AI style.
        /// <para/><b>Used by:</b>Wire Kite
        /// </summary>
        WireKite = 125,
        /// <summary>
        /// Geyser Trap AI.
        /// <para/><b>Used by:</b>Geyser Trap, DD2 Ogre Stomp
        /// </summary>
        GeyserTrap = 126,
        /// <summary>
        /// Sandnado AI.
        /// <para/><b>Used by:</b>Sandnado Friendly, Sandnado Hostile
        /// </summary>
        Sandnado = 127,
        /// <summary>
        /// Sandnado Hostile Mark AI.
        /// <para/><b>Used by:</b>Sandnado Hostile Mark
        /// </summary>
        SandnadoHostileMark = 128,
        /// <summary>
        /// Spirit Flame AI.
        /// <para/><b>Used by:</b>Spirit Flame
        /// </summary>
        SpiritFlame = 129,
        /// <summary>
        /// FlameBurst Tower AI.
        /// <para/><b>Used by:</b>DD2 FlameBurst Tower (T1/T2/T3)
        /// </summary>
        FlameBurstTower = 130,
        /// <summary>
        /// FlameBurst Tower Shot AI.
        /// <para/><b>Used by:</b>DD2 FlameBurst Tower (T1/T2/T3) Shot
        /// </summary>
        FlameBurstTowerShot = 131,
        /// <summary>
        /// Elder Wins AI.
        /// <para/><b>Used by:</b>DD2 Elder Wins
        /// </summary>
        ElderWins = 132,
        /// <summary>
        /// Dark Mage Healing AI.
        /// <para/><b>Used by:</b>DD2 Dark Mage Raise, DD2 Dark Mage Heal
        /// </summary>
        DarkMageHeal = 133,
        /// <summary>
        /// Ballistra Tower AI.
        /// <para/><b>Used by:</b>DD2 Ballistra Tower (T1/T2/T3)
        /// </summary>
        BallistraTower = 134,
        /// <summary>
        /// Ogre Smash AI.
        /// <para/><b>Used by:</b>DD2 Ogre Smash
        /// </summary>
        OgreSmash = 135,
        /// <summary>
        /// Betsy Flame Breath AI.
        /// <para/><b>Used by:</b>DD2 Betsy Flame Breath
        /// </summary>
        BetsyFlameBreath = 136,
        /// <summary>
        /// Lightning Aura AI.
        /// <para/><b>Used by:</b>DD2 Lightning Aura (T1/T2/T3)
        /// </summary>
        LightningAura = 137,
        /// <summary>
        /// Explosive Trap AI.
        /// <para/><b>Used by:</b>DD2 Explosive Trap (T1/T2/T3)
        /// </summary>
        ExplosiveTrap = 138,
        /// <summary>
        /// Explosive Trap Explosion AI.
        /// <para/><b>Used by:</b>DD2 Explosive Trap (T1/T2/T3) Explosion
        /// </summary>
        ExplosiveTrapExplosion = 139,
        /// <summary>
        /// Monk Staff AI.
        /// <para/><b>Used by:</b>DD2 Monk Staff (T1/T3)
        /// </summary>
        MonkStaff = 140,
        /// <summary>
        /// Monk Staff Explosion AI.
        /// <para/><b>Used by:</b>DD2 Monk Staff (T1) Explosion
        /// </summary>
        MonkStaffExplosion = 141,
        /// <summary>
        /// Monk Staff 2 AI.
        /// <para/><b>Used by:</b>DD2 Monk Staff (T2/T3-Alt)
        /// </summary>
        MonkStaff2 = 142,
        /// <summary>
        /// Monk Staff Ghast AI.
        /// <para/><b>Used by:</b>DD2 Monk Staff (T2) Ghast
        /// </summary>
        MonkStaffGhast = 143,
        /// <summary>
        /// DD2 Pet AI.
        /// <para/><b>Used by:</b>DD2 Pet Dragon, DD2 Pet Ghost, DD2 Pet Gato
        /// </summary>
        DD2Pet = 144,
        /// <summary>
        /// Apprentice Storm AI.
        /// <para/><b>Used by:</b>DD2 Apprentice Storm
        /// </summary>
        ApprenticeStorm = 145,
        /// <summary>
        /// DD2 Win AI.
        /// <para/><b>Used by:</b>DD2 Win
        /// </summary>
        DD2Win = 146

        #endregion
    }

    //public enum ProjFlags
    //{
    //    Counterweight,
    //    Arrow,
    //    Bobber,
    //    Trap
    //}

    public enum ProjectileDamageType
    {
        None,
        Melee,
        Ranged,
        Magic,
        Summoner,
        Thrown
    }

    /// <summary>
    /// ?TrailingMode?
    /// </summary>
    public enum TrailingMode
    {
        None = -1,
        /// <summary>
        /// ?TrailingMode0?
        /// </summary>
        TrailingMode_0 = 0,
        /// <summary>
        /// ?TrailingMode1?
        /// </summary>
        TrailingMode_1 = 1,
        /// <summary>
        /// ?TrailingMode2?
        /// </summary>
        TrailingMode_2 = 2
    }
}
