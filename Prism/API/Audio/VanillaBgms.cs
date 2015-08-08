using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;

namespace Prism.API.Audio
{
    public static class VanillaBgms
    {
        #region ID arrays
        readonly static int[]
            MoonLordNPCs = { NPCID.MoonLordCore },
            QueenBeeNPCs = { NPCID.QueenBee },

            MartianMadnessNPCs =
            {
                NPCID.BrainScrambler, NPCID.RayGunner , NPCID.MartianOfficer, NPCID.ForceBubble, NPCID.GrayGrunt        , NPCID.MartianEngineer,
                NPCID.MartianDrone  , NPCID.GigaZapper, NPCID.ScutlixRider  , NPCID.Scutlix    , NPCID.MartianSaucerCore, NPCID.MartianWalker
            },
            LunarPillarNPCs =
            {
                NPCID.LunarTowerVortex, NPCID.LunarTowerStardust, NPCID.LunarTowerNebula, NPCID.LunarTowerSolar
            },
            PlanteraNPCs =
            {
                NPCID.Plantera, NPCID.PlanterasHook, NPCID.PlanterasTentacle
            },
            Boss2NPCs =
            {
                NPCID.WallofFleshEye, NPCID.WallofFlesh, NPCID.Retinazer, NPCID.Spazmatism
            },
            Boss1NPCs =
            {
                NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail
            },
            Boss3NPCs =
            {
                NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.SnowmanGangsta, NPCID.MisterStabby, NPCID.SnowBalla, NPCID.BrainofCthulhu
            },
            GolemNPCs =
            {
                NPCID.Golem, NPCID.CultistDevote, NPCID.CultistBoss
            },
            PiratesNPCs =
            {
                NPCID.PirateDeckhand, NPCID.PirateCorsair, NPCID.PirateDeadeye, NPCID.PirateCrossbower, NPCID.PirateCaptain, NPCID.PirateShip
            },
            GoblinArmyNPCs =
            {
                NPCID.GoblinPeon, NPCID.GoblinThief, NPCID.GoblinWarrior, NPCID.GoblinSorcerer, NPCID.GoblinArcher
            };
        #endregion

        static int[] TempNpcArray = new int[1];

        public readonly static BgmEntry[] MusicBoxes = Enumerable.Range(1, 37) // do not include ambient background
                .Select(c => new BgmEntry(Bgm.VanillaBgmOf(MusicBoxIdToCueId(c)), BgmPriority.MusicBox, () => Main.musicBox == c)).ToArray();

        public readonly static BgmEntry
            Title = new BgmEntry(Bgm.VanillaBgmOf(6), BgmPriority.Title, () => Main.gameMenu),

            FrostMoon   = new BgmEntry(Bgm.VanillaBgmOf(32), BgmPriority.Event, () => Main.snowMoon   ),
            PumpkinMoon = new BgmEntry(Bgm.VanillaBgmOf(30), BgmPriority.Event, () => Main.pumpkinMoon),

            MoonLord       = new BgmEntry(Bgm.VanillaBgmOf(38), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  7 : Bgm.AnyNPCsForMusic(MoonLordNPCs      )),
            MartianMadness = new BgmEntry(Bgm.VanillaBgmOf(37), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  9 : Bgm.AnyNPCsForMusic(MartianMadnessNPCs)),
            LunarPillar    = new BgmEntry(Bgm.VanillaBgmOf(34), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId == 10 : Bgm.AnyNPCsForMusic(LunarPillarNPCs   )),
            Plantera       = new BgmEntry(Bgm.VanillaBgmOf(24), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  6 : Bgm.AnyNPCsForMusic(PlanteraNPCs      )),
            Boss2          = new BgmEntry(Bgm.VanillaBgmOf(12), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  2 : Bgm.AnyNPCsForMusic(Boss2NPCs         )),
            Boss1          = new BgmEntry(Bgm.VanillaBgmOf( 5), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  1 : Bgm.AnyNPCsForMusic(IsBoss1Boss       )),
            Boss3          = new BgmEntry(Bgm.VanillaBgmOf(13), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  3 : Bgm.AnyNPCsForMusic(Boss3NPCs         )),
            Golem          = new BgmEntry(Bgm.VanillaBgmOf(17), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  4 : Bgm.AnyNPCsForMusic(GolemNPCs         )),
            QueenBee       = new BgmEntry(Bgm.VanillaBgmOf(25), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  5 : Bgm.AnyNPCsForMusic(QueenBeeNPCs      )),
            Pirates        = new BgmEntry(Bgm.VanillaBgmOf(35), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId ==  8 : Bgm.AnyNPCsForMusic(PiratesNPCs       )),
            GoblinArmy     = new BgmEntry(Bgm.VanillaBgmOf(39), BgmPriority.Boss, () => Bgm.justScanned ? Bgm.bossMusicId == 11 : Bgm.AnyNPCsForMusic(GoblinArmyNPCs    )),

