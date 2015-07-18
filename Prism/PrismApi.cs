using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Main MainInstance
        {
            get;
            internal set;
        }
        public static string ModDirectory
        {
            get;
            internal set;
        }
    }
}
