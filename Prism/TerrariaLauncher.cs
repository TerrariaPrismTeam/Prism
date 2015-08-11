using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Debugging;
using Steamworks;
using Terraria.Social;

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
