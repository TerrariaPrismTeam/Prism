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
#if DEV_BUILD
        public readonly static VersionType VersionType = VersionType.DevBuild;
#elif DEBUG
        public readonly static VersionType VersionType = VersionType.PreRelease;
#else
        public readonly static VersionType VersionType = VersionType.Normal;
#endif

        public static string NiceVersionString
        {
            get
            {
                switch (VersionType)
                {
                    case VersionType.DevBuild:
                        return "Prism DevBuild [" + GitInfo.REPO + "/" + GitInfo.BRANCH + "]";
                    case VersionType.PreRelease:
                        return "Prism PreRelease v" + Version.ToString();
                    case VersionType.Normal:
                        return "Prism v" + Version.ToString();
                    default:
                        return "Prism";
                }
            }
        }

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
        public readonly static ModInfo VanillaInfo = new ModInfo("_", TerrariaString, TerrariaString, "Re-Logic", TerrariaVersionString, "Vanilla terraria", String.Empty, String.Empty, Empty<IReference>.Array);
    }
}
