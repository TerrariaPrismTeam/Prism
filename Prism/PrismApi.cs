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
        public readonly static VersionType VersionType = VersionType.DevBuild;

        public readonly static string TerrariaVersionString = AssemblyInfo.TERRARIA_VERSION;
        public readonly static Version TerrariaVersion = new Version(AssemblyInfo.TERRARIA_VERSION);

        public readonly static string
            JsonManifestFileName       = "manifest.json",
            DefaultDllRefsSubdirectory = "\\References",
            VanillaString              = "Vanilla",
            TerrariaString             = "Terraria";

        public readonly static string PrismForumThread = @"http://forums.terraria.org/index.php?threads/thread_name.####/";

        internal readonly static string HelpErrorText = "Please report error messages you encounter to the Prism thread: " + PrismForumThread
                                                    + "\nReporting these error messages makes it easier for us to determine the cause of the issue and release a fix for it. "
                                                    +   "(just please remember to put the error message in a [spoiler] or upload it to a pastebin!)";

        public static string ModDirectory
        {
            get;
            internal set;
        }
        public readonly static ModInfo VanillaInfo = new ModInfo("_", TerrariaString, TerrariaString, "Re-Logic", TerrariaVersionString, "Vanilla terraria", String.Empty, String.Empty, new IReference[0]);
    }
}
