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
        public partial class ItemDef
        {
            public static DefIndexer<ItemDef> Defs
            {
                get
                {
                    var vanillaDefs = Handler.ItemDef.DefsByType.Values.Select(id => new KeyValuePair<ObjectRef, ItemDef>(new ObjectRef(id.InternalName), id));
                    var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                    return new DefIndexer<ItemDef>(vanillaDefs.Concat(modDefs), ByObjRef, ById);
                }
            }

            static IEnumerable<KeyValuePair<ObjectRef, ItemDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.ItemDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, ItemDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static ItemDef ByObjRef(ObjectRef or)
            {
                if (or.Mod == PrismApi.VanillaInfo)
                {
                    if (!Handler.ItemDef.VanillaDefsByName.ContainsKey(or.Name))
                        throw new InvalidOperationException("Vanilla item definition '" + or.Name + "' is not found.");

                    return Handler.ItemDef.VanillaDefsByName[or.Name];
                }

                if (!ModData.ModsFromInternalName.ContainsKey(or.ModName))
                    throw new InvalidOperationException("Item definition '" + or.Name + "' in mod '" + or.ModName + "' could not be returned because the mod is not loaded.");
                if (!ModData.ModsFromInternalName[or.ModName].ItemDefs.ContainsKey(or.Name))
                    throw new InvalidOperationException("Item definition '" + or.Name + "' in mod '" + or.ModName + "' could not be resolved because the item is not loaded.");

                return ModData.ModsFromInternalName[or.ModName].ItemDefs[or.Name];
            }
            static ItemDef ById(int id)
            {
                if (id >= ItemID.Count)
                    throw new ArgumentOutOfRangeException("id", "The id must be a vanilla item type or netID.");

                return Handler.ItemDef.DefsByType[id];
            }
        }
        public partial class NpcDef
        {
            public static DefIndexer<NpcDef> Defs
            {
                get
                {
                    var vanillaDefs = Handler.NpcDef.DefsByType.Values.Select(nd => new KeyValuePair<ObjectRef, NpcDef>(new ObjectRef(nd.InternalName), nd));
                    var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                    return new DefIndexer<NpcDef>(vanillaDefs.Concat(modDefs), ByObjRef, ById);
                }
            }

            static IEnumerable<KeyValuePair<ObjectRef, NpcDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.NpcDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, NpcDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static NpcDef ByObjRef(ObjectRef or)
            {
                if (or.Mod == PrismApi.VanillaInfo)
                {
                    if (!Handler.NpcDef.VanillaDefsByName.ContainsKey(or.Name))
                        throw new InvalidOperationException("Vanilla NPC definition '" + or.Name + "' is not found.");

                    return Handler.NpcDef.VanillaDefsByName[or.Name];
                }

                if (!ModData.ModsFromInternalName.ContainsKey(or.ModName))
                    throw new InvalidOperationException("NPC definition '" + or.Name + "' in mod '" + or.ModName + "' could not be returned because the mod is not loaded.");
                if (!ModData.ModsFromInternalName[or.ModName].NpcDefs.ContainsKey(or.Name))
                    throw new InvalidOperationException("NPC definition '" + or.Name + "' in mod '" + or.ModName + "' could not be resolved because the NPC is not loaded.");

                return ModData.ModsFromInternalName[or.ModName].NpcDefs[or.Name];
            }
            static NpcDef ById(int id)
            {
                if (id >= NPCID.Count)
                    throw new ArgumentOutOfRangeException("id", "The id must be a vanilla NPC type or netID.");

                return Handler.NpcDef.DefsByType[id];
            }
        }
        public partial class ProjectileDef
        {
            public static DefIndexer<ProjectileDef> Defs
            {
                get
                {
                    var vanillaDefs = Handler.ProjDef.DefsByType.Values.Select(pd => new KeyValuePair<ObjectRef, ProjectileDef>(new ObjectRef(pd.InternalName), pd));
                    var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                    return new DefIndexer<ProjectileDef>(vanillaDefs.Concat(modDefs), ByObjRef, ById);
                }
            }

            static IEnumerable<KeyValuePair<ObjectRef, ProjectileDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.ProjectileDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, ProjectileDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static ProjectileDef ByObjRef(ObjectRef or)
            {
                if (or.Mod == PrismApi.VanillaInfo)
                {
                    if (!Handler.ProjDef.VanillaDefsByName.ContainsKey(or.Name))
                        throw new InvalidOperationException("Vanilla projectile definition '" + or.Name + "' is not found.");

                    return Handler.ProjDef.VanillaDefsByName[or.Name];
                }

                if (!ModData.ModsFromInternalName.ContainsKey(or.ModName))
                    throw new InvalidOperationException("Projectile definition '" + or.Name + "' in mod '" + or.ModName + "' could not be returned because the mod is not loaded.");
                if (!ModData.ModsFromInternalName[or.ModName].ProjectileDefs.ContainsKey(or.Name))
                    throw new InvalidOperationException("Projectile definition '" + or.Name + "' in mod '" + or.ModName + "' could not be resolved because the projectile is not loaded.");

                return ModData.ModsFromInternalName[or.ModName].ProjectileDefs[or.Name];
            }
            static ProjectileDef ById(int id)
            {
                if (id >= ProjectileID.Count || id < 0)
                    throw new ArgumentOutOfRangeException("id", "The id must be a vanilla projectile type.");

                return Handler.ProjDef.DefsByType[id];
            }
        }
        public partial class TileDef
        {
            public static DefIndexer<TileDef> Defs
            {
                get
                {
                    var vanillaDefs = Handler.TileDef.DefsByType.Values.Select(td => new KeyValuePair<ObjectRef, TileDef>(new ObjectRef(td.InternalName), td));
                    var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                    return new DefIndexer<TileDef>(vanillaDefs.Concat(modDefs), ByObjRef, ById);
                }
            }

            static IEnumerable<KeyValuePair<ObjectRef, TileDef>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.TileDefs.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, TileDef>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static TileDef ByObjRef(ObjectRef or)
            {
                if (or.Mod == PrismApi.VanillaInfo)
                {
                    if (!Handler.TileDef.VanillaDefsByName.ContainsKey(or.Name))
                        throw new InvalidOperationException("Vanilla tile definition '" + or.Name + "' is not found.");

                    return Handler.TileDef.VanillaDefsByName[or.Name];
                }

                if (!ModData.ModsFromInternalName.ContainsKey(or.ModName))
                    throw new InvalidOperationException("Tile definition '" + or.Name + "' in mod '" + or.ModName + "' could not be returned because the mod is not loaded.");
                if (!ModData.ModsFromInternalName[or.ModName].TileDefs.ContainsKey(or.Name))
                    throw new InvalidOperationException("Tile definition '" + or.Name + "' in mod '" + or.ModName + "' could not be resolved because the tile is not loaded.");

                return ModData.ModsFromInternalName[or.ModName].TileDefs[or.Name];
            }
            static TileDef ById(int id)
            {
                if (id >= TileID.Count || id < 0)
                    throw new ArgumentOutOfRangeException("id", "The id must be a vanilla tile type.");

                return Handler.TileDef.DefsByType[id];
            }
        }
    }
    namespace Audio
    {
        public partial class Bgm
        {
            public static DefIndexer<BgmEntry> Entries
            {
                get
                {
                    // welcome to VERY GODDAMN VERBOSE functional programming
                    // seriously, type inferrence FTW
                    var vanillaDefs = VanillaDict.Select(kvp => new KeyValuePair<ObjectRef, BgmEntry>(new ObjectRef(kvp.Key), kvp.Value));
                    var modDefs = ModData.Mods.Select(kvp => GetModDefs(kvp)).Flatten();

                    return new DefIndexer<BgmEntry>(vanillaDefs.Concat(modDefs), ByObjRef, ById);
                }
            }

            static IEnumerable<KeyValuePair<ObjectRef, BgmEntry>> GetModDefs(KeyValuePair<ModInfo, ModDef> kvp)
            {
                return kvp.Value.BgmEntries.SafeSelect(kvp_ => new KeyValuePair<ObjectRef, BgmEntry>(new ObjectRef(kvp_.Key, kvp.Key), kvp_.Value));
            }

            static BgmEntry ByObjRef(ObjectRef or)
            {
                if (or.Mod == PrismApi.VanillaInfo)
                {
                    if (!VanillaDict.ContainsKey(or.Name))
                        throw new InvalidOperationException("Vanilla BGM entry '" + or.Name + "' is not found.");

                    return VanillaDict[or.Name];
                }

                if (!ModData.ModsFromInternalName.ContainsKey(or.ModName))
                    throw new InvalidOperationException("BGM entry '" + or.Name + "' in mod '" + or.ModName + "' could not be returned because the mod is not loaded.");
                if (!ModData.ModsFromInternalName[or.ModName].BgmEntries.ContainsKey(or.Name))
                    throw new InvalidOperationException("BGM entry '" + or.Name + "' in mod '" + or.ModName + "' could not be resolved because the BGM entry is not loaded.");

                return ModData.ModsFromInternalName[or.ModName].BgmEntries[or.Name];
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
