using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Prism
{
    public static class ExceptionHandler
    {
        static int GetHResult(Exception e)
        {
            return (int)typeof(Exception).GetProperty("HResult", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(e, null);
        }

        public static void Handle     (Exception e)
        {
            if (Debugger.IsAttached)
                throw new RethrownException(e); // signal to the debugger instead of displaying the error message, for convenience

            // TODO: move to exception UI page
            Trace.WriteLine(e.Message + " at " + e.TargetSite);
        }
        public static void HandleFatal(Exception e)
        {
            if (Debugger.IsAttached)
                throw new RethrownException(e); // signal to the debugger instead of displaying the error message, for convenience

            MessageBox.Show("A fatal error occured:\n" + e, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(GetHResult(e));
        }
    }
}