            Underworld   = new BgmEntry(Bgm.VanillaBgmOf(36), BgmPriority.Biome, () => Main.player[Main.myPlayer].position.Y > (Main.maxTilesY - 200) * 16),
            Eclipse      = new BgmEntry(Bgm.VanillaBgmOf(27), BgmPriority.Biome, () => Main.eclipse && Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16 + Main.screenHeight / 2),
            Space        = new BgmEntry(Bgm.VanillaBgmOf(15), BgmPriority.Biome, IsInSpace),
            Lihzahrd     = new BgmEntry(Bgm.VanillaBgmOf(26), BgmPriority.Biome, () => Main.tile[(int)(Main.player[Main.myPlayer].Center.X / 16f), (int)(Main.player[Main.myPlayer].Center.Y / 16f)].wall == WallID.LihzahrdBrickUnsafe),
            Mushrooms    = new BgmEntry(Bgm.VanillaBgmOf(29), BgmPriority.Biome, () => (Main.bgStyle == 9 && Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16 + Main.screenHeight / 2) || Main.ugBack == 2),
            UgCorruption = new BgmEntry(Bgm.VanillaBgmOf(10), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneCorrupt && IsUnderground()),
            Corruption   = new BgmEntry(Bgm.VanillaBgmOf( 8), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneCorrupt),
            UgCrimson    = new BgmEntry(Bgm.VanillaBgmOf(33), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneCrimson && IsUnderground()),
            Crimson      = new BgmEntry(Bgm.VanillaBgmOf(16), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneCrimson),
            Dungeon      = new BgmEntry(Bgm.VanillaBgmOf(23), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneDungeon),
            Meteor       = new BgmEntry(Bgm.VanillaBgmOf( 2), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneMeteor),
            Jungle       = new BgmEntry(Bgm.VanillaBgmOf( 7), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneJungle),
            Snow         = new BgmEntry(Bgm.VanillaBgmOf(20), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneSnow && IsUnderground()),
            Ice          = new BgmEntry(Bgm.VanillaBgmOf(14), BgmPriority.Biome, () => Main.player[Main.myPlayer].ZoneSnow),

            UgHallow    = new BgmEntry(Bgm.VanillaBgmOf(11), BgmPriority.Environment, () => IsUnderground() && Main.player[Main.myPlayer].ZoneHoly),
            UgDesert    = new BgmEntry(Bgm.VanillaBgmOf(21), BgmPriority.Environment, () => IsUnderground() && Main.sandTiles > 2200),
            Underground = new BgmEntry(new AltSupportingBgm(Bgm.VanillaBgmOf(4), Bgm.VanillaBgmOf(31), SwitchMode.Alternate), BgmPriority.Environment, IsUnderground),
            BloodMoon   = new BgmEntry(Bgm.VanillaBgmOf( 2), BgmPriority.Environment, () => Main.bloodMoon),
            Rain        = new BgmEntry(Bgm.VanillaBgmOf(19), BgmPriority.Environment, () => Main.cloudAlpha > 0f && !Main.gameMenu),
            Night       = new BgmEntry(Bgm.VanillaBgmOf( 3), BgmPriority.Environment, () => !Main.dayTime),
            Hallow      = new BgmEntry(Bgm.VanillaBgmOf( 9), BgmPriority.Environment, () => Main.player[Main.myPlayer].ZoneHoly),
            Ocean       = new BgmEntry(Bgm.VanillaBgmOf(22), BgmPriority.Environment, IsInOcean),
            Desert      = new BgmEntry(Bgm.VanillaBgmOf(21), BgmPriority.Environment, () => Main.sandTiles > 2200),
            Day         = new BgmEntry(new AltSupportingBgm(Bgm.VanillaBgmOf(1), Bgm.VanillaBgmOf(18), SwitchMode.Random), BgmPriority.Environment, () => Main.dayTime),

            Ambient = new BgmEntry(Bgm.VanillaBgmOf(28), BgmPriority.Ambient, () => Main.cloudAlpha > 0f && Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16 + Main.screenHeight / 2 && !Main.player[Main.myPlayer].ZoneSnow);

        static int MusicBoxIdToCueId(int boxId)
        {
            int ret = boxId + 1;

            // legacy stuff
            if (boxId == 4 || boxId == 5 || boxId == 9 || boxId == 11)
                ret = boxId;
            if (boxId == 8 | boxId == 10 || boxId >= 27 || boxId <= 31 || boxId == 37)
                ret = boxId + 2;

            if (boxId == 3)
                ret = 6;
            if (boxId == 32)
                ret = 38;
            if (boxId == 33)
                ret = 37;
            if (boxId == 36)
                ret = 34;

            return ret;
        }

        static bool IsBoss1Boss(NPC n)
        {
            TempNpcArray[0] = n.type;
            return Bgm.AnyNPCsForMusic(Boss1NPCs) || (n.boss && !Bgm.AnyNPCsForMusic(TempNpcArray));
        }

        static bool IsUnderground()
        {
            return Main.player[Main.myPlayer].position.Y > Main.worldSurface * 16.0 + Main.screenHeight / 2;
        }
        static bool IsInSpace()
        {
            float screenCentreYInTiles = (Main.screenPosition.Y + Main.screenHeight / 2) / 16f;

            int maxFactorSq = Main.maxTilesX / 4200;
            maxFactorSq *= maxFactorSq;

            return (screenCentreYInTiles - (65f + 10f * maxFactorSq)) / ((float)Main.worldSurface / 5f) < 1f;
        }
        static bool IsInOcean()
        {
            var px = (int)((Main.screenPosition.X + Main.screenWidth / 2) / 16f);
            return Main.screenPosition.Y / 16f < Main.worldSurface + 10.0 && (px < 380 || px > Main.maxTilesX - 380);
        }

        internal static void FillVanilla()
        {
            Bgm.VanillaDict.Add("Title", Title);

            for (int i = 0; i < MusicBoxes.Length; i++)
                Bgm.VanillaDict.Add("MusicBox" + (i + 1), MusicBoxes[i]);

            // will be in the order of definitions above, but Title is excluded (it is more important than the musicboxes)
            foreach (var fi in typeof(VanillaBgms).GetFields(BindingFlags.Public | BindingFlags.Static).Where(fi => fi.Name != "Title" && fi.FieldType == typeof(BgmEntry)))
                Bgm.VanillaDict.Add(fi.Name, (BgmEntry)fi.GetValue(null));
        }
    }
}
