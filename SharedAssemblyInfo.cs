using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Terraria Prism Team")]
[assembly: AssemblyProduct("Prism")]
[assembly: AssemblyCopyright("Copyright © Terraria Prism Team 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid(AssemblyInfo.GUID)]
#if DEV_BUILD
[assembly: AssemblyVersion(AssemblyInfo.DEV_BUILD_VERSION)]
#else
[assembly: AssemblyVersion(AssemblyInfo.VERSION)]
#endif
[assembly: AssemblyFileVersion(AssemblyInfo.VERSION)]

static partial class AssemblyInfo
{
    public const string GUID = "fa53dc38-9b2b-45d3-818e-3e60f69143f6";
    public const string VERSION = "1.0.0.0";
    public const string MIN_TERRARIA_VERSION = "1.3.2.0";
    public const string MAX_TERRARIA_VERSION = "1.3.2.0";
    public const string DISP_TERRARIA_VERSION = "1.3.2";

    internal const string DEV_BUILD_VERSION = "0.0.0.0";
}
static partial class GitInfo
{
    public const string REPO = "Prism";
    public const string BRANCH = "develop";
}
