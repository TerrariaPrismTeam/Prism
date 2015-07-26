using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Prism
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Set to true in order to get full stack traces in a message box
        /// </summary>
        public static bool DetailedExceptions = false;

        static int GetHResult(Exception e)
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

            if (e.GetType() == typeof(TargetInvocationException))
            {
                TargetInvocationException tie = (TargetInvocationException)e;
                Trace.WriteLine(e.Message + ":\n" + tie.InnerException.Message + " at " + tie.InnerException.TargetSite);

                e = tie.InnerException;
            }
            else
            {
                Trace.WriteLine(e.Message + " at " + e.TargetSite);
            }

            if(DetailedExceptions)
            {
                MessageBox.Show("An exception has occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void HandleFatal(Exception e, bool exitImmediately = true)
        {
            if (Debugger.IsAttached)
                throw new RethrownException(e); // signal to the debugger instead of displaying the error message, for convenience

            MessageBox.Show("A fatal error occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (exitImmediately)
                Environment.Exit(GetHResult(e));
        }
    }
}
