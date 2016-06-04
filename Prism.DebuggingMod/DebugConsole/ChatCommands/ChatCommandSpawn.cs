using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandSpawn : ChatCommand
    {
        public ChatCommandSpawn()
            : base(name          : ""
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

        //public static void ItemGivenText(Queue<Item> itemsGiven)
        //{
        //    string text = "";

        //    while (itemsGiven.Count > 0)
        //    {
        //        int i = itemsGiven.Dequeue();
        //        text += " [i/s" + Main.item[i].stack + "/p" + Main.item[i].prefix + ":" + Main.item[i].type + "]";
        //    }
            
        //    Helpers.Main.RandColorText("You have been given" + text + ".", true);
        //}
    }
}
