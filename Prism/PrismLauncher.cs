using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.Debugging;
using Prism.Mods;
using Prism.Util;
using Steamworks;
using Terraria.Initializers;
using Terraria.Social;

using T = Terraria;

namespace Prism
{
    static class PrismLauncher
    {
        readonly static Dictionary<char, string> ShortToLongArgs = new Dictionary<char, string>
        {
            { 'D', "DEBUG" },
            { 'H', "HELP"  },
            { '?', "HELP"  }
        };

        static bool SteamExists()
        {
            if (!DllCheck.Test())
                return false;

            try
            {
                bool ret = SteamAPI.IsSteamRunning();

                Logging.LogInfo("Steam detected.");

                return ret;
            }
            catch
            {
                return false;
            }
        }

        static void ReadCommandLineArgs(Argument[] args)
        {
            for (int i = 0; i < args.Length; i++)
                switch (args[i].Name)
                {
                    case null:
                        // do something with the Value here
                        break;
                    case "HELP":
                        Console.WriteLine("No.");
                        break;
                    case "DEBUG":
                        if (args[i].Type != ArgType.KeyValuePair)
                            throw new FormatException("DEBUG argument must be a key-value pair!");

                        var v = args[i].Value;

                        if (!Directory.Exists(v))
                            throw new DirectoryNotFoundException("The directory of the mod to debug (\"" + v + "\") does not exist.");
                        if (File.Exists(v))
                            throw new DirectoryNotFoundException("The path to the mod to debug is a file, not a directory.");

                        Logging.LogInfo("Debugging mod " + Path.GetFileName(v));

                        ModLoader.DebugModDir = v;
                        break;
                }
        }

        internal static int Launch(string[] args)
        {
            try
            {
                ReadCommandLineArgs(CommandLineArgs.Parse(args, ShortToLongArgs));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("An error occured during command-line argument parsing:");
                Console.Error.WriteLine(e);

                return ExceptionHandler.GetHResult(e);
            }

            using (var m = new TMain())
            {
                SocialAPI.Initialize(SteamExists() ? SocialMode.Steam : SocialMode.None);
                LaunchInitializer.LoadParameters(m);

                PrismDebug.Init();

                try
                {
                    T.Main.OnEngineLoad += () =>
                    {
                        T.Program.StartForceLoad();

                        //TODO: move this to another thread (in StartForceLoad)
                        //T.Program.ForceLoadAssembly(typeof(T.Program).Assembly /* Terraria */, true);
#if !DEV_BUILD
                        //T.Program.ForceLoadAssembly(Assembly.GetExecutingAssembly() /* Prism */, true);
#endif
                    };

                    m.Run();
                }
                catch (Exception e)
                {
                    ExceptionHandler.HandleFatal(e, false);
                }
            }

            return 0;
        }
    }
}
