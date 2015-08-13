using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Audio;

namespace Prism.Mods
{
    static class ContentLoader
    {
        internal static void Reset()
        {
            Bgm.VanillaDict.Clear();
        }
        internal static void Setup()
        {
            VanillaBgms.FillVanilla();
        }

        static Dictionary<string, BgmEntry> SetEntryModDefs(ModDef def, Dictionary<string, BgmEntry> dict)
        {
            foreach (var kvp in dict)
            {
                kvp.Value.InternalName = kvp.Key;
                kvp.Value.Mod = def.Info;
            }

            return dict;
        }

        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            var ret = new List<LoaderError>();

            mod.gameBehaviour = mod.contentHandler.CreateGameBInternally();

            mod.BgmEntries = SetEntryModDefs(mod, mod.contentHandler.GetBgmsInternally());

            return ret;
        }
    }
}
