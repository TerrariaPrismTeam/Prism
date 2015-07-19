using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Terraria;

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
        public readonly static VersionType VersionType = VersionType.DevBuild;

        public readonly static string
            JsonManifestFileName = "manifest.json",
            DefaultDllRefsSubdirectory = "\\References";

        public static string ModDirectory
        {
            get;
            internal set;
        }
        public readonly static ModInfo VanillaInfo = new ModInfo("_", "Terraria", "Terraria", "Re-Logic", AssemblyInfo.TERRARIA_VERSION, "Vanilla terraria", String.Empty, String.Empty, new IReference[0]);
    }
}
