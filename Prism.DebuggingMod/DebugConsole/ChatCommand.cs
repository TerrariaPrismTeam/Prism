using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.DebuggingMod.ChatConsole
{
    public abstract class ChatCommand
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string UsageText;
        public readonly bool RequiresArgs;
        public readonly bool CaseSensitive;
        public readonly int MinArgs;
        public readonly int MaxArgs;

        public ChatCommand(string name, string description, string usageText, bool requiresArgs, bool caseSensitive, int minArgs, int maxArgs)
        {
            Name = name;
            RequiresArgs = requiresArgs;
            UsageText = usageText;
            Description = description;
            CaseSensitive = caseSensitive;
            MinArgs = minArgs;
            MaxArgs = maxArgs;
        }

        public abstract void Run(string args, List<string> splitArgs);

        public void DisplayHelp()
        {
            ChatConsole.Info(Name + "\n" + Description + "\nUsage: " + UsageText);
        }
    }
}
