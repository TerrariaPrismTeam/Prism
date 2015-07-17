using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Prism.Debugging
{
    struct TraceLine
    {
        public string Text;
        public int Timeleft;

        public TraceLine(string text, int timeLeft)
        {
            Text = text;
            Timeleft = timeLeft;
        }
        public TraceLine(int indent, string message, string category = null, int timeLeft = 0)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < indent; i++)
                sb.Append(' ');

            if (!String.IsNullOrEmpty(category))
                sb.Append('[').Append(category).Append(']').Append(':').Append(' ');

            int firstNewline = message.IndexOfAny(new[] { '\n', '\r' });
            sb.Append(firstNewline == -1 ? message : message.Remove(firstNewline));

            Text = sb.ToString();
            Timeleft = timeLeft <= 0 ? 180 : timeLeft;
        }
    }

    static class PrismDebug
    {
        const int MAX_LINES = 10;

        static object @lock = new object();
        static PrismTraceListener listener;
        internal static List<TraceLine> lines = new List<TraceLine>();

        internal static void Init()
        {
            lock (@lock)
            {
                lines.Clear();
            }

            if (listener == null)
                Trace.Listeners.Add(listener = new PrismTraceListener());
        }

        internal static void AddLine(TraceLine line)
        {
            lock (@lock)
            {
                if (lines.Count == MAX_LINES)
                    lines.RemoveAt(0);

                lines.Add(line);
            }
        }

        internal static void Update()
        {
            lock (@lock)
            {
                for (int i = 0; i < lines.Count; i++)
                    if (lines[i].Timeleft <= 1)
                        lines.RemoveAt(i--);
                    else
                        lines[i] = new TraceLine(lines[i].Text, lines[i].Timeleft - 1);
            }
        }
    }
}
