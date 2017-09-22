using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Prism.Debugging;

namespace Prism
{
    static class Program
    {
        static void Init()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                Environment.SetEnvironmentVariable("FNA_WORKAROUND_WINDOW_RESIZABLE", "1");

            AssemblyResolver.Init();

#if !WINDOWS
            FNALoggerEXT.LogInfo  = msg => Logging.LogInfo   (msg);
            FNALoggerEXT.LogWarn  = msg => Logging.LogWarning(msg);
            FNALoggerEXT.LogError = msg => Logging.LogError  (msg);
#endif

            Logging.Init();
        }

        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                Init();

                // having it in this class would cause a TypeInitializationException before Main is called.
                // this happens because all types in Prism.Terraria.dll still refer to Terraria.exe, but this is
                // fixed in AssemblyResolver by resolving Terraria.exe as Prism.Terraria.dll.
                // (and it also helps resolving some other assemblies, including mod references)
                return PrismLauncher.Launch(args);
            }
            catch (Exception e)
            {
                return ExceptionHandler.HandleFatal(e, false /* Environment.Exit is not needed here */);
            }
            finally
            {
                Logging.Close();
            }
        }
    }
}
