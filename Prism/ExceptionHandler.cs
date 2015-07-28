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

            //TODO: log the full exception in some way or another... later
            //TODO: move to exception UI page... later
            if (e is TargetInvocationException)
            {
                var tie = (TargetInvocationException)e;
                Trace.WriteLine(e.Message + ":\n" + tie.InnerException.Message + " at " + tie.InnerException.TargetSite);

                e = tie.InnerException;
            }
            else if (e is TypeLoadException)
            {
                var tle = (TypeLoadException)e;
                Trace.WriteLine("Could not load type " + tle.TypeName + " at " + tle.TargetSite);
            }
            else if (e is AggregateException)
            {
                var ae = (AggregateException)e;

                for (int i = 0; i < ae.InnerExceptions.Count; i++)
                    Handle(ae.InnerExceptions[i]); // prints it
            }
            else
            {
                Trace.WriteLine(e.Message + " at " + e.TargetSite);
            }

            if (DetailedExceptions)
            {
                MessageBox.Show("An exception has occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static int HandleFatal(Exception e, bool exitImmediately = true)
        {
            if (Debugger.IsAttached)
                throw new RethrownException(e); // signal to the debugger instead of displaying the error message, for convenience

            MessageBox.Show("A fatal error occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

            var hr = GetHResult(e);

            if (exitImmediately)
                Environment.Exit(hr);

            return hr;
        }
    }
}
