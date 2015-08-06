using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Terraria Prism Team")]
[assembly: AssemblyProduct("Prism")]
[assembly: AssemblyCopyright("Copyright © Terraria Prism Team 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid(AssemblyInfo.GUID)]
#if DEV_BUILD
[assembly: AssemblyVersion(AssemblyInfo.DEV_BUILD_VERSION)]
[assembly: AssemblyFileVersion(AssemblyInfo.DEV_BUILD_VERSION)]
#else
[assembly: AssemblyVersion(AssemblyInfo.VERSION)]
[assembly: AssemblyFileVersion(AssemblyInfo.VERSION)]
#endif

static class AssemblyInfo
{
    public const string GUID = "fa53dc38-9b2b-45d3-818e-3e60f69143f6";
    public const string VERSION = "1.0.0.0";
    public const string TERRARIA_VERSION = "1.3.0.7";
    internal const string DEV_BUILD_VERSION = "65534.65534.65534.65534"; //Max version value possible lol.
}

static class GitInfo
{
    public const string REPO = "Prism";
    public const string BRANCH = "master";
}
