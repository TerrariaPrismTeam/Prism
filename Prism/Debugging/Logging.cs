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
            Warning = '?',
            Error   = '!';

        readonly static string
            Fatal   = "FATAL ERROR: ",
            Warn    = "Warning: "    ,
            UTC     = "UTC "         ,

            LogFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\prism.log";

        static StreamWriter sw = null;

        internal static void Init ()
        {
            if (sw != null)
                return;

            sw = new StreamWriter(LogFile, true);

            LogInfo(PrismApi.NiceVersionString + " launched, logger started.");
        }
        internal static void Close()
        {
            if (sw == null)
                return;

            LogInfo("Logger shutting down, Prism will follow in a few milliseconds...");

            sw.Flush  ();
            sw.Dispose();
            sw = null;
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
            if (sw == null)
                return;

            var sb = new StringBuilder();

            var utc = DateTime.UtcNow;
            sb.Append('[').Append(UTC).Append(utc.ToShortDateString()).Append(' ').Append(utc.ToLongTimeString()).Append(']')
              .Append('[').Append(severity).Append(']')
              .Append(' ').Append(text);

            sw.WriteLine(sb.ToString());
            sw.Flush();
        }

        internal static void LogInfo(string message)
        {
            Log(Info, message);
        }
        internal static void LogWarning(string warning)
        {
            if (Debugger.IsAttached)
                Debug.WriteLine(Warn + warning);

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
            Log(Error, Fatal + error);
        }
        internal static void LogFatal(Exception e)
        {
            LogFatal(e.ToString());
        }
    }
}
