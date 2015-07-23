using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Prism.Debugging
{
    public class PrismTraceListener : TraceListener
    {
        readonly static string NAME = "Prism trace listener.";

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
            set
            {
                throw new InvalidOperationException();
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
    }
}
