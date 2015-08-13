using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Debugging;
using Steamworks;
using Terraria.Social;

using TProgram = Terraria.Program;

namespace Prism
{
    static class TerrariaLauncher
    {
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

        internal static void Launch()
        {
            using (var m = new TMain())
            {
                SocialAPI.Initialize(SteamExists() ? SocialMode.Steam : SocialMode.None);
                PrismDebug.Init();

                try
                {
                    TMain.OnEngineLoad += () =>
                    {
                        TProgram.ForceLoadAssembly(typeof(TProgram).Assembly /* Terraria */, true);
#if !DEV_BUILD
                        TProgram.ForceLoadAssembly(Assembly.GetExecutingAssembly() /* Prism */, true);
#endif
                    };

                    m.Run();
                }
                catch (Exception e)
                {
                    ExceptionHandler.HandleFatal(e, false);
                }
            }
        }
    }
}
