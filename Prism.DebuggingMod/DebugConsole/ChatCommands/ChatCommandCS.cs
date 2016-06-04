using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandCS : ChatCommand
    {
        public ChatCommandCS()
            : base(name          : "CS"
                  ,description   : "Runs static C# code."  
                  ,usageText     : "/cs {code}"  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 1   
                  ,maxArgs       : 1    
                  )
        {

        }

        public override void Run(string args, List<string> splitArgs)
        {
            ChatConsole.Error("Not yet implemented; PoroCYon is currently (supposedly) writing the lib for this...");
        }
    }
}
