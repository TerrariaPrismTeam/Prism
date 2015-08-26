using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WfMsgBox = System.Windows.Forms.MessageBox;

namespace Prism
{
    public static class MessageBox
    {
        public static void ShowError(string message)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:

                case PlatformID.MacOSX:
                    try
                    {
                        WfMsgBox.Show(message, "Prism: Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch
                    {
                        if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                            goto TRY_XMESSAGE;
                    }
                    break;
                case PlatformID.Unix:
                TRY_XMESSAGE:
                    bool tryConsole = false;

                    try
                    {
                        if (Process.Start("xmessage \"" + message.Replace('"', '\'') + "\"") == null)
                            tryConsole = true;
                    }
                    catch
                    {
                        tryConsole = true;
                    }

                    if (tryConsole)
                        try
                        {
                            Console.WriteLine(message);
                        }
                        catch (IOException) { }

                    break;
            }
        }
    }
}
