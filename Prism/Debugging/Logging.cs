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
    public static class Logging
    {
        const char
            Info    = 'i',
            Warning = '?',
            Error   = '!';

        readonly static string
            Fatal   = "FATAL ERROR: ",
            Warn    = "Warning: "    ,

            LogFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/prism.log";

        static StreamWriter sw = null;

        internal static void Init ()
        {
            if (sw != null)
                return;

            if (File.Exists(LogFile))
            {
                var fi = new FileInfo(LogFile);
                if (fi.Length > 0x400 * 0x400)
                    File.Delete(LogFile);
            }

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
            sb.Append(String.Format("[UTC {0:D2}:{1:D2}:{2:D2}][{3}] ",
                        utc.Hour, utc.Minute, utc.Second, severity)).Append(text);

            string s = sb.ToString();
            sw.WriteLine(s);
            sw.Flush();

#if DEV_BUILD || DEBUG
            if (Console.OpenStandardError() != Stream.Null)
            {
                Console.Error.WriteLine(s);
                Console.Error.Flush();
            }
#endif
        }

        public static void LogInfo(string message)
        {
            Log(Info, message);
        }
        public static void LogWarning(string warning)
        {
            if (Debugger.IsAttached)
                Debug.WriteLine(Warn + warning);

            Log(Warning, warning);
        }
        public static void LogError(string error)
        {
            Log(Error, error);
        }
        public static void LogError(Exception e)
        {
            var readable = GetExnMessage(ref e);
            var text = e.ToString();

            PrismTraceListener.RanLogger = true ;
            Trace.Fail(readable, text);
            PrismTraceListener.RanLogger = false;

            Log(Error, text);

            if (Debugger.IsAttached)
                Debug.Fail(text);
        }
        internal static void LogFatal(string error)
        {
            Log(Error, Fatal + error);

            if (Debugger.IsAttached)
                Debug.Fail(error);
        }
        internal static void LogFatal(Exception e)
        {
            LogFatal(e.ToString());
        }
    }
}

