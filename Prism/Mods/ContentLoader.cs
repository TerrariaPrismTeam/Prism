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
            Bgm.VanillaDicts.Clear();
            Bgm.ByMod.Clear();
        }
        internal static void Setup()
        {
            VanillaBgms.FillVanilla();
        }

        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            var ret = new List<LoaderError>();

            foreach (var kvp in mod.GetBgmsInternally())
                Bgm.ByMod.Add(mod.Info, mod.GetBgmsInternally());

            return ret;
        }
    }
}
