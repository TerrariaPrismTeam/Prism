using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandHelp : ChatCommand
    {
        public ChatCommandHelp()
            : base(name          : "Help"
                  ,description   : "Prints help text for any command."  
                  ,usageText     : "/help command"  
                  ,requiresArgs  : true
                  ,caseSensitive : false
                  ,minArgs       : 1   
                  ,maxArgs       : 1    
                  )
        {

        }

        public override void Run(string args, List<string> splitArgs)
        {
            ChatConsole.Commands[splitArgs[0]].DisplayHelp();
        }
    }
}
