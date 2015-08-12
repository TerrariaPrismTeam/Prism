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
    //TODO: (?) put most of the stuff in ByObjRef and ById in DefIndexer -> less code repetition (=> less c/p errors)
    namespace Defs
    {
        public partial class ItemDef
        {
            static EntityDIH<Item, ItemBehaviour, ItemDef> helper = new EntityDIH<Item, ItemBehaviour, ItemDef>(
                ItemID.Count, "Item", md => md.ItemDefs, Handler.ItemDef.VanillaDefsByName, Handler.ItemDef.DefsByType);

            public static DefIndexer<ItemDef> Defs
            {
                get
                {
                    return new DefIndexer<ItemDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById);
                }
            }
        }
        public partial class NpcDef
        {
            static EntityDIH<NPC, NpcBehaviour, NpcDef> helper = new EntityDIH<NPC, NpcBehaviour, NpcDef>(
                NPCID.Count, "NPC", md => md.NpcDefs, Handler.NpcDef.VanillaDefsByName, Handler.NpcDef.DefsByType);

            public static DefIndexer<NpcDef> Defs
            {
                get
                {
                    return new DefIndexer<NpcDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById);
                }
            }
        }
        public partial class ProjectileDef
        {
            static EntityDIH<Projectile, ProjectileBehaviour, ProjectileDef> helper = new EntityDIH<Projectile, ProjectileBehaviour, ProjectileDef>(
                ProjectileID.Count, "Projectile", md => md.ProjectileDefs, Handler.ProjDef.VanillaDefsByName, Handler.ProjDef.DefsByType);

            public static DefIndexer<ProjectileDef> Defs
            {
                get
                {
                    return new DefIndexer<ProjectileDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById);
                }
            }
        }
        public partial class TileDef
        {
            static EntityDIH<Tile, TileBehaviour, TileDef> helper = new EntityDIH<Tile, TileBehaviour, TileDef>(
                TileID.Count, "Tile", md => md.TileDefs, Handler.TileDef.VanillaDefsByName, Handler.TileDef.DefsByType);

            public static DefIndexer<TileDef> Defs
            {
                get
                {
                    return new DefIndexer<TileDef>(helper.GetEnumerable(), helper.ByObjRef, helper.ById);
                }
            }
        }
    }
    namespace Audio
    {
        public partial class Bgm
        {
            static DIHelper<BgmEntry> helper = new DIHelper<BgmEntry>(40, "BGM entry", md => md.BgmEntries, VanillaDict, null);

            public static DefIndexer<BgmEntry> Entries
            {
                get
                {
                    var vanillaDefs = VanillaDict.Select(kvp => new KeyValuePair<ObjectRef, BgmEntry>(new ObjectRef(kvp.Key), kvp.Value));
                    var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                    return new DefIndexer<BgmEntry>(vanillaDefs.Concat(modDefs), helper.ByObjRef, ById);
                }
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
                        return VanillaBgms.Underground;
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
    }
}
