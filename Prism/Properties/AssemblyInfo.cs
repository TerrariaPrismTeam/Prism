using System.Runtime.InteropServices;
using System.Reflection;
using Prism;

#if !UNIX
[assembly: AssemblyTitle("Prism")]
[assembly: AssemblyDescription("Modding API for Terraria 1.3 and beyond")]
[assembly: AssemblyInformationalVersion(AssemblyInfo.NICE_VERSION)]
#endif

static partial class AssemblyInfo
{
    public const VersionType VERSION_TYPE =
#if DEV_BUILD
        VersionType.DevBuild
#elif DEBUG
        VersionType.PreRelease
#else
        VersionType.Normal
#endif
        ;
    public const string VERSION_TYPE_STRING =
#if DEV_BUILD
        "DevBuild"
#elif DEBUG
        "PreRelease"
#else
        "Normal"
#endif
        ;

    public const string NICE_VERSION = "Prism " +
#if DEV_BUILD
        VERSION_TYPE_STRING + " [" + GitInfo.REPO + "/" + GitInfo.BRANCH + "]"
#elif DEBUG
        VERSION_TYPE_STRING + " v" + VERSION
#else
        "v" + VERSION
#endif
        ;
}
