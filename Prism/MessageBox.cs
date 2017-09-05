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
            if (Environment.OSVersion.Platform == PlatformID.Xbox)
                return; // wtf?

            try
            {
                WfMsgBox.Show(message, "Prism: Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch // shouldn't throw on Windows
            {
                if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    bool tryConsole = true;

                    /*try
                    {
                        if (Process.Start("xmessage \"" + message.Replace('"', '\'') + "\"") == null) // most distros have this
                            tryConsole = true;
                    }
                    catch
                    {
                        tryConsole = true;
                    }*/

                    if (tryConsole)
                        try
                        {
                            Console.WriteLine(message);
                        }
                        catch (IOException) { } // well fuck
                }
            }
        }
    }
}
