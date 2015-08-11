using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Prism.Debugging
{
    public sealed class PrismTraceListener : TraceListener
    {
        readonly static string NAME = "Prism trace listener.";

        internal static bool RanLogger = false;

        public override bool IsThreadSafe
        {
            get
            {
                return true; // I hope
            }
        }
        public override string Name
        {
            get
            {
                return NAME;
            }
        }

        public override void Write(string message, string category)
        {
            PrismDebug.AddLine(new TraceLine(IndentLevel * IndentSize, message, category));
        }
        public override void Write(string message)
        {
            PrismDebug.AddLine(new TraceLine(IndentLevel * IndentSize, message));
        }

        public override void WriteLine(string message, string category)
        {
            Write(message + Environment.NewLine, category);
        }
        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }

        public override void Fail(string message)
        {
            PrismDebug.AddLine(new TraceLine(IndentLevel * IndentSize , message + Environment.NewLine, "!", 600));
        }
        public override void Fail(string message, string detailMessage)
        {
            if (RanLogger)
                return;

            RanLogger = true ;
            Logging.LogError(detailMessage);
            RanLogger = false;

            Fail(message);
        }
    }
}
