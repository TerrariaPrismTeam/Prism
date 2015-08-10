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
                  ,description   : ""  
                  ,usageText     : ""  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 1   
                  ,maxArgs       : 1    
                  )
        {

        }

        public override void Run(string args, List<string> splitArgs)
        {
            
        }
    }
}
