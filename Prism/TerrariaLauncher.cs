using System;
using System.Collections.Generic;
using System.Linq;
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
                return SteamAPI.IsSteamRunning();
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
