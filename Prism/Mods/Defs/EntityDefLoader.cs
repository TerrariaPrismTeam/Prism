using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    static class EntityDefLoader
    {
        static Dictionary<string, TEntityDef> SetROProperties<TEntityDef>(ModDef def, Dictionary<string, TEntityDef> dict)
            where TEntityDef : EntityDef
        {
            foreach (var kvp in dict)
            {
                kvp.Value.InternalName = kvp.Key;
                kvp.Value.Mod = def.Info;
            }

            return dict;
        }

        internal static void Reset()
        {
            ItemDefHandler.Reset();
        }
        internal static void Setup()
        {
            ItemDefHandler.FillVanilla();
        }
        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            mod.ItemDefs = SetROProperties(mod, mod.GetItemDefsI());

            // validate props

            ItemDefHandler.Load(mod.ItemDefs);

            return new List<LoaderError>();
        }
    }
}
