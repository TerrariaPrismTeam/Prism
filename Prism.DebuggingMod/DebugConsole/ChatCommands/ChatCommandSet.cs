using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandSet : ChatCommand
    {
        public ChatCommandSet()
            : base(name          : "Set"
                  ,description   : "Sets a debug variable."  
                  ,usageText     : "/Set {VariableName} {NewValue}"  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 2
                  ,maxArgs       : 2
                  )
        {

        }

        public override void Run(string args, List<string> splitArgs)
        {
            ChatConsole.Error("Will be implemented as soon as the debug variable system is created and implemented.");
        }
    }
}
