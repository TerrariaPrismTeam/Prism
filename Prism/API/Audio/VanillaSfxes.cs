using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.API.Audio
{
    public static class VanillaSfxes
    {
        public static SfxEntry
            DigBlock, PlayerHit, UseItem, NpcHit, NpcKilled, PlayerKilled, CutGrass,
            GrabItem, DoorOpen, DoorClose, MenuOpen, MenuClose, MenuTick, Shatter,
            ZombieIdle, NpcAttackSound, DoubleJump, Run, Buy, Splash, FemaleHit,
            DigOre, Unlock, Drown, Chat, MaxMana, MummyIdle, PixieIdle,
            MechBuzz, NpcIdleSound, DuckIdle, FrogIdle, NpcIdleSoundQuiet, BeetleIdle, AmbientWater,
            AmbientLava, NpcAttackSoundExpert, Meowmere, CoinPickup, AmbientDrip, Camera, MoonLordCry;

        static SfxEntry GetVanilla(SoundEffect   e , SfxPlayBehaviour b = SfxPlayBehaviour.Singleton, bool ambient = false)
        {
            return new SfxEntry(e.CreateInstance, b, ambient);
        }
        static SfxEntry GetVanilla(SoundEffect[] es, SfxPlayBehaviour b = SfxPlayBehaviour.Singleton, bool ambient = false)
        {
            return new SfxEntry(i => es[i == -1 || i >= es.Length ? Main.rand.Next(es.Length) : i].CreateInstance(), es.Length, _ => b, ambient);
        }
        static SfxEntry GetVanilla(SoundEffect[] es, Func<int, SfxPlayBehaviour> b, bool ambient = false)
        {
            return new SfxEntry(i => es[i == -1 || i >= es.Length ? Main.rand.Next(es.Length) : i].CreateInstance(), es.Length,      b, ambient);
        }

        static void PopulateDict()
        {
            foreach (var fi in typeof(VanillaSfxes).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var se = (SfxEntry)fi.GetValue(null);

                se.InternalName = fi.Name;
                se.Mod = PrismApi.VanillaInfo;

                Sfx.VanillaDict.Add(fi.Name, se);
            }
        }

        internal static void Reset()
        {
            DigBlock = PlayerHit = UseItem = NpcHit = NpcKilled = PlayerKilled = CutGrass
                = GrabItem = DoorOpen = MenuOpen = MenuClose = MenuTick = Shatter
                = ZombieIdle = NpcAttackSound = DoubleJump = Run = Buy = Splash = FemaleHit
                = DigOre = Unlock = Drown = Chat = MaxMana = MummyIdle = PixieIdle
                = MechBuzz = NpcIdleSound = DuckIdle = FrogIdle = NpcIdleSoundQuiet = BeetleIdle = AmbientWater
                = AmbientLava = NpcAttackSoundExpert = Meowmere = CoinPickup = AmbientDrip = Camera = MoonLordCry
                = null;
        }
        internal static void FillVanilla()
        {
            DigBlock  = GetVanilla(Main.soundDig      );
            PlayerHit = GetVanilla(Main.soundPlayerHit);

            UseItem   = GetVanilla(Main.soundItem     , v =>
            {
                switch (v)
                {
                    case 55:
                    case 57:
                        return SfxPlayBehaviour.PlayIfStopped;
                    case 9:
                    case 10:
                    case 24:
                    case 26:
                    case 34:
                    case 43:
                    case 103:
                        return SfxPlayBehaviour.MultipleInstances;
                    default:
                        return SfxPlayBehaviour.Singleton;
                }
            });
            NpcHit    = GetVanilla(Main.soundNPCHit   , v => v == 57 /* moon lord roar */ ? SfxPlayBehaviour.PlayIfStopped : SfxPlayBehaviour.Singleton        );
            NpcKilled = GetVanilla(Main.soundNPCKilled, v => v == 10 /* wall of flesh  */ ? SfxPlayBehaviour.PlayIfStopped : SfxPlayBehaviour.MultipleInstances);

            PlayerKilled = GetVanilla(Main.soundPlayerKilled);
            CutGrass        = GetVanilla(Main.soundGrass       );
            GrabItem         = GetVanilla(Main.soundGrab        );
            DoorOpen     = GetVanilla(Main.soundDoorOpen    );
            DoorClose    = GetVanilla(Main.soundDoorClosed  );
            MenuClose    = GetVanilla(Main.soundMenuClose   );
            MenuOpen     = GetVanilla(Main.soundMenuOpen    );
            MenuTick     = GetVanilla(Main.soundMenuTick    );
            Shatter      = GetVanilla(Main.soundShatter     );

            ZombieIdle = new SfxEntry(v =>
            {
                v = v == NPCID.BloodZombie ? Main.rand.Next(21, 24) : Main.rand.Next(3);

                return Main.soundZombie[v].CreateInstance();
            }, NPCID.BloodZombie + 1, _ => SfxPlayBehaviour.MultipleInstances);

            NpcAttackSound = new SfxEntry(v => Main.soundRoar[v == -1 ? 0 : v].CreateInstance(), Main.soundRoar.Length, _ => SfxPlayBehaviour.PlayIfStopped);

            DoubleJump = GetVanilla(Main.soundDoubleJump);
            Run        = GetVanilla(Main.soundRun       );

            Buy        = GetVanilla(Main.soundCoins     , SfxPlayBehaviour.MultipleInstances);

            Splash = GetVanilla(Main.soundSplash, SfxPlayBehaviour.PlayIfStopped);

            FemaleHit = GetVanilla(Main.soundFemaleHit);
            DigOre    = GetVanilla(Main.soundTink     );
            Unlock    = GetVanilla(Main.soundUnlock   );
            Drown     = GetVanilla(Main.soundDrown    );

            Chat      = GetVanilla(Main.soundChat     , SfxPlayBehaviour.MultipleInstances);
            MaxMana   = GetVanilla(Main.soundMaxMana  , SfxPlayBehaviour.MultipleInstances);

            MummyIdle    = GetVanilla(Main.soundZombie.Subarray(3, 3), SfxPlayBehaviour.MultipleInstances);
            PixieIdle    = GetVanilla(Main.soundPixie, SfxPlayBehaviour.PlayIfStoppedUpdateParams        );
            MechBuzz     = GetVanilla(Main.soundMech, SfxPlayBehaviour.PlayIfStopped                     );
            NpcIdleSound = GetVanilla(Main.soundZombie, SfxPlayBehaviour.PlayIfStopped                   );

            DuckIdle = new SfxEntry(v =>
            {
                v = Main.rand.Next(300) == 0 ? 12 : (v == -1 ? Main.rand.Next(10, 12) : v + 10);

                return Main.soundZombie[v].CreateInstance();
            }, 2, _ => SfxPlayBehaviour.PlayIfStopped, true);

            FrogIdle               = GetVanilla(Main.soundZombie[13], SfxPlayBehaviour.MultipleInstances, true);
            NpcIdleSoundQuiet = GetVanilla(Main.soundZombie, SfxPlayBehaviour.PlayIfStopped        , true);
            BeetleIdle              = GetVanilla(Main.soundZombie[15], SfxPlayBehaviour.PlayIfStopped    , true);

            AmbientWater = GetVanilla(Main.soundLiquid[0], SfxPlayBehaviour.PlayIfStoppedUpdateParams, true);
            AmbientLava  = GetVanilla(Main.soundLiquid[1], SfxPlayBehaviour.PlayIfStoppedUpdateParams, true);

            NpcAttackSoundExpert = new SfxEntry(v => Main.soundRoar[v == -1 ? 0 : v].CreateInstance(), Main.soundRoar.Length, _ => SfxPlayBehaviour.MultipleInstances);

            Meowmere   = GetVanilla(Main.soundItem.Subarray(57, 2), SfxPlayBehaviour.MultipleInstances);
            CoinPickup = GetVanilla(Main.soundCoin, SfxPlayBehaviour.MultipleInstances                );

            AmbientDrip = GetVanilla(Main.soundDrip, SfxPlayBehaviour.MultipleInstances, true);

            Camera = GetVanilla(Main.soundCamera);

            MoonLordCry = GetVanilla(Main.soundNPCKilled[10], SfxPlayBehaviour.MultipleInstances);

            PopulateDict();
        }
    }
}
