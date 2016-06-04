using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandGet : ChatCommand
    {
        public ChatCommandGet()
            : base(name          : "Get"
                  ,description   : "Gets the value of a debugging variable."  
                  ,usageText     : "/Get [VariableName]"  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 1 
                  ,maxArgs       : 1    
                  )
        {

        }

        public override void Run(string args, List<string> splitArgs)
        {
            ChatConsole.Error("Will be implemented as soon as the debug variable system is created and implemented.");
        }
    }
}
