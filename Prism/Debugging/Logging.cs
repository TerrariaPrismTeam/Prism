using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Prism.Debugging
{
    static class Logging
    {
        const char
            Info    = 'i',
            Warning = '!',
            Error   = 'E';

        static FileStream fs = null;
        static StreamWriter sw = null;

        internal static void Init ()
        {
            fs = File.Open("prism.log", FileMode.Append);

            sw = new StreamWriter(fs);
        }
        internal static void Close()
        {
            sw.Flush();
            sw.Dispose();
            sw = null;

            fs.Dispose();
            fs = null;
        }

        static string GetExnMessage(ref Exception e)
        {
            if (e is RethrownException)
            {
                e = e.InnerException;

                return GetExnMessage(ref e);
            }
            else if (e is TargetInvocationException)
            {
                var tie = (TargetInvocationException)e;

                e = tie.InnerException;

                return e.Message + ":\n" + tie.InnerException.Message + " at " + tie.InnerException.TargetSite;
            }
            else if (e is TypeLoadException)
            {
                var tle = (TypeLoadException)e;

                return "Could not load type " + tle.TypeName + " at " + tle.TargetSite;
            }
            else if (e is AggregateException)
            {
                var ae = (AggregateException)e;

                return String.Join(Environment.NewLine, ae.InnerExceptions.Select(ie => GetExnMessage(ref ie)));
            }

            return e.Message + " at " + e.TargetSite;
        }

        static void Log(char severity, string text)
        {
            if (fs == null || sw == null)
                return;

            var sb = new StringBuilder();

            sb.Append('[').Append(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)).Append(']');
            sb.Append('[').Append(severity).Append(']');
            sb.Append(' ').Append(text);

            sw.WriteLine(sb.ToString());
            sw.Flush();
            fs.Flush();
        }

        internal static void LogInfo(string message)
        {
            Log(Info, message);
        }
        internal static void LogWarning(string warning)
        {
            Log(Warning, warning);
        }
        internal static void LogError(string error)
        {
            Log(Error, error);
        }
        internal static void LogError(Exception e)
        {
            var readable = GetExnMessage(ref e);
            var text = e.ToString();

            Trace.TraceError(readable);

            if (Console.OpenStandardError() != Stream.Null)
                Console.Error.WriteLine(text);

            Log(Error, text);

            if (Debugger.IsAttached)
                Debug.Fail(text);
        }
        internal static void LogFatal(string error)
        {
            Log(Error, "FATAL ERROR: " + error);
        }
        internal static void LogFatal(Exception e)
        {
            LogFatal(e.ToString());
        }
    }
}
