using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Prism.Debugging;

namespace Prism
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Set to true in order to get full stack traces in a message box
        /// </summary>
        public static bool DetailedExceptions = Debugger.IsAttached;

        internal static int GetHResult(Exception e)
        {
            try
            {
                return (int)typeof(Exception).GetProperty("HResult", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(e, null);
            }
            catch
            {
                return 1; // fuckfuckfuckfuckfuckfuck
                //RIP D:
            }
        }

        public static void Handle     (Exception e)
        {
            if (Debugger.IsAttached)
                throw new RethrownException(e); // signal to the debugger instead of displaying the error message, for convenience

            //TODO: move to exception UI page... later
            Logging.LogError(e);

            if (DetailedExceptions)
                MessageBox.Show("An exception has occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static int  HandleFatal(Exception e, bool exitImmediately = true)
        {
            if (Debugger.IsAttached)
                throw new RethrownException(e); // signal to the debugger instead of displaying the error message, for convenience

            Logging.LogFatal(e);

            MessageBox.Show("A fatal error occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

            var hr = GetHResult(e);

            if (exitImmediately)
                Environment.Exit(hr);

            return hr;
        }
    }
}
