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

            //TODO: log the full exception in some way or another... later
            //TODO: move to exception UI page... later
            if (e is TargetInvocationException)
            {
                var tie = (TargetInvocationException)e;
                Trace.WriteLine(tie.InnerException.Message + " at " + tie.InnerException.TargetSite);
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
                Trace.WriteLine(e.Message + " at " + e.TargetSite);
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
