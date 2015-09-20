using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Util;

namespace Prism
{
    public enum VersionType
    {
        Normal,
        PreRelease,
        DevBuild
    }

    public static class PrismApi
    {
        public readonly static Version Version = new Version(AssemblyInfo.VERSION);
        public readonly static VersionType VersionType = AssemblyInfo.VERSION_TYPE;

        public readonly static string NiceVersionString = AssemblyInfo.NICE_VERSION;

        public readonly static string TerrariaVersionString = AssemblyInfo.TERRARIA_VERSION;
        public readonly static Version TerrariaVersion = new Version(AssemblyInfo.TERRARIA_VERSION);

        public readonly static string
            JsonManifestFileName       = "manifest.json" ,
            DefaultDllRefsSubdirectory = "\\References"  ,
            VanillaString              = "Vanilla"       ,
            TerrariaString             = "Terraria"      ,
            PrismString                = "Prism"         ,
            PrismTerrariaString        = "Prism.Terraria";

        public readonly static ModInfo VanillaInfo = new ModInfo("_", TerrariaString, TerrariaString, "Re-Logic", TerrariaVersionString, "Vanilla terraria", PrismTerrariaString, "Terraria.Main", Empty<IReference>.Array);

        public static string ModDirectory
        {
            get;
            internal set;
        }
    }
}
