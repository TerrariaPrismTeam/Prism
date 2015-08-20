using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Util
{
    public enum ArgType
    {
        Flag,
        Value,
        Verb,
        KeyValuePair
    }
    public struct Argument
    {
        public ArgType Type;
        /// <summary>
        /// Null if <see cref="Type" /> is equal to <see cref="ArgType.Value" />.
        /// </summary>
        public string Name;
        /// <summary>
        /// Null if <see cref="Type" /> is equal to <see cref="ArgType.Flag" /> or <see cref="ArgType.Verb" />.
        /// </summary>
        public string Value;
    }

    public static class CommandLineArgs
    {
        readonly static string
            FORWARD_SLASH = "/",
            SINGLE_DASH   = "-",
            DOUBLE_DASH   = "--";

        readonly static char[]
            KVP_SEPARATORS = { ':', '=' },
            ARG_PREFIXES   = { '-', '/' };

        static bool IsWord(string s)
        {
            return s.Any(c => Char.IsLetterOrDigit(c) || c == '_' || c == '-');
        }
        static KeyValuePair<string, string> GetKvp(string arg)
        {
            var split = arg.TrimStart(ARG_PREFIXES).Split(KVP_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 1)
                return new KeyValuePair<string, string>(split[0], null);
            else if (split.Length == 2)
                return new KeyValuePair<string, string>(split[0], split[1]);

            throw new FormatException("Invalid argument passed in the command-line: \"" + arg + "\"");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <param name="shortToLong">Keys must be culture-invariant uppercase.</param>
        /// <returns></returns>
        public static Argument[] Parse(string[] args, Dictionary<char, string> shortToLong)
        {
            var ret = new List<Argument>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(FORWARD_SLASH, StringComparison.Ordinal))
                {
                    var t = GetKvp(args[i]);

                    var fu = Char.ToUpperInvariant(t.Key[0]);
                    var n = (t.Key.Length == 1 && shortToLong.ContainsKey(fu) ? shortToLong[fu] : t.Key).ToUpperInvariant();

                    ret.Add(t.Value == null ? new Argument()
                    {
                        Type = ArgType.Flag,
                        Name = n
                    } : new Argument()
                    {
                        Type = ArgType.KeyValuePair,
                        Name = n,
                        Value = t.Value
                    });
                }
                else if (args[i].StartsWith(DOUBLE_DASH, StringComparison.Ordinal))
                {
                    var t = GetKvp(args[i]);
                    var n = t.Key.ToUpperInvariant();

                    ret.Add(t.Value == null ? new Argument()
                    {
                        Type = ArgType.Flag,
                        Name = n
                    } : new Argument()
                    {
                        Type = ArgType.KeyValuePair,
                        Name = n,
                        Value = t.Value
                    });
                }
                else if (args[i].StartsWith(SINGLE_DASH, StringComparison.Ordinal))
                {
                    var t = GetKvp(args[i]);

                    var fu = Char.ToUpperInvariant(t.Key[0]);
                    if (t.Key.Length != 1 || !shortToLong.ContainsKey(fu))
                        throw new FormatException("Command-line option shorthand '" + fu + "' does not exist.");
                    var n = shortToLong[fu].ToUpperInvariant();

                    ret.Add(t.Value == null ? new Argument()
                    {
                        Type = ArgType.Flag,
                        Name = n
                    } : new Argument()
                    {
                        Type = ArgType.KeyValuePair,
                        Name = n,
                        Value = t.Value
                    });
                }
                else if (IsWord(args[i]))
                    ret.Add(new Argument()
                    {
                        Type = ArgType.Verb,
                        Name = args[i].ToUpperInvariant()
                    });
                else
                    ret.Add(new Argument()
                    {
                        Type = ArgType.Value,
                        Value = args[i]
                    });
            }

            return ret.ToArray();
        }
    }
}
