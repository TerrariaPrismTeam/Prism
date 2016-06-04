using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.API
{
    namespace Defs
    {
        public partial class BuffDef
        {
            static readonly DIHelper<BuffDef> helper = new DIHelper<BuffDef>(BuffID.Count, "Buff", md => md.BuffDefs, Handler.BuffDef.VanillaDefsByName, Handler.BuffDef.DefsByType);

            public static DefIndexer<BuffDef> Defs
            {
                get;
                private set;
            }

            static BuffDef()
            {
                var vanillaDefs = Handler.BuffDef.VanillaDefsByName.Select(kvp => new KeyValuePair<ObjectRef, BuffDef>(new ObjectRef(kvp.Key), kvp.Value));
                var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                Defs = new DefIndexer<BuffDef>(vanillaDefs.Concat(modDefs), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }

            static IEnumerable<KeyValuePair<ObjectRef, BuffDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.BuffDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, BuffDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }
        }
        public partial class ItemDef
        {
            static readonly EntityDIH<Item, ItemBehaviour, ItemDef> helper = new EntityDIH<Item, ItemBehaviour, ItemDef>(
                ItemID.Count, "Item", md => md.ItemDefs, Handler.ItemDef.VanillaDefsByName, Handler.ItemDef.DefsByType);

            public static DefIndexer<ItemDef> Defs
            {
                get;
                private set;
            }

            static ItemDef()
            {
                Defs = new DefIndexer<ItemDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }
        }
        public partial class MountDef
        {
            static readonly DIHelper<MountDef> helper = new DIHelper<MountDef>(MountID.Count, "Mount", md => md.MountDefs, Handler.MountDef.VanillaDefsByName, Handler.MountDef.DefsByType);

            public static DefIndexer<MountDef> Defs
            {
                get;
                private set;
            }

            static MountDef()
            {
                var vanillaDefs = Handler.MountDef.VanillaDefsByName.Select(kvp => new KeyValuePair<ObjectRef, MountDef>(new ObjectRef(kvp.Key), kvp.Value));
                var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                Defs = new DefIndexer<MountDef>(vanillaDefs.Concat(modDefs), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }

            static IEnumerable<KeyValuePair<ObjectRef, MountDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.MountDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, MountDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }
        }
        public partial class NpcDef
        {
            static readonly EntityDIH<NPC, NpcBehaviour, NpcDef> helper = new EntityDIH<NPC, NpcBehaviour, NpcDef>(
                NPCID.Count, "NPC", md => md.NpcDefs, Handler.NpcDef.VanillaDefsByName, Handler.NpcDef.DefsByType);

            public static DefIndexer<NpcDef> Defs
            {
                get;
                private set;
            }

            static NpcDef()
            {
                Defs = new DefIndexer<NpcDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }
        }
        public partial class ProjectileDef
        {
            static readonly EntityDIH<Projectile, ProjectileBehaviour, ProjectileDef> helper = new EntityDIH<Projectile, ProjectileBehaviour, ProjectileDef>(
                ProjectileID.Count, "Projectile", md => md.ProjectileDefs, Handler.ProjDef.VanillaDefsByName, Handler.ProjDef.DefsByType);

            public static DefIndexer<ProjectileDef> Defs
            {
                get;
                private set;
            }

            static ProjectileDef()
            {
                Defs = new DefIndexer<ProjectileDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }
        }
        public partial class TileDef
        {
            static readonly EntityDIH<Tile, TileBehaviour, TileDef> helper = new EntityDIH<Tile, TileBehaviour, TileDef>(
                TileID.Count, "Tile", md => md.TileDefs, Handler.TileDef.VanillaDefsByName, Handler.TileDef.DefsByType);

            public static DefIndexer<TileDef> Defs
            {
                get;
                private set;
            }

            static TileDef()
            {
                Defs = new DefIndexer<TileDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }
        }
        public partial class WallDef
        {
            static readonly DIHelper<WallDef> helper = new DIHelper<WallDef>(WallID.Count, "Wall", md => md.WallDefs, Handler.WallDef.VanillaDefsByName, Handler.WallDef.DefsByType);

            public static DefIndexer<WallDef> Defs
            {
                get;
                private set;
            }

            static WallDef()
            {
                var vanillaDefs = Handler.WallDef.VanillaDefsByName.Select(kvp => new KeyValuePair<ObjectRef, WallDef>(new ObjectRef(kvp.Key), kvp.Value));
                var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                Defs = new DefIndexer<WallDef>(vanillaDefs.Concat(modDefs), helper.ByObjRef, helper.ById, helper.ByIdUnsafe);
            }

            static IEnumerable<KeyValuePair<ObjectRef, WallDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.WallDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, WallDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }
        }
    }
    namespace Audio
    {
        public partial class Bgm
        {
            static readonly DIHelper<BgmEntry> helper = new DIHelper<BgmEntry>(40, "BGM entry", md => md.BgmEntries, VanillaDict, null);

            public static DefIndexer<BgmEntry> Entries
            {
                get;
                private set;
            }

            static Bgm()
            {
                var vanillaDefs = VanillaDict.Select(kvp => new KeyValuePair<ObjectRef, BgmEntry>(new ObjectRef(kvp.Key), kvp.Value));
                var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                Entries = new DefIndexer<BgmEntry>(vanillaDefs.Concat(modDefs), helper.ByObjRef, ById, ById);
            }

            static IEnumerable<KeyValuePair<ObjectRef, BgmEntry>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.BgmEntries.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, BgmEntry>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static BgmEntry ById(int id)
            {
                switch (id)
                {
                    case 1:
                        return VanillaBgms.Day;
                    case 2:
                        return VanillaBgms.BloodMoon;
                    case 3:
                        return VanillaBgms.Night;
                    case 4:
                        return VanillaBgms.Underground;
                    case 5:
                        return VanillaBgms.Boss1;
                    case 6:
                        return VanillaBgms.Title;
                    case 7:
                        return VanillaBgms.Jungle;
                    case 8:
                        return VanillaBgms.Corruption;

                    case 9:
                        return VanillaBgms.Hallow;
                    case 10:
                        return VanillaBgms.UgCorruption;
                    case 11:
                        return VanillaBgms.UgHallow;
                    case 12:
                        return VanillaBgms.Boss2;
                    case 13:
                        return VanillaBgms.Boss3;

                    case 14:
                        return VanillaBgms.Ice;
                    case 15:
                        return VanillaBgms.Space;
                    case 16:
                        return VanillaBgms.Crimson;
                    case 17:
                        return VanillaBgms.Golem;
                    case 18:
                        return VanillaBgms.Day;
                    case 19:
                        return VanillaBgms.Rain;
                    case 20:
                        return VanillaBgms.Snow;
                    case 21:
                        return VanillaBgms.Desert;
                    case 22:
                        return VanillaBgms.Ocean;
                    case 23:
                        return VanillaBgms.Dungeon;
                    case 24:
                        return VanillaBgms.Plantera;
                    case 25:
                        return VanillaBgms.QueenBee;
                    case 26:
                        return VanillaBgms.Lihzahrd;
                    case 27:
                        return VanillaBgms.Eclipse;
                    case 28:
                        return VanillaBgms.Ambient;
                    case 29:
                        return VanillaBgms.Mushrooms;
                    case 30:
                        return VanillaBgms.PumpkinMoon;
                    case 31:
                        return VanillaBgms.Underground;
                    case 32:
                        return VanillaBgms.FrostMoon;

                    case 33:
                        return VanillaBgms.UgCrimson;
                    case 34:
                        return VanillaBgms.LunarPillar;
                    case 35:
                        return VanillaBgms.Pirates;
                    case 36:
                        return VanillaBgms.Underworld;
                    case 37:
                        return VanillaBgms.MartianMadness;
                    case 38:
                        return VanillaBgms.MoonLord;
                    case 39:
                        return VanillaBgms.GoblinArmy;
                }

                throw new ArgumentOutOfRangeException("id", "The id must be a vanilla BGM ID.");
            }
        }
        public partial class Sfx
        {
            static readonly DIHelper<SfxEntry> helper = new DIHelper<SfxEntry>(40, "SFX entry", md => md.SfxEntries, VanillaDict, null);

            public static DefIndexer<SfxEntry> Entries
            {
                get;
                private set;
            }

            static Sfx()
            {
                var vanillaDefs = VanillaDict.Select(kvp => new KeyValuePair<ObjectRef, SfxEntry>(new ObjectRef(kvp.Key), kvp.Value));
                var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                Entries = new DefIndexer<SfxEntry>(vanillaDefs.Concat(modDefs), helper.ByObjRef, ById, ById);
            }

            static IEnumerable<KeyValuePair<ObjectRef, SfxEntry>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.SfxEntries.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, SfxEntry>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static SfxEntry ById(int id)
            {
                switch (id)
                {
                    case 0:
                        return VanillaSfxes.DigBlock;
                    case 1:
                        return VanillaSfxes.PlayerHit;
                    case 2:
                        return VanillaSfxes.UseItem;
                    case 3:
                        return VanillaSfxes.NpcHit;
                    case 4:
                        return VanillaSfxes.NpcKilled;
                    case 5:
                        return VanillaSfxes.PlayerKilled;
                    case 6:
                        return VanillaSfxes.CutGrass;
                    case 7:
                        return VanillaSfxes.GrabItem;
                    case 8:
                        return VanillaSfxes.DoorOpen;
                    case 9:
                        return VanillaSfxes.DoorClose;
                    case 10:
                        return VanillaSfxes.MenuOpen;
                    case 11:
                        return VanillaSfxes.MenuClose;
                    case 12:
                        return VanillaSfxes.MenuTick;
                    case 13:
                        return VanillaSfxes.Shatter;
                    case 14:
                        return VanillaSfxes.ZombieIdle;
                    case 15:
                        return VanillaSfxes.NpcAttackSound;
                    case 16:
                        return VanillaSfxes.DoubleJump;
                    case 17:
                        return VanillaSfxes.Run;
                    case 18:
                        return VanillaSfxes.Buy;
                    case 19:
                        return VanillaSfxes.Splash;
                    case 20:
                        return VanillaSfxes.FemaleHit;
                    case 21:
                        return VanillaSfxes.DigOre;
                    case 22:
                        return VanillaSfxes.Unlock;
                    case 23:
                        return VanillaSfxes.Drown;
                    case 24:
                        return VanillaSfxes.Chat;
                    case 25:
                        return VanillaSfxes.MaxMana;
                    case 26:
                        return VanillaSfxes.MummyIdle;
                    case 27:
                        return VanillaSfxes.PixieIdle;
                    case 28:
                        return VanillaSfxes.MechBuzz;
                    case 29:
                        return VanillaSfxes.NpcIdleSound;
                    case 30:
                        return VanillaSfxes.DuckIdle;
                    case 31:
                        return VanillaSfxes.FrogIdle;
                    case 32:
                        return VanillaSfxes.NpcIdleSoundQuiet;
                    case 33:
                        return VanillaSfxes.BeetleIdle;
                    case 34:
                        return VanillaSfxes.AmbientWater;
                    case 35:
                        return VanillaSfxes.AmbientLava;
                    case 36:
                        return VanillaSfxes.NpcAttackSoundExpert;
                    case 37:
                        return VanillaSfxes.Meowmere;
                    case 38:
                        return VanillaSfxes.CoinPickup;
                    case 39:
                        return VanillaSfxes.AmbientDrip;
                    case 40:
                        return VanillaSfxes.Camera;
                    case 41:
                        return VanillaSfxes.MoonLordCry;
                }

                throw new ArgumentOutOfRangeException("id", "The id must be a vanilla SFX ID.");
            }
        }
    }
}
