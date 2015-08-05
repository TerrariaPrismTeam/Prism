using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;

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
#if DEV_BUILD
        public readonly static VersionType VersionType = VersionType.DevBuild;
#else
#if DEBUG
        public readonly static VersionType VersionType = VersionType.PreRelease;
#else
        public readonly static VersionType VersionType = VersionType.Normal;
#endif
#endif

        public readonly static string TerrariaVersionString = AssemblyInfo.TERRARIA_VERSION;
        public readonly static Version TerrariaVersion = new Version(AssemblyInfo.TERRARIA_VERSION);

        public readonly static string
            JsonManifestFileName       = "manifest.json",
            DefaultDllRefsSubdirectory = "\\References",
            VanillaString              = "Vanilla",
            TerrariaString             = "Terraria";

        public static string ModDirectory
        {
            get;
            internal set;
        }
        public readonly static ModInfo VanillaInfo = new ModInfo("_", TerrariaString, TerrariaString, "Re-Logic", TerrariaVersionString, "Vanilla terraria", String.Empty, String.Empty, new IReference[0]);
    }
}
