using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Defs;

namespace Prism.Mods
{
    static class ContentLoader
    {
        internal static void Reset()
        {
            VanillaBgms .Reset();
            Bgm.VanillaDict.Clear();

            VanillaSfxes.Reset();
            Sfx.VanillaDict.Clear();
        }
        internal static void Setup()
        {
            VanillaBgms .FillVanilla();
            VanillaSfxes.FillVanilla();
        }

        static Dictionary<string, TEntry> SetEntryModDefs<TEntry, TRef>(ModDef def, Dictionary<string, TEntry> dict)
            where TEntry : AudioEntry<TEntry, TRef>
            where TRef : EntityRef<TEntry>
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

            mod.gameBehaviour = mod.ContentHandler.CreateGameBInternally();

            mod.BgmEntries = SetEntryModDefs<BgmEntry, BgmRef>(mod, mod.ContentHandler.GetBgmsInternally ());
            mod.SfxEntries = SetEntryModDefs<SfxEntry, SfxRef>(mod, mod.ContentHandler.GetSfxesInternally());

            return ret;
        }
    }
}
