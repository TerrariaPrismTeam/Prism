using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Prism.Debugging
{
    struct TraceLine : IEquatable<TraceLine>
    {
        public readonly string OrigText;

        public string Text;
        public int Timeleft;

        internal int times;

        public TraceLine(string text, int timeLeft)
        {
            Text = OrigText = text;
            Timeleft = timeLeft;

            times = 0;
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

            Text = OrigText = sb.ToString();
            Timeleft = timeLeft <= 0 ? 180 : timeLeft;

            times = 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is TraceLine)
                return Equals((TraceLine)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return OrigText.GetHashCode();
        }
        public override string ToString()
        {
            return Text ?? String.Empty;
        }

        public bool Equals(TraceLine other)
        {
            return Timeleft == other.Timeleft && (Text == other.Text || Timeleft == 0);
        }

        public static bool operator ==(TraceLine a, TraceLine b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(TraceLine a, TraceLine b)
        {
            return !a.Equals(b);
        }
    }

    static class PrismDebug
    {
        const int MAX_LINES = 10;

        public const int PADDING_X = 48, PADDING_Y = 24;

        internal readonly static object @lock = new object();
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
                if (lines.Any(l => l.OrigText == line.Text))
                {
                    var toEdit = lines.Last(l => l.OrigText == line.Text);
                    var index = lines.LastIndexOf(toEdit);

                    lines.RemoveAt(index);

                    line.times = toEdit.times + 1;

                    line.Text += " (" + (line.times + 1) + " times)";
                }

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
